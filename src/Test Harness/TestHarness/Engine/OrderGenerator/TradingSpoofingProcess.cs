using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.TimeBars;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingSpoofingProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();
        private readonly IReadOnlyCollection<string> _spoofingTargetSedols;
        private readonly IIntraDayHistoryStack _intraDayHistoryStack;
        private readonly TimeSpan _executePoint = TimeSpan.FromMinutes(65);

        private bool _hasProcessedSpoofingBreaches;
        private DateTime? _executeOn;

        public TradingSpoofingProcess(
            IReadOnlyCollection<string> spoofingTargetSedols,
            ITradeStrategy<Order> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _spoofingTargetSedols =
                spoofingTargetSedols
                    ?.Where(cts => !string.IsNullOrWhiteSpace(cts))
                    ?.ToList()
                ?? new List<string>();

            _intraDayHistoryStack = new IntraDayHistoryStack(TimeSpan.FromHours(1));
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null)
            {
                return;
            }

            _intraDayHistoryStack.Add(value, value.Epoch);

            if (_executeOn == null)
            {
                _executeOn = value.Epoch.Add(_executePoint);
                return;
            }

            if (_hasProcessedSpoofingBreaches)
            {
                return;
            }

            lock (_lock)
            {
                if (_hasProcessedSpoofingBreaches)
                {
                    return;
                }

                if (value.Epoch < _executeOn.Value)
                {
                    return;
                }

                _intraDayHistoryStack.ArchiveExpiredActiveItems(value.Epoch);
                var activeItems = _intraDayHistoryStack.ActiveMarketHistory();

                var i = 0;
                foreach (var sedol in _spoofingTargetSedols)
                {
                    switch (i)
                    {
                        case 0:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, value, 10);
                            break;
                        case 1:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, value, 7);
                            break;
                        case 2:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, value, 5);
                            break;
                        case 3:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, value, 4);
                            break;
                        case 4:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, value, 3);
                            break;
                        case 5:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, value, 2);
                            break;
                    }
                    i++;
                }
                _hasProcessedSpoofingBreaches = true;
            }
        }

        private void CreateMarkingTheCloseTradesForWindowBreachInSedol(
            string sedol,
            Stack<EquityIntraDayTimeBarCollection> frames,
            EquityIntraDayTimeBarCollection latestFrame,
            int cancelledTrades)
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

            var headSecurity =
                latestFrame
                    .Securities
                    .FirstOrDefault(fram => 
                        string.Equals(
                            fram.Security.Identifiers.Sedol,
                            sedol,
                            StringComparison.InvariantCultureIgnoreCase));

            if (!securities.Any())
            {
                return;
            }

            // _executeOn
            var tradedVolume = securities.Sum(sec => sec.SpreadTimeBar.Volume.Traded);

            // select a suitably low % of the traded volume
            tradedVolume = (int)((decimal)tradedVolume * 0.03m);

            for (var i = 0; i < cancelledTrades; i++)
            {
                var tradeTime = latestFrame.Epoch;

                var volumeOrder = new Order(
                    headSecurity.Security,
                    headSecurity.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,
                    "version-1",
                    "version-1",
                    "version-1",
                    tradeTime.AddSeconds(-i),
                    tradeTime.AddSeconds(-i),
                    null,
                    null,
                    i == 0 ? null : (DateTime?) tradeTime.AddSeconds(i),
                    i == 0 ? (DateTime?) tradeTime.AddSeconds(i) : null,
                    i == 0 ? OrderTypes.MARKET : OrderTypes.LIMIT,
                    i == 0 ? OrderDirections.BUY : OrderDirections.SELL,
                    headSecurity.SpreadTimeBar.Price.Currency,
                    headSecurity.SpreadTimeBar.Price.Currency,
                    OrderCleanDirty.NONE,
                    null,
                    new Money(headSecurity.SpreadTimeBar.Price.Value, headSecurity.SpreadTimeBar.Price.Currency),
                    new Money(headSecurity.SpreadTimeBar.Price.Value, headSecurity.SpreadTimeBar.Price.Currency),
                    (int) tradedVolume,
                    i == 0 ? (int) tradedVolume : 0,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

                TradeStream.Add(volumeOrder);
            }
        }

        protected override void _TerminateTradingStrategy()
        { }
    }
}
