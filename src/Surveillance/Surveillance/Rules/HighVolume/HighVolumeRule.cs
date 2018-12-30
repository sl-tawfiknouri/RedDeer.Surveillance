using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Markets;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Factories;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.HighVolume
{
    public class HighVolumeRule : BaseUniverseRule, IHighVolumeRule
    {
        private readonly IHighVolumeRuleParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly IDataRequestMessageSender _dataRequestSender;
        private readonly ILogger _logger;

        private bool _hadMissingData = false;

        public HighVolumeRule(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursManager tradingHoursManager,
            IDataRequestMessageSender dataRequestSender,
            ILogger<IHighVolumeRule> logger) 
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                DomainV2.Scheduling.Rules.HighVolume,
                HighVolumeRuleFactory.Version,
                "High Volume Rule",
                opCtx,
                factory,
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _dataRequestSender = dataRequestSender ?? throw new ArgumentNullException(nameof(dataRequestSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                    _parameters.WindowSize, 
                    tradePosition,
                    mostRecentTrade?.Instrument,
                    _parameters,
                    dailyBreach,
                    windowBreach,
                    marketCapBreach,
                    tradedVolume);

            var message = new UniverseAlertEvent(DomainV2.Scheduling.Rules.HighVolume, breach, _ruleCtx);
            _alertStream.Add(message);
        }

        private HighVolumeRuleBreach.BreachDetails CheckDailyVolume(Order mostRecentTrade, long tradedVolume)
        {
            var dailyBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_parameters.HighVolumePercentageDaily.HasValue)
            {
                dailyBreach = DailyVolumeCheck(mostRecentTrade, tradedVolume);
            }

            return dailyBreach;
        }

        private HighVolumeRuleBreach.BreachDetails CheckWindowVolume(Order mostRecentTrade, long tradedVolume)
        {
            var windowBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_parameters.HighVolumePercentageWindow.HasValue)
            {
                windowBreach = WindowVolumeCheck(mostRecentTrade, tradedVolume);
            }

            return windowBreach;
        }

        private HighVolumeRuleBreach.BreachDetails CheckMarketCap(Order mostRecentTrade, List<Order> tradedSecurities)
        {
            var marketCapBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_parameters.HighVolumePercentageMarketCap.HasValue)
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

            var tradingHours = _tradingHoursManager.Get(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"HighVolumeRule. Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                _ruleCtx?.Id()); 

            var securityResult = UniverseMarketCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogWarning($"High Volume Rule. Missing data for {marketDataRequest}.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            var threshold = (long)Math.Ceiling(_parameters.HighVolumePercentageDaily.GetValueOrDefault(0) * security.DailyVolume.Traded);

            if (threshold <= 0)
            {
                _hadMissingData = true;
                _logger.LogError($"High Volume Rule. Daily volume threshold of {threshold} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var breachPercentage =
                security.DailyVolume.Traded != 0 && tradedVolume != 0
                    ? (decimal)tradedVolume / (decimal)security.DailyVolume.Traded
                    : 0;

            if (tradedVolume >= threshold)
            {
                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, threshold);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        private HighVolumeRuleBreach.BreachDetails WindowVolumeCheck(Order mostRecentTrade, long tradedVolume)
        {
            var tradingHours = _tradingHoursManager.Get(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"HighVolumeRule. Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market?.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHours.OpeningInUtcForDay(UniverseDateTime),
                    tradingHours.ClosingInUtcForDay(UniverseDateTime),
                    _ruleCtx?.Id());

            var marketResult = UniverseMarketCache.GetMarkets(marketRequest);

            if (marketResult.HadMissingData)
            {
                _logger.LogInformation($"High Volume unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var securityDataTicks = marketResult.Response;           
            var windowVolume = securityDataTicks.Sum(sdt => sdt.Volume.Traded);
            var threshold = (long)Math.Ceiling(_parameters.HighVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

            var breachPercentage =
                windowVolume != 0 && tradedVolume != 0
                    ? (decimal)tradedVolume / (decimal)windowVolume
                    : 0;

            if (threshold <= 0)
            {
                _hadMissingData = true;
                _logger.LogError($"High Volume Rule. Daily volume threshold of {threshold} was recorded.");
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

            var tradingHours = _tradingHoursManager.Get(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"HighVolumeRule. Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime),
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(UniverseDateTime),
                _ruleCtx?.Id());

            var securityResult = UniverseMarketCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogWarning($"High Volume Rule. Missing data for {marketDataRequest}.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            double thresholdValue =
                (double)Math.Ceiling(_parameters.HighVolumePercentageMarketCap.GetValueOrDefault(0)
                * security.MarketCap.GetValueOrDefault(0));

            if (thresholdValue <= 0)
            {
                _hadMissingData = true;
                _logger.LogError($"High Volume Rule. Market cap threshold of {thresholdValue} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradedValue =
                (double)trades
                    .Select(tr => tr.OrderFilledVolume.GetValueOrDefault(0) * (tr.OrderAveragePrice.GetValueOrDefault().Value))
                    .Sum();

            var breachPercentage =
                security.MarketCap.GetValueOrDefault(0) != 0 && tradedValue != 0
                    ? (decimal)tradedValue / (decimal)security.MarketCap.GetValueOrDefault(0)
                    : 0;

            if (tradedValue >= thresholdValue)
            {
                var thresholdCurrencyValue = new CurrencyAmount((decimal)thresholdValue, security.Spread.Price.Currency);
                var tradedCurrencyValue = new CurrencyAmount((decimal)tradedValue, security.Spread.Price.Currency);

                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, thresholdCurrencyValue, tradedCurrencyValue);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        { }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred in the High Volume Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred in the High Volume Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred in the High Volume Rule at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured in the High Volume Rule");

            var alert = new UniverseAlertEvent(DomainV2.Scheduling.Rules.HighVolume, null, _ruleCtx, true);
            _alertStream.Add(alert);

            if (_hadMissingData)
            {
                var requestTask = _dataRequestSender.Send(_ruleCtx.Id());
                requestTask.Wait();
                _ruleCtx.EndEvent().EndEventWithMissingDataError();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
