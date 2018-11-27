using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Finance;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Factories;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.HighVolume
{
    public class HighVolumeRule : BaseUniverseRule, IHighVolumeRule
    {
        private readonly IHighVolumeRuleParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ILogger _logger;

        private bool _hadMissingData = false;

        public HighVolumeRule(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            ILogger<IHighVolumeRule> logger) 
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Scheduling.Rules.HighVolume,
                HighVolumeRuleFactory.Version,
                "High Volume Rule",
                opCtx,
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                        tr.OrderStatus == OrderStatus.Fulfilled
                        || tr.OrderStatus == OrderStatus.PartialFulfilled)
                    .ToList();

            var tradedVolume =
                tradedSecurities
                    .Sum(tr => tr.FulfilledVolume);

            var tradePosition = new TradePosition(tradeWindow.ToList());
            var mostRecentTrade = tradeWindow.Pop();

            HighVolumeRuleBreach.BreachDetails dailyBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_parameters.HighVolumePercentageDaily.HasValue)
            {
                dailyBreach = DailyVolumeCheck(mostRecentTrade, tradedVolume);
            }

            HighVolumeRuleBreach.BreachDetails windowBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_parameters.HighVolumePercentageWindow.HasValue)
            {
                windowBreach = WindowVolumeCheck(mostRecentTrade, tradedVolume);
            }

            HighVolumeRuleBreach.BreachDetails marketCapBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_parameters.HighVolumePercentageMarketCap.HasValue)
            {
                marketCapBreach = MarketCapCheck(mostRecentTrade, tradedSecurities);
            }

            if ((!dailyBreach?.HasBreach ?? true)
                && (!windowBreach?.HasBreach ?? true)
                && (!marketCapBreach?.HasBreach ?? true))
            {
                return;
            }

            var breach =
                new HighVolumeRuleBreach(
                    _parameters.WindowSize, 
                    tradePosition,
                    mostRecentTrade?.Security,
                    _parameters,
                    dailyBreach,
                    windowBreach,
                    marketCapBreach,
                    tradedVolume);

            var message = new UniverseAlertEvent(Domain.Scheduling.Rules.HighVolume, breach, _ruleCtx);
            _alertStream.Add(message);
        }

        private HighVolumeRuleBreach.BreachDetails DailyVolumeCheck(TradeOrderFrame mostRecentTrade, int tradedVolume)
        {
            if (!LatestExchangeFrameBook.ContainsKey(mostRecentTrade.Market.Id))
            {
                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            LatestExchangeFrameBook.TryGetValue(mostRecentTrade.Market.Id, out var exchangeFrame);

            if (exchangeFrame == null)
            {
                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = exchangeFrame
                .Securities
                .FirstOrDefault(sec => Equals(sec.Security.Identifiers, mostRecentTrade.Security.Identifiers));

            if (security == null)
            {
                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var threshold = (int)Math.Ceiling(_parameters.HighVolumePercentageDaily.GetValueOrDefault(0) * security.DailyVolume.Traded);

            var breachPercentage = 
                security.DailyVolume.Traded != 0 && tradedVolume != 0
                ? (decimal)tradedVolume / (decimal)security.DailyVolume.Traded
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

        private HighVolumeRuleBreach.BreachDetails WindowVolumeCheck(TradeOrderFrame mostRecentTrade, int tradedVolume)
        {
            if (!MarketHistory.TryGetValue(mostRecentTrade.Market.Id, out var marketStack))
            {
                _logger.LogInformation($"Layering unable to fetch market data frames for {mostRecentTrade.Market.Id} at {UniverseDateTime}.");

                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var securityDataTicks = marketStack
                .ActiveMarketHistory()
                .Where(amh => amh != null)
                .Select(amh =>
                    amh.Securities?.FirstOrDefault(sec =>
                        Equals(sec.Security.Identifiers, mostRecentTrade.Security.Identifiers)))
                .Where(sec => sec != null)
                .ToList();

            var windowVolume = securityDataTicks.Sum(sdt => sdt.Volume.Traded);
            var threshold = (int)Math.Ceiling(_parameters.HighVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

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

        private HighVolumeRuleBreach.BreachDetails MarketCapCheck(TradeOrderFrame mostRecentTrade, List<TradeOrderFrame> trades)
        {
            if (trades == null
                || !trades.Any())
            {
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            if (!LatestExchangeFrameBook.ContainsKey(mostRecentTrade.Market.Id))
            {
                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            LatestExchangeFrameBook.TryGetValue(mostRecentTrade.Market.Id, out var exchangeFrame);

            if (exchangeFrame == null)
            {
                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = exchangeFrame
                .Securities
                .FirstOrDefault(sec => Equals(sec.Security.Identifiers, mostRecentTrade.Security.Identifiers));

            if (security == null)
            {
                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

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
                    .Select(tr => tr.FulfilledVolume * (tr.ExecutedPrice.GetValueOrDefault().Value))
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

            var alert = new UniverseAlertEvent(Domain.Scheduling.Rules.HighVolume, null, _ruleCtx, true);
            _alertStream.Add(alert);

            if (_hadMissingData)
            {
                _ruleCtx.EndEvent().EndEventWithMissingDataError();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }
        }
    }
}
