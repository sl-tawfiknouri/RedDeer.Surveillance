using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    /// <summary>
    /// We've tried to solve some issues imperatively with logic that can be
    /// solved with using statistics - such as using cross correlation and/or auto correlations
    /// in order to keep this rule simple/easy to understand
    /// if we need to increase the sophistication of this rule - stats is the route to go down - RT 07/05/2019
    /// </summary>
    public class RampingRule : BaseUniverseRule, IRampingRule
    {
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private readonly IRampingRuleEquitiesParameters _rampingParameters;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IRampingAnalyser _rampingAnalyser;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;

        private readonly ILogger _logger;
        private bool _hadMissingData = false;

        public RampingRule(
            IRampingRuleEquitiesParameters rampingParameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            IUniverseOrderFilter orderFilter,
            RuleRunMode runMode,
            IRampingAnalyser rampingAnalyser,
            IMarketTradingHoursService tradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                rampingParameters?.WindowSize ?? TimeSpan.FromDays(7),
                TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.Ramping,
                EquityRuleRampingFactory.Version,
                "Ramping Rule",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _rampingParameters = rampingParameters ?? throw new ArgumentNullException(nameof(rampingParameters));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _rampingAnalyser = rampingAnalyser ?? throw new ArgumentNullException(nameof(rampingAnalyser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            if (!ExceedsTradingFrequencyThreshold(tradeWindow))
            {
                // LOG THEN EXIT
                _logger.LogInformation($"Trading Frequency of {_rampingParameters.ThresholdOrdersExecutedInWindow} was not exceeded. Returning.");
                return;
            }

            if (!ExceedsTradingVolumeInWindowThreshold(tradeWindow.ToList(), tradeWindow.Peek()))
            {
                // LOG THEN EXIT
                _logger.LogInformation($"Trading Volume of {_rampingParameters.ThresholdVolumePercentageWindow} was not exceeded. Returning.");
                return;
            }

            var lastTrade = tradeWindow.Peek();
            var tradingHours = _tradingHoursService.GetTradingHoursForMic(lastTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {lastTrade.Market?.MarketIdentifierCode}");
                return;
            }

            var tradingDates = _tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                lastTrade.Market?.MarketIdentifierCode);

            var marketDataRequest = new MarketDataRequest(
                lastTrade.Market?.MarketIdentifierCode,
                lastTrade.Instrument.Cfi,
                lastTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                _ruleCtx?.Id(),
                DataSource.AllIntraday);

            var marketData = UniverseEquityIntradayCache.GetMarketsForRange(marketDataRequest, tradingDates, RunMode);

            if (marketData.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogWarning($"Missing data for {marketDataRequest}.");
                return;
            }

            var windowStackCopy = new Stack<Order>(tradeWindow);
            var rampingOrders = new Stack<Order>(windowStackCopy);
            var rampingAnalysisResults = new List<IRampingStrategySummaryPanel>();

            while (rampingOrders.Any())
            {
                var rampingOrderList = rampingOrders.ToList();
                var rootOrder = rampingOrders.Peek();
                var marketDataSubset = marketData.Response.Where(_ => _.TimeStamp <= rootOrder.FilledDate).ToList();
                var rampingAnalysisResult = _rampingAnalyser.Analyse(rampingOrderList, marketDataSubset);
                rampingAnalysisResults.Add(rampingAnalysisResult);
                rampingOrders.Pop();
            }

            if (!rampingAnalysisResults.Any()
                || !rampingAnalysisResults.First().HasRampingStrategy()
                || rampingAnalysisResults.All(_ => !_.HasRampingStrategy()))
            {
                // LOG THEN EXIT
                _logger.LogInformation($"A rule breach was not detected for {lastTrade?.Instrument?.Identifiers}. Returning.");
                return;
            }

            var rampingPrevalence = RampingPrevalence(rampingAnalysisResults);
            if (rampingPrevalence < _rampingParameters.AutoCorrelationCoefficient)
            {
                // LOG THEN EXIT
                _logger.LogInformation($"A rule breach was not detected due to an auto correlation of {rampingPrevalence} for {lastTrade?.Instrument?.Identifiers}. Returning.");
                return;
            }

            var tradePosition = new TradePosition(tradeWindow.ToList());

            var breach =
                new RampingRuleBreach(
                    BackwardWindowSize,
                    tradePosition,
                    lastTrade.Instrument,
                    _rampingParameters.Id,
                    _ruleCtx?.Id(),
                    _ruleCtx?.CorrelationId(),
                    OrganisationFactorValue,
                    rampingAnalysisResults.Last());

            _logger.LogInformation($"RunRule has breached parameter conditions for {lastTrade?.Instrument?.Identifiers}. Adding message to alert stream.");
            var message = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Ramping, breach, _ruleCtx);
            _alertStream.Add(message);
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        public decimal RampingPrevalence(List<IRampingStrategySummaryPanel> panels)
        {
            if (panels == null
                || !panels.Any())
            {
                return 0;
            }

            decimal rampCount = panels.Count(_ => _.HasRampingStrategy());
            decimal totals = panels.Count;

            if (rampCount == 0)
            {
                return 0;
            }

            var rampingPercentage = rampCount / totals;

            return rampingPercentage;
        }

        private bool ExceedsTradingFrequencyThreshold(Stack<Order> orders)
        {
            if (_rampingParameters?.ThresholdOrdersExecutedInWindow == null)
            {
                return true;
            }

            if (orders == null
                || !orders.Any())
            {
                return false;
            }

            return orders.Count >= _rampingParameters.ThresholdOrdersExecutedInWindow.GetValueOrDefault(0);
        }

        private bool ExceedsTradingVolumeInWindowThreshold(List<Order> orders, Order mostRecentTrade)
        {
            if (_rampingParameters?.ThresholdVolumePercentageWindow == null
                || _rampingParameters.ThresholdVolumePercentageWindow <= 0
                || orders == null
                || !orders.Any())
            {
                return true;
            }

            var tradingHours = _tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var tradingDates = _tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                mostRecentTrade.Market?.MarketIdentifierCode);

            var marketRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market?.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(BackwardWindowSize)),
                    tradingHours.ClosingInUtcForDay(UniverseDateTime),
                    _ruleCtx?.Id(),
                    DataSource.AllIntraday);

            var marketResult = UniverseEquityIntradayCache.GetMarketsForRange(marketRequest, tradingDates, RunMode);

            if (marketResult.HadMissingData)
            {
                _logger.LogTrace($"Unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return false;
            }

            var securityDataTicks = marketResult.Response;
            var windowVolume = securityDataTicks.Sum(sdt => sdt.SpreadTimeBar.Volume.Traded);
            var threshold = (long)Math.Ceiling(_rampingParameters.ThresholdVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

            if (threshold <= 0)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
                return false;
            }

            var tradedVolume = orders.Sum(_ => _.OrderFilledVolume.GetValueOrDefault(0));
                
            if (tradedVolume >= threshold)
            {
                return true;
            }

            return false;
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        { 
            // we don't use post order event in ramping rule
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // we don't use post order event in ramping rule
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured");

            if (_hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Ramping, null, _ruleCtx, false, true);
                _alertStream.Add(alert);

                _dataRequestSubscriber.SubmitRequest();
            }

            _ruleCtx?.EndEvent();
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (RampingRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (RampingRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
