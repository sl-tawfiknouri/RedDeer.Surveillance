using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial;
using Domain.Core.Financial.Money;
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
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume
{
    public class HighVolumeRule : BaseUniverseRule, IHighVolumeRule
    {
        private readonly IHighVolumeRuleEquitiesParameters _equitiesParameters;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private readonly ILogger _logger;

        private bool _hadMissingData = false;

        public HighVolumeRule(
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursManager tradingHoursManager,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<IHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger) 
            : base(
                equitiesParameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Surveillance.Scheduling.Rules.HighVolume,
                EquityRuleHighVolumeFactory.Version,
                "High Volume Rule",
                opCtx,
                factory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            _ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            var tradedSecurities =
                tradeWindow
                    .Where(tr =>
                        tr.OrderStatus() == OrderStatus.Filled)
                    .ToList();

            var tradedVolume =
                tradedSecurities
                    .Sum(tr => tr.OrderFilledVolume.GetValueOrDefault(0));

            var tradePosition = new TradePosition(tradeWindow.ToList());
            var mostRecentTrade = tradeWindow.Pop();

            var dailyBreach = CheckDailyVolume(mostRecentTrade, tradedVolume);
            var windowBreach = CheckWindowVolume(mostRecentTrade, tradedVolume);
            var marketCapBreach = CheckMarketCap(mostRecentTrade, tradedSecurities);

            if (HasNoBreach(dailyBreach, windowBreach, marketCapBreach))
            {
                return;
            }

            var breach =
                new HighVolumeRuleBreach(
                    OrganisationFactorValue,
                    _ruleCtx.SystemProcessOperationContext(),
                    _ruleCtx.CorrelationId(),
                    _equitiesParameters.WindowSize, 
                    tradePosition,
                    mostRecentTrade?.Instrument,
                    _equitiesParameters,
                    dailyBreach,
                    windowBreach,
                    marketCapBreach,
                    tradedVolume);

            _logger.LogInformation($"RunRule had a breach for {mostRecentTrade?.Instrument?.Identifiers}. Daily Breach {dailyBreach?.HasBreach} | Window Breach {windowBreach?.HasBreach} | Market Cap Breach {marketCapBreach?.HasBreach}. Passing to alert stream.");
            var message = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.HighVolume, breach, _ruleCtx);
            _alertStream.Add(message);
        }

        private HighVolumeRuleBreach.BreachDetails CheckDailyVolume(Order mostRecentTrade, long tradedVolume)
        {
            var dailyBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_equitiesParameters.HighVolumePercentageDaily.HasValue)
            {
                dailyBreach = DailyVolumeCheck(mostRecentTrade, tradedVolume);
            }

            return dailyBreach;
        }

        private HighVolumeRuleBreach.BreachDetails CheckWindowVolume(Order mostRecentTrade, long tradedVolume)
        {
            var windowBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_equitiesParameters.HighVolumePercentageWindow.HasValue)
            {
                windowBreach = WindowVolumeCheck(mostRecentTrade, tradedVolume);
            }

            return windowBreach;
        }

        private HighVolumeRuleBreach.BreachDetails CheckMarketCap(Order mostRecentTrade, List<Order> tradedSecurities)
        {
            var marketCapBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_equitiesParameters.HighVolumePercentageMarketCap.HasValue)
            {
                marketCapBreach = MarketCapCheck(mostRecentTrade, tradedSecurities);
            }

            return marketCapBreach;
        }

        private bool HasNoBreach(
            HighVolumeRuleBreach.BreachDetails dailyBreach,
            HighVolumeRuleBreach.BreachDetails windowBreach,
            HighVolumeRuleBreach.BreachDetails marketCapBreach)
        {
            return (!dailyBreach?.HasBreach ?? true)
                   && (!windowBreach?.HasBreach ?? true)
                   && (!marketCapBreach?.HasBreach ?? true);
        }

        private HighVolumeRuleBreach.BreachDetails DailyVolumeCheck(Order mostRecentTrade, long tradedVolume)
        {
            if (mostRecentTrade == null)
            {
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradingHours = _tradingHoursManager.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(WindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                _ruleCtx?.Id()); 

            var securityResult = UniverseEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogWarning($"Missing data for {marketDataRequest}.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            var threshold = (long)Math.Ceiling(
                _equitiesParameters.HighVolumePercentageDaily.GetValueOrDefault(0) * security.DailySummaryTimeBar.DailyVolume.Traded);

            if (threshold <= 0)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var breachPercentage =
                security.DailySummaryTimeBar.DailyVolume.Traded != 0 && tradedVolume != 0
                    ? (decimal)tradedVolume / (decimal)security.DailySummaryTimeBar.DailyVolume.Traded
                    : 0;

            if (tradedVolume >= threshold)
            {
                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, threshold);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        private HighVolumeRuleBreach.BreachDetails WindowVolumeCheck(Order mostRecentTrade, long tradedVolume)
        {
            var tradingHours = _tradingHoursManager.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var tradingDates = _tradingHoursManager.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(WindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                mostRecentTrade.Market?.MarketIdentifierCode);

            var marketRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market?.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(WindowSize)),
                    tradingHours.ClosingInUtcForDay(UniverseDateTime),
                    _ruleCtx?.Id());

            var marketResult = UniverseEquityIntradayCache.GetMarketsForRange(marketRequest, tradingDates, RunMode);

            if (marketResult.HadMissingData)
            {
                _logger.LogTrace($"Unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var securityDataTicks = marketResult.Response;           
            var windowVolume = securityDataTicks.Sum(sdt => sdt.SpreadTimeBar.Volume.Traded);
            var threshold = (long)Math.Ceiling(_equitiesParameters.HighVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

            var breachPercentage =
                windowVolume != 0 && tradedVolume != 0
                    ? (decimal)tradedVolume / (decimal)windowVolume
                    : 0;

            if (threshold <= 0)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            if (tradedVolume >= threshold)
            {
                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, threshold);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        private HighVolumeRuleBreach.BreachDetails MarketCapCheck(Order mostRecentTrade, List<Order> trades)
        {
            if (trades == null
                || !trades.Any())
            {
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradingHours = _tradingHoursManager.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(WindowSize)),
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(UniverseDateTime),
                _ruleCtx?.Id());

            var securityResult = UniverseEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Missing data for {marketDataRequest}.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            double thresholdValue =
                (double)Math.Ceiling(_equitiesParameters.HighVolumePercentageMarketCap.GetValueOrDefault(0)
                * security.DailySummaryTimeBar.MarketCap.GetValueOrDefault(0));

            if (thresholdValue <= 0)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Market cap threshold of {thresholdValue} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradedValue =
                (double)trades
                    .Select(tr => tr.OrderFilledVolume.GetValueOrDefault(0) * (tr.OrderAverageFillPrice.GetValueOrDefault().Value))
                    .Sum();

            var breachPercentage =
                security.DailySummaryTimeBar.MarketCap.GetValueOrDefault(0) != 0 && tradedValue != 0
                    ? (decimal)tradedValue / (decimal)security.DailySummaryTimeBar.MarketCap.GetValueOrDefault(0)
                    : 0;

            if (tradedValue >= thresholdValue)
            {
                var thresholdMoney = new Money((decimal)thresholdValue, mostRecentTrade.OrderCurrency);
                var tradedMoney = new Money((decimal)tradedValue, mostRecentTrade.OrderCurrency);

                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, thresholdMoney, tradedMoney);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        { }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured");

            if (_hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.HighVolume, null, _ruleCtx, false, true);
                _alertStream.Add(alert);

                _dataRequestSubscriber.SubmitRequest();
                _ruleCtx.EndEvent();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (HighVolumeRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (HighVolumeRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
