using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Stub;
using Surveillance.Rules.Marking_The_Close.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Marking_The_Close
{
    public class MarkingTheCloseRule : BaseUniverseRule, IMarkingTheCloseRule
    {
        private readonly IMarkingTheCloseParameters _parameters;
        private readonly IMarkingTheCloseMessageSender _messageSender;
        private readonly ILogger _logger;
        private volatile bool _processingMarketClose;
        private MarketOpenClose _latestMarketClosure;

        public MarkingTheCloseRule(
            IMarkingTheCloseParameters parameters,
            IMarkingTheCloseMessageSender messageSender,
            ILogger<MarkingTheCloseRule> logger)
            : base(
                parameters?.Window ?? TimeSpan.FromMinutes(30),
                Domain.Scheduling.Rules.MarkingTheClose,
                "V1.0",
                "Marking The Close",
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
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

            var tradedSecurity = LatestExchangeFrame
                 ?.Securities
                 ?.FirstOrDefault(sec => Equals(sec.Security.Identifiers, securities.FirstOrDefault()?.Security.Identifiers));

            if (tradedSecurity == null)
            {
                return;
            }

            if (_parameters.PercentageThresholdDailyVolume != null)
            {
                CheckDailyVolumeTraded(securities, tradedSecurity);
            }
        }

        private void CheckDailyVolumeTraded(
            Stack<TradeOrderFrame> securities,
            SecurityTick tradedSecurity)
        {
            if (securities == null
                || !securities.Any()
                || tradedSecurity == null)
            {
                return;
            }

            var volumeTradedBuy =
                securities
                    .Where(sec => sec.Position == OrderPosition.BuyLong)
                    .Sum(sec => sec.Volume);

            var volumeTradedSell =
                securities
                    .Where(sec => sec.Position == OrderPosition.SellLong)
                    .Sum(sec => sec.Volume);

            var thresholdVolumeTraded = tradedSecurity.DailyVolume.Traded * _parameters.PercentageThresholdDailyVolume;

            var hasBuyDailyVolumeBreach = volumeTradedBuy >= thresholdVolumeTraded;
            var buyDailyPercentageBreach = CalculateBuyBreach(tradedSecurity, volumeTradedBuy, hasBuyDailyVolumeBreach);

            var hasSellDailyVolumeBreach = volumeTradedSell >= thresholdVolumeTraded;
            var sellDailyPercentageBreach = CalculateSellBreach(tradedSecurity, volumeTradedSell, hasSellDailyVolumeBreach);

            if (hasSellDailyVolumeBreach
                || hasBuyDailyVolumeBreach)
            {
                var position = new TradePosition(securities.ToList(), null, null, null);
                var breach = new MarkingTheCloseBreach(
                    _parameters.Window,
                    tradedSecurity.Security,
                    _latestMarketClosure,
                    position,
                    _parameters,
                    hasSellDailyVolumeBreach,
                    sellDailyPercentageBreach,
                    hasBuyDailyVolumeBreach,
                    buyDailyPercentageBreach);

                _messageSender.Send(breach);
            }
        }

        private static decimal? CalculateBuyBreach(SecurityTick tradedSecurity, int volumeTradedBuy, bool hasBuyDailyVolumeBreach)
        {
            return hasBuyDailyVolumeBreach
                && volumeTradedBuy > 0
                && tradedSecurity.DailyVolume.Traded > 0
                // ReSharper disable RedundantCast
                ? (decimal?)((decimal)volumeTradedBuy / (decimal)tradedSecurity.DailyVolume.Traded)
                // ReSharper restore RedundantCast
                    : null;
        }

        private static decimal? CalculateSellBreach(SecurityTick tradedSecurity, int volumeTradedSell, bool hasSellDailyVolumeBreach)
        {
            return hasSellDailyVolumeBreach
                   && volumeTradedSell > 0
                   && tradedSecurity.DailyVolume.Traded > 0
                // ReSharper disable RedundantCast
                ? (decimal?)((decimal)volumeTradedSell / (decimal)tradedSecurity.DailyVolume.Traded)
                // ReSharper restore RedundantCast
                : null;
        }

        protected override void Genesis()
        {
            _logger.LogDebug("Genesis occurred in the Marking The Close Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Market Open ({exchange?.MarketId}) occurred in Marking The Close Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Market Close ({exchange?.MarketId}) occurred in Marking The Close Rule at {exchange?.MarketClose}");

            _processingMarketClose = true;
            _latestMarketClosure = exchange;
            RunRuleForAllTradingHistories(exchange?.MarketClose);
            _processingMarketClose = false;
        }

        protected override void EndOfUniverse()
        {
            _logger.LogDebug("Eschaton occured in Marking The Close Rule");
        }
    }
}
