namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

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

    /// <summary>
    ///     We've tried to solve some issues imperatively with logic that can be
    ///     solved with using statistics - such as using cross correlation and/or auto correlations
    ///     in order to keep this rule simple/easy to understand
    ///     if we need to increase the sophistication of this rule - stats is the route to go down - RT 07/05/2019
    /// </summary>
    public class RampingRule : BaseUniverseRule, IRampingRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly ILogger _logger;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly IRampingAnalyser _rampingAnalyser;

        private readonly IRampingRuleEquitiesParameters _rampingParameters;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private bool _hadMissingData;

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
                rampingParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(7),
                rampingParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.Ramping,
                EquityRuleRampingFactory.Version,
                "Ramping Rule",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this._rampingParameters = rampingParameters ?? throw new ArgumentNullException(nameof(rampingParameters));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._rampingAnalyser = rampingAnalyser ?? throw new ArgumentNullException(nameof(rampingAnalyser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._dataRequestSubscriber =
                dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (RampingRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (RampingRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public decimal RampingPrevalence(List<IRampingStrategySummaryPanel> panels)
        {
            if (panels == null || !panels.Any()) return 0;

            decimal rampCount = panels.Count(_ => _.HasRampingStrategy());
            decimal totals = panels.Count;

            if (rampCount == 0) return 0;

            var rampingPercentage = rampCount / totals;

            return rampingPercentage;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null || !tradeWindow.Any()) return;

            if (!this.ExceedsTradingFrequencyThreshold(tradeWindow))
            {
                // LOG THEN EXIT
                this._logger.LogInformation(
                    $"Trading Frequency of {this._rampingParameters.ThresholdOrdersExecutedInWindow} was not exceeded. Returning.");
                return;
            }

            if (!this.ExceedsTradingVolumeInWindowThreshold(
                    tradeWindow.ToList(),
                    tradeWindow.Any() ? tradeWindow.Peek() : null))
            {
                // LOG THEN EXIT
                this._logger.LogInformation(
                    $"Trading Volume of {this._rampingParameters.ThresholdVolumePercentageWindow} was not exceeded. Returning.");
                return;
            }

            var lastTrade = tradeWindow.Any() ? tradeWindow.Peek() : null;

            if (lastTrade == null)
            {
                this._logger.LogError("Last trade was null, this is unusual. Returning.");
                return;
            }

            var tradingHours = this._tradingHoursService.GetTradingHoursForMic(lastTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {lastTrade.Market?.MarketIdentifierCode}");
                return;
            }

            var tradingDates = this._tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                lastTrade.Market?.MarketIdentifierCode);

            var marketDataRequest = new MarketDataRequest(
                lastTrade.Market?.MarketIdentifierCode,
                lastTrade.Instrument.Cfi,
                lastTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                this._ruleCtx?.Id(),
                DataSource.AllIntraday);

            var marketData = this.UniverseEquityIntradayCache.GetMarketsForRange(
                marketDataRequest,
                tradingDates,
                this.RunMode);

            if (marketData.HadMissingData)
            {
                this._hadMissingData = true;
                this._logger.LogWarning($"Missing data for {marketDataRequest}.");
                return;
            }

            var windowStackCopy = new Stack<Order>(tradeWindow);
            var rampingOrders = new Stack<Order>(windowStackCopy);
            var rampingAnalysisResults = new List<IRampingStrategySummaryPanel>();

            while (rampingOrders.Any())
            {
                var rampingOrderList = rampingOrders.ToList();
                var rootOrder = rampingOrders.Any() ? rampingOrders.Peek() : null;
                var marketDataSubset = marketData.Response.Where(_ => _.TimeStamp <= rootOrder.FilledDate).ToList();
                var rampingAnalysisResult = this._rampingAnalyser.Analyse(rampingOrderList, marketDataSubset);
                rampingAnalysisResults.Add(rampingAnalysisResult);
                rampingOrders.Pop();
            }

            if (!rampingAnalysisResults.Any() || !rampingAnalysisResults.First().HasRampingStrategy()
                                              || rampingAnalysisResults.All(_ => !_.HasRampingStrategy()))
            {
                // LOG THEN EXIT
                this._logger.LogInformation(
                    $"A rule breach was not detected for {lastTrade?.Instrument?.Identifiers}. Returning.");
                return;
            }

            var rampingPrevalence = this.RampingPrevalence(rampingAnalysisResults);
            if (rampingPrevalence < this._rampingParameters.AutoCorrelationCoefficient)
            {
                // LOG THEN EXIT
                this._logger.LogInformation(
                    $"A rule breach was not detected due to an auto correlation of {rampingPrevalence} for {lastTrade?.Instrument?.Identifiers}. Returning.");
                return;
            }

            var tradePosition = new TradePosition(tradeWindow.ToList());

            // wrong but should be a judgement
            var breach = new RampingRuleBreach(
                this.BackwardWindowSize,
                tradePosition,
                lastTrade.Instrument,
                this._rampingParameters.Id,
                this._ruleCtx?.Id(),
                this._ruleCtx?.CorrelationId(),
                this.OrganisationFactorValue,
                rampingAnalysisResults.Last(),
                this._rampingParameters,
                null,
                null,
                this.UniverseDateTime);

            this._logger.LogInformation(
                $"RunRule has breached parameter conditions for {lastTrade?.Instrument?.Identifiers}. Adding message to alert stream.");
            var message = new UniverseAlertEvent(Rules.Ramping, breach, this._ruleCtx);
            this._alertStream.Add(message);
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation("Eschaton occured");

            if (this._hadMissingData && this.RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(Rules.Ramping, null, this._ruleCtx, false, true);
                this._alertStream.Add(alert);

                this._dataRequestSubscriber.SubmitRequest();
            }

            this._ruleCtx?.EndEvent();
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation("Genesis occurred");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // we don't use post order event in ramping rule
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            // we don't use post order event in ramping rule
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private bool ExceedsTradingFrequencyThreshold(Stack<Order> orders)
        {
            if (this._rampingParameters?.ThresholdOrdersExecutedInWindow == null) return true;

            if (orders == null || !orders.Any()) return false;

            return orders.Count >= this._rampingParameters.ThresholdOrdersExecutedInWindow.GetValueOrDefault(0);
        }

        private bool ExceedsTradingVolumeInWindowThreshold(List<Order> orders, Order mostRecentTrade)
        {
            if (this._rampingParameters?.ThresholdVolumePercentageWindow == null
                || this._rampingParameters.ThresholdVolumePercentageWindow <= 0 || orders == null
                || !orders.Any()) return true;

            var tradingHours =
                this._tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");

            var tradingDates = this._tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                mostRecentTrade.Market?.MarketIdentifierCode);

            var marketRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                this._ruleCtx?.Id(),
                DataSource.AllIntraday);

            var marketResult = this.UniverseEquityIntradayCache.GetMarketsForRange(
                marketRequest,
                tradingDates,
                this.RunMode);

            if (marketResult.HadMissingData)
            {
                this._logger.LogTrace(
                    $"Unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {this.UniverseDateTime}.");

                this._hadMissingData = true;
                return false;
            }

            var securityDataTicks = marketResult.Response;
            var windowVolume = securityDataTicks.Sum(sdt => sdt.SpreadTimeBar.Volume.Traded);
            var threshold = (long)Math.Ceiling(
                this._rampingParameters.ThresholdVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

            if (threshold <= 0)
            {
                this._hadMissingData = true;
                this._logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
                return false;
            }

            var tradedVolume = orders.Sum(_ => _.OrderFilledVolume.GetValueOrDefault(0));

            if (tradedVolume >= threshold) return true;

            return false;
        }
    }
}