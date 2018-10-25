using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
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
        private readonly IMarkingTheCloseMessageSender _messageSender;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly ILogger _logger;
        private volatile bool _processingMarketClose;
        private MarketOpenClose _latestMarketClosure;
        private int _alertCount;
        private bool _hadMissingData = false;

        public MarkingTheCloseRule(
            IMarkingTheCloseParameters parameters,
            IMarkingTheCloseMessageSender messageSender,
            ISystemProcessOperationRunRuleContext ruleCtx,
            ILogger<MarkingTheCloseRule> logger)
            : base(
                parameters?.Window ?? TimeSpan.FromMinutes(30),
                Domain.Scheduling.Rules.MarkingTheClose,
                Versioner.Version(1, 0),
                "Marking The Close",
                ruleCtx,
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
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

            var marketId = securities.FirstOrDefault()?.Market?.Id;
            if (marketId == null)
            {
                return;
            }

            if (!LatestExchangeFrameBook.ContainsKey(marketId))
            {
                return;
            }

            LatestExchangeFrameBook.TryGetValue(marketId, out var frame);

            var tradedSecurity =
                frame
                    ?.Securities
                    ?.FirstOrDefault(sec => Equals(sec.Security.Identifiers, securities.FirstOrDefault()?.Security.Identifiers));

            if (tradedSecurity == null)
            {
                return;
            }

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

            _alertCount += 1;
            _messageSender.Send(breach);
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        private VolumeBreach CheckWindowVolumeTraded(
            Stack<TradeOrderFrame> securities,
            SecurityTick tradedSecurity)
        {
            if (tradedSecurity.Market?.Id == null 
                || !MarketHistory.ContainsKey(tradedSecurity.Market?.Id))
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            MarketHistory.TryGetValue(tradedSecurity.Market.Id, out var marketHistoryStack);

            if (marketHistoryStack == null)
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            var securityUpdates =
                marketHistoryStack
                .ActiveMarketHistory()
                .SelectMany(amh =>
                    amh.Securities.Where(sec => Equals(sec.Security.Identifiers, tradedSecurity.Security.Identifiers)))
                .Where(secUpdate => secUpdate != null)
                .ToList();

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
            Stack<TradeOrderFrame> securities,
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
            Stack<TradeOrderFrame> securities,
            SecurityTick tradedSecurity,
            decimal thresholdVolumeTraded,
            int marketVolumeTraded)
        {
            if (securities == null
                || !securities.Any()
                || tradedSecurity == null)
            {
                return new VolumeBreach();
            }

            var volumeTradedBuy =
                securities
                    .Where(sec => sec.Position == OrderPosition.Buy)
                    .Sum(sec => sec.FulfilledVolume);

            var volumeTradedSell =
                securities
                    .Where(sec => sec.Position == OrderPosition.Sell)
                    .Sum(sec => sec.FulfilledVolume);

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

        private decimal? CalculateBuyBreach(int volumeTradedBuy, int marketVolume, bool hasBuyVolumeBreach)
        {
            return hasBuyVolumeBreach
                && volumeTradedBuy > 0
                && marketVolume > 0
                // ReSharper disable RedundantCast
                ? (decimal?)((decimal)volumeTradedBuy / (decimal)marketVolume)
                // ReSharper restore RedundantCast
                    : null;
        }

        private decimal? CalculateSellBreach(int volumeTradedSell, int marketVolume, bool hasSellDailyVolumeBreach)
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
            _alertCount = 0;
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
            _ruleCtx.UpdateAlertEvent(_alertCount);
            var opCtx = _ruleCtx?.EndEvent();

            if (_hadMissingData)
            {
                opCtx.EndEventWithMissingDataError();
            }

            _alertCount = 0;
        }
    }
}
