using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.MarkingTheClose
{
    public class MarkingTheCloseRule : BaseUniverseRule, IMarkingTheCloseRule
    {
        private readonly IMarkingTheCloseParameters _parameters;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly ILogger _logger;
        private volatile bool _processingMarketClose;
        private MarketOpenClose _latestMarketClosure;
        private bool _hadMissingData = false;

        public MarkingTheCloseRule(
            IMarkingTheCloseParameters parameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            ILogger<MarkingTheCloseRule> logger)
            : base(
                parameters?.Window ?? TimeSpan.FromMinutes(30),
                DomainV2.Scheduling.Rules.MarkingTheClose,
                MarkingTheCloseRuleFactory.Version,
                "Marking The Close",
                ruleCtx,
                factory,
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            if (!_processingMarketClose
                || _latestMarketClosure == null)
            {
                return;
            }

            history.ArchiveExpiredActiveItems(_latestMarketClosure.MarketClose);

            var securities = history.ActiveTradeHistory();
            if (!securities.Any())
            {
                // no securities were being traded within the market closure time window
                return;
            }

            var marketDataRequest = new MarketDataRequest(
                securities.Peek().Market.MarketIdentifierCode,
                securities.Peek().Instrument.Identifiers,
                null,
                null);

            var dataResponse = UniverseMarketCache.Get(marketDataRequest);

            if (dataResponse.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Marking The Close Rule had missing data for {securities.Peek().Instrument.Identifiers} on {UniverseDateTime}");
                return;
            }

            var tradedSecurity = dataResponse.Response;

            VolumeBreach dailyVolumeBreach = null;
            if (_parameters.PercentageThresholdDailyVolume != null)
            {
                dailyVolumeBreach = CheckDailyVolumeTraded(securities, tradedSecurity);
            }

            VolumeBreach windowVolumeBreach = null;
            if (_parameters.PercentageThresholdWindowVolume != null)
            {
                windowVolumeBreach = CheckWindowVolumeTraded(securities, tradedSecurity);
            }

            if ((dailyVolumeBreach == null || !dailyVolumeBreach.HasBreach())
                && (windowVolumeBreach == null || !windowVolumeBreach.HasBreach()))
            {
                return;
            }

            var position = new TradePosition(securities.ToList());
            var breach = new MarkingTheCloseBreach(
                _parameters.Window,
                tradedSecurity.Security,
                _latestMarketClosure,
                position,
                _parameters,
                dailyVolumeBreach ?? new VolumeBreach(),
                windowVolumeBreach ?? new VolumeBreach());

            var alertEvent = new UniverseAlertEvent(DomainV2.Scheduling.Rules.MarkingTheClose, breach, _ruleCtx);
            _alertStream.Add(alertEvent);
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        private VolumeBreach CheckWindowVolumeTraded(
            Stack<Order> securities,
            SecurityTick tradedSecurity)
        {
            var marketDataRequest =
                new MarketDataRequest(
                    tradedSecurity.Market.MarketIdentifierCode,
                    tradedSecurity.Security.Identifiers,
                    null,
                    null);

            var marketResult = UniverseMarketCache.GetMarkets(marketDataRequest);
            if (marketResult.HadMissingData)
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            var securityUpdates = marketResult.Response;
            var securityVolume = securityUpdates.Sum(su => su.Volume.Traded);
            var thresholdVolumeTraded = securityVolume * _parameters.PercentageThresholdWindowVolume;

            if (thresholdVolumeTraded == null)
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            var result =
                CalculateVolumeBreaches(
                    securities, 
                    tradedSecurity,
                    thresholdVolumeTraded.GetValueOrDefault(0),
                    securityVolume);

            return result;
        }

        private VolumeBreach CheckDailyVolumeTraded(
            Stack<Order> securities,
            SecurityTick tradedSecurity)
        {
            var thresholdVolumeTraded = tradedSecurity.DailyVolume.Traded * _parameters.PercentageThresholdDailyVolume;

            if (thresholdVolumeTraded == null)
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            var result =
                CalculateVolumeBreaches(
                    securities,
                    tradedSecurity,
                    thresholdVolumeTraded.GetValueOrDefault(0),
                    tradedSecurity.DailyVolume.Traded);

            return result;
        }

        private VolumeBreach CalculateVolumeBreaches(
            Stack<Order> securities,
            SecurityTick tradedSecurity,
            decimal thresholdVolumeTraded,
            long marketVolumeTraded)
        {
            if (securities == null
                || !securities.Any()
                || tradedSecurity == null)
            {
                return new VolumeBreach();
            }

            var volumeTradedBuy =
                securities
                    .Where(sec => 
                        sec.OrderPosition == OrderPositions.BUY
                        || sec.OrderPosition == OrderPositions.SHORT)
                    .Sum(sec => sec.OrderFilledVolume.GetValueOrDefault(0));

            var volumeTradedSell =
                securities
                    .Where(sec => 
                        sec.OrderPosition == OrderPositions.SELL
                        || sec.OrderPosition == OrderPositions.COVER)
                    .Sum(sec => sec.OrderFilledVolume.GetValueOrDefault(0));

            var hasBuyDailyVolumeBreach = volumeTradedBuy >= thresholdVolumeTraded;
            var buyDailyPercentageBreach = CalculateBuyBreach(volumeTradedBuy, marketVolumeTraded, hasBuyDailyVolumeBreach);

            var hasSellDailyVolumeBreach = volumeTradedSell >= thresholdVolumeTraded;
            var sellDailyPercentageBreach = CalculateSellBreach(volumeTradedSell, marketVolumeTraded, hasSellDailyVolumeBreach);

            if (!hasSellDailyVolumeBreach
                && !hasBuyDailyVolumeBreach)
            {
                return new VolumeBreach();
            }

            return new VolumeBreach
            {
                BuyVolumeBreach = buyDailyPercentageBreach,
                HasBuyVolumeBreach = hasBuyDailyVolumeBreach,
                SellVolumeBreach = sellDailyPercentageBreach,
                HasSellVolumeBreach = hasSellDailyVolumeBreach
            };
        }

        private decimal? CalculateBuyBreach(long volumeTradedBuy, long marketVolume, bool hasBuyVolumeBreach)
        {
            return hasBuyVolumeBreach
                && volumeTradedBuy > 0
                && marketVolume > 0
                // ReSharper disable RedundantCast
                ? (decimal?)((decimal)volumeTradedBuy / (decimal)marketVolume)
                // ReSharper restore RedundantCast
                    : null;
        }

        private decimal? CalculateSellBreach(long volumeTradedSell, long marketVolume, bool hasSellDailyVolumeBreach)
        {
            return hasSellDailyVolumeBreach
                   && volumeTradedSell > 0
                   && marketVolume > 0
                // ReSharper disable RedundantCast
                ? (decimal?)((decimal)volumeTradedSell / (decimal)marketVolume)
                // ReSharper restore RedundantCast
                : null;
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred in the Marking The Close Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred in Marking The Close Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred in Marking The Close Rule at {exchange?.MarketClose}");

            _processingMarketClose = true;
            _latestMarketClosure = exchange;
            RunRuleForAllTradingHistoriesInMarket(exchange, exchange?.MarketClose);
            _processingMarketClose = false;
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured in Marking The Close Rule");
            var opCtx = _ruleCtx?.EndEvent();

            if (_hadMissingData)
            {
                opCtx?.EndEventWithMissingDataError();
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
