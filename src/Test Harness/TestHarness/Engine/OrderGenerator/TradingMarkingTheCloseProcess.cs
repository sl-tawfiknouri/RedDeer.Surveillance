﻿using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingMarkingTheCloseProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();
        private readonly IReadOnlyCollection<string> _markingTheCloseTargetSedols;
        private readonly IMarketHistoryStack _marketHistoryStack;
        private readonly ExchangeDto _marketDto;

        private bool _hasProcessedMarkingTheCloseRuleBreaches;
        private DateTime? _executeOn;

        public TradingMarkingTheCloseProcess(
            IReadOnlyCollection<string> markingTheCloseTargetSedols,
            ITradeStrategy<Order> orderStrategy,
            ExchangeDto marketDto,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _markingTheCloseTargetSedols =
                markingTheCloseTargetSedols
                    ?.Where(cts => !string.IsNullOrWhiteSpace(cts))
                    ?.ToList()
                ?? new List<string>();
            _marketHistoryStack = new MarketHistoryStack(TimeSpan.FromMinutes(29));
            _marketDto = marketDto ?? throw new ArgumentNullException(nameof(marketDto));
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(MarketTimeBarCollection value)
        {
            if (value == null)
            {
                return;
            }

            _marketHistoryStack.Add(value, value.Epoch);

            if (_executeOn == null)
            {
                _executeOn = value.Epoch.Date.Add(_marketDto.MarketCloseTime).Add(-TimeSpan.FromMinutes(15));
            }

            if (_hasProcessedMarkingTheCloseRuleBreaches)
            {
                return;
            }

            lock (_lock)
            {
                if (_hasProcessedMarkingTheCloseRuleBreaches)
                {
                    return;
                }

                if (value.Epoch < _executeOn.Value)
                {
                    return;
                }
                
                _marketHistoryStack.ArchiveExpiredActiveItems(value.Epoch);
                var activeItems = _marketHistoryStack.ActiveMarketHistory();

                var i = 0;
                foreach (var sedol in _markingTheCloseTargetSedols)
                {
                    switch (i)
                    {
                        case 0:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, 0.2m);
                            break;
                        case 1:
                            CreateMarkingTheCloseTradesForDailyBreachInSedol(sedol, activeItems, 0.4m);
                            break;
                        case 2:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, 0.15m);
                            break;
                        case 3:
                            CreateMarkingTheCloseTradesForDailyBreachInSedol(sedol, activeItems, 0.3m);
                            break;
                        case 4:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, 0.1m);
                            break;
                        case 5:
                            CreateMarkingTheCloseTradesForDailyBreachInSedol(sedol, activeItems, 0.05m);
                            break;
                    }
                    i++;
                }
                _hasProcessedMarkingTheCloseRuleBreaches = true;
            }
        }

        private void CreateMarkingTheCloseTradesForWindowBreachInSedol(
            string sedol,
            Stack<MarketTimeBarCollection> frames,
            decimal percentageOfTraded)
        {
            if (string.IsNullOrWhiteSpace(sedol))
            {
                return;
            }

            var securities =
                frames
                .SelectMany(frame =>
                    frame.Securities.Where(sec =>
                        string.Equals(
                            sec?.Security.Identifiers.Sedol,
                            sedol,
                            StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            if (!securities.Any())
            {
                return;
            }
            // _executeOn
            var tradedVolume = securities.Sum(sec => sec.Volume.Traded);
            var headSecurity = securities.FirstOrDefault();
            var volumeForBreachesToTrade = (((decimal)tradedVolume * percentageOfTraded) + 1) * 0.18m;

            for (var i = 0; i < 6; i++)
            {
                var timeOffset = _marketDto.MarketCloseTime.Add(-TimeSpan.FromMinutes((2 * i) + 1));
                var tradeTime = headSecurity.TimeStamp.Date.Add(timeOffset);

                var volume = new Order(
                    headSecurity.Security,
                    headSecurity.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    tradeTime,
                    tradeTime,
                    null,
                    null,
                    null,
                    tradeTime,
                    OrderTypes.MARKET,
                    OrderPositions.BUY,
                    headSecurity.Spread.Price.Currency,
                    new CurrencyAmount(headSecurity.Spread.Price.Value, headSecurity.Spread.Price.Currency),
                    new CurrencyAmount(headSecurity.Spread.Price.Value, headSecurity.Spread.Price.Currency),
                    (int) volumeForBreachesToTrade,
                    (int) volumeForBreachesToTrade,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new Trade[0]);

                TradeStream.Add(volume);
            }
        }

        private void CreateMarkingTheCloseTradesForDailyBreachInSedol(
            string sedol,
            Stack<MarketTimeBarCollection> frames,
            decimal percentageOfTraded)
        {
            if (string.IsNullOrWhiteSpace(sedol))
            {
                return;
            }

            var securities =
                frames
                    .OrderByDescending(i => i.Epoch)
                    .FirstOrDefault()
                    .Securities.FirstOrDefault(sec =>
                        string.Equals(
                            sec?.Security.Identifiers.Sedol,
                            sedol,
                            StringComparison.InvariantCultureIgnoreCase));

            var totalVolumeInWindow =
                frames.Sum(io => io.Securities.FirstOrDefault(sec =>
                     string.Equals(
                         sec?.Security.Identifiers.Sedol,
                         sedol,
                         StringComparison.InvariantCultureIgnoreCase))?.Volume.Traded ?? 0);

            if (securities == null)
            {
                return;
            }

            var tradedVolume = securities.DailyVolume.Traded;
            var volumeForBreachesToTrade = (((decimal) tradedVolume * percentageOfTraded) + 1);

            var adjustedVolumeForBreachesToTrade =
                volumeForBreachesToTrade > totalVolumeInWindow
                    ? totalVolumeInWindow
                    : volumeForBreachesToTrade;

            var finalVolumeForBreachestoTrade = adjustedVolumeForBreachesToTrade * 0.18m;

            for (var i = 0; i < 6; i++)
            {
                var timeOffset = _marketDto.MarketCloseTime.Add(-TimeSpan.FromMinutes((2 * i) + 1));
                var tradeTime = securities.TimeStamp.Date.Add(timeOffset);

                var volume = new Order(
                    securities.Security,
                    securities.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    tradeTime,
                    tradeTime,
                    null,
                    null,
                    null,
                    tradeTime,
                    OrderTypes.MARKET,
                    OrderPositions.BUY,
                    securities.Spread.Price.Currency,
                    new CurrencyAmount(securities.Spread.Price.Value, securities.Spread.Price.Currency),
                    new CurrencyAmount(securities.Spread.Price.Value, securities.Spread.Price.Currency),
                    (int)finalVolumeForBreachestoTrade,
                    (int)finalVolumeForBreachestoTrade,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new Trade[0]);

                TradeStream.Add(volume);
            }
        }

        protected override void _TerminateTradingStrategy()
        { }

    }
}
