using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Market;
using Domain.Market.Interfaces;
using Domain.Trades.Orders;
using NLog;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingLayeringProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();
        private readonly IReadOnlyCollection<string> _layeringTargetSedols;
        private readonly IMarketHistoryStack _marketHistoryStack;
        private readonly IReadOnlyCollection<DataGenerationPlan> _plan;

        private bool _hasProcessedLayeringBreaches;
        private DateTime? _executeOn;

        public TradingLayeringProcess(
            IReadOnlyCollection<string> layeringTargetSedols,
            IReadOnlyCollection<DataGenerationPlan> plan,
            ITradeStrategy<TradeOrderFrame> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _layeringTargetSedols =
                layeringTargetSedols
                    ?.Where(cts => !string.IsNullOrWhiteSpace(cts))
                    ?.ToList()
                ?? new List<string>();

            _marketHistoryStack = new MarketHistoryStack(TimeSpan.FromHours(1));
            _plan = plan ?? new DataGenerationPlan[0];
        }

        protected override void _InitiateTrading()
        { }

        // i think we still need to maintain the link between exchange frames and the orders
        // to keep prices reflecting the incremental behaviour ...yeah =( -.- o.(0) yeah
        // ok so the assumption is that we'll still receive the exchange frames?
        // sure...in that case the layering can be entirely frame driven...
        public override void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                return;
            }

            if (_plan?.Any() ?? true)
            {
                return;
            }

            _marketHistoryStack.Add(value, value.TimeStamp);

            if (_hasProcessedLayeringBreaches)
            {
                return;
            }

            lock (_lock)
            {
                if (_hasProcessedLayeringBreaches)
                {
                    return;
                }

                if (value.TimeStamp < _executeOn.Value)
                {
                    return;
                }

                _marketHistoryStack.ArchiveExpiredActiveItems(value.TimeStamp);
                var activeItems = _marketHistoryStack.ActiveMarketHistory();

                var i = 0;
                foreach (var sedol in _layeringTargetSedols)
                {
                    switch (i)
                    {
                        case 0:
                            CreateLayeringTradesForWindowBreachInSedol(sedol, activeItems, value, 10);
                            break;
                        case 1:
                            CreateLayeringTradesForWindowBreachInSedol(sedol, activeItems, value, 7);
                            break;
                        case 2:
                            CreateLayeringTradesForWindowBreachInSedol(sedol, activeItems, value, 5);
                            break;
                        case 3:
                            CreateLayeringTradesForWindowBreachInSedol(sedol, activeItems, value, 4);
                            break;
                        case 4:
                            CreateLayeringTradesForWindowBreachInSedol(sedol, activeItems, value, 3);
                            break;
                        case 5:
                            CreateLayeringTradesForWindowBreachInSedol(sedol, activeItems, value, 2);
                            break;
                    }
                    i++;
                }
                _hasProcessedLayeringBreaches = true;
            }
        }

        private void CreateLayeringTradesForWindowBreachInSedol(
            string sedol,
            Stack<ExchangeFrame> frames,
            ExchangeFrame latestFrame,
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
            var tradedVolume = securities.Sum(sec => sec.Volume.Traded);

            // select a suitably low % of the traded volume
            tradedVolume = (int)((decimal)tradedVolume * 0.03m);

            for (var i = 0; i < cancelledTrades; i++)
            {
                var tradeTime = latestFrame.TimeStamp;

                var volumeFrame = new TradeOrderFrame(
                    i == 0 ? OrderType.Market : OrderType.Limit,
                    headSecurity.Market,
                    headSecurity.Security,
                    new Price(headSecurity.Spread.Price.Value, headSecurity.Spread.Price.Currency),
                    new Price(headSecurity.Spread.Price.Value, headSecurity.Spread.Price.Currency),
                    i == 0 ? (int)tradedVolume : 0,
                    (int)tradedVolume,
                    i == 0 ? OrderPosition.Buy : OrderPosition.Sell,
                    i == 0 ? OrderStatus.Fulfilled : OrderStatus.Cancelled,
                    tradeTime.AddSeconds(i),
                    tradeTime.AddSeconds(-i),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    headSecurity.Spread.Price.Currency);

                TradeStream.Add(volumeFrame);
            }
        }

        protected override void _TerminateTradingStrategy()
        { }
    }
}
