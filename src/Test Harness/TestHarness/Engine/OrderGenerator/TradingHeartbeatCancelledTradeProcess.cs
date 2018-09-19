using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.Frames;
using Domain.Market;
using Domain.Trades.Orders;
using MathNet.Numerics.Distributions;
using NLog;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Generate a bunch of cancelled trades that are either
    /// over 1 million in value or over 80% cancelled
    /// </summary>
    public class TradingHeartbeatCancelledTradeProcess : BaseTradingProcess, IOrderDataGenerator
    {
        private readonly IPulsatingHeartbeat _heartbeat;
        private readonly object _lock = new object();
        private volatile bool _initiated;
        private ExchangeFrame _lastFrame;

        private int valueOfCancelledTradeRatioThreshold = 200000;
        private int valueOfCancelledTradeThreshold = 1000000;
        private int valueOfSingularCancelledTradeThreshold = 5000000;
        private decimal cancellationThresholdPercentage = 0.5m;

        public TradingHeartbeatCancelledTradeProcess(
            ILogger logger,
            ITradeStrategy<TradeOrderFrame> orderStrategy,
            IPulsatingHeartbeat heartbeat) 
            : base(logger, orderStrategy)
        {
            _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
        }

        public override void OnNext(ExchangeFrame value)
        {
            lock (_lock)
            {
                if (!_initiated)
                {
                    _heartbeat.OnBeat(TradeOnHeartbeat);
                    _initiated = true;
                }

                if (value == null)
                {
                    return;
                }

                _lastFrame = value;
            }
        }

        protected override void _InitiateTrading()
        {
            _heartbeat.Start();
        }

        protected override void _TerminateTradingStrategy()
        {
            _heartbeat.Stop();
        }

        private void TradeOnHeartbeat(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (_lastFrame == null
                    || !_lastFrame.Securities.Any())
                {
                    return;
                }

                var selectSecurityToCancelTradesFor = DiscreteUniform.Sample(0, _lastFrame.Securities.Count - 1);
                var cancellationSecurity = _lastFrame.Securities.Skip(selectSecurityToCancelTradesFor).FirstOrDefault();
                var cancellationOrderTotal = DiscreteUniform.Sample(1, 20);
                var cancellationOrderTactic = DiscreteUniform.Sample(0, 2);

                var orders = new TradeOrderFrame[0];
                switch (cancellationOrderTactic)
                {
                    case 0:
                        orders = SingularCancelledOrder(cancellationSecurity, _lastFrame.Exchange);
                        break;
                    case 1:
                        orders = CancellationOrdersByValue(cancellationSecurity, cancellationOrderTotal);
                        break;
                    case 2:
                        orders = CancellationOrdersByRatio(cancellationSecurity, cancellationOrderTotal);
                        break;
                }

                foreach (var item in orders)
                {
                    _tradeStream.Add(item);
                }
            }
        }
        
        private TradeOrderFrame[] CancellationOrdersByValue(SecurityTick security, int totalOrders)
        {
            if (totalOrders == 0
                || security == null)
            {
                return new TradeOrderFrame[0];
            }

            var ordersToCancel = DiscreteUniform.Sample(1, totalOrders);
            var ordersToFulfill = totalOrders - ordersToCancel;
            var minimumPerOrderValue = valueOfCancelledTradeThreshold * (1 / ordersToCancel);

            var orders = new List<TradeOrderFrame>();

            for (var x = 0; x < ordersToCancel; x++)
            {
                orders.Add(OrderForValue(OrderStatus.Cancelled, minimumPerOrderValue, security, _lastFrame.Exchange));
            }

            for (var x = 0; x < ordersToFulfill; x++)
            {
                var fulfilledOrderValue = DiscreteUniform.Sample(0, minimumPerOrderValue * 3);
                orders.Add(OrderForValue(OrderStatus.Fulfilled, fulfilledOrderValue, security, _lastFrame.Exchange));
            }

            return orders.ToArray();
        }

        private TradeOrderFrame[] CancellationOrdersByRatio(SecurityTick security, int totalOrders)
        {
            if (totalOrders == 0
                || security == null)
            {
                return new TradeOrderFrame[0];
            }

            var ordersToCancel = Math.Min((int)(totalOrders * cancellationThresholdPercentage) + 1, totalOrders);
            var ordersToFulfill = totalOrders - ordersToCancel;
            var minimumPerOrderValue = valueOfCancelledTradeRatioThreshold * (1 / ordersToCancel);

            var orders = new List<TradeOrderFrame>();

            for (var x = 0; x < ordersToCancel; x++)
            {
                orders.Add(OrderForValue(OrderStatus.Cancelled, minimumPerOrderValue, security, _lastFrame.Exchange));
            }

            for (var x = 0; x < ordersToFulfill; x++)
            {
                var fulfilledOrderValue = DiscreteUniform.Sample(0, minimumPerOrderValue);
                orders.Add(OrderForValue(OrderStatus.Placed, fulfilledOrderValue, security, _lastFrame.Exchange));
            }

            return orders.ToArray();
        }

        private TradeOrderFrame[] SingularCancelledOrder(SecurityTick security, StockExchange exchange)
        {
            var cancelledTradeOrderValue = DiscreteUniform.Sample(valueOfSingularCancelledTradeThreshold, 10000000);
            var order = OrderForValue(OrderStatus.Cancelled, cancelledTradeOrderValue, security, exchange);

            return new[] { order };
        }

        private TradeOrderFrame OrderForValue(OrderStatus status, decimal value, SecurityTick security, StockExchange exchange)
        {
            var volume = (int)((value / security.Spread.Ask.Value) + 1);
            var orderPosition = (OrderPosition)DiscreteUniform.Sample(0, 3);

            var order =
                new TradeOrderFrame(
                    OrderType.Market,
                    exchange,
                    security.Security,
                    null,
                    volume,
                    orderPosition,
                    status,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    "Trader-1",
                    string.Empty,
                    "Broker-1",
                    "Broker-2");

            return order;
        }
    }
}