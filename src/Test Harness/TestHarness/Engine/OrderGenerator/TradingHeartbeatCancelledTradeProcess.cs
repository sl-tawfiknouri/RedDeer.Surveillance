using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.TimeBars;
using Domain.Financial;
using Domain.Trading;
using MathNet.Numerics.Distributions;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Generate a bunch of cancelled trades that are either
    /// over 1 million in value or over 50% cancelled by either trade number or volume
    /// </summary>
    public class TradingHeartbeatCancelledTradeProcess : BaseTradingProcess
    {
        private readonly IPulsatingHeartbeat _heartbeat;
        private readonly object _lock = new object();
        private volatile bool _initiated;
        private EquityIntraDayTimeBarCollection _lastFrame;

        private decimal _cancellationOfPositionVolumeThresholdPercentage = 0.8m; // % of position cancelled aggregated over all trades in a given direction
        private decimal _cancellationOfOrdersSubmittedThresholdPercentage = 0.8m; // of total orders
        private int _valueOfCancelledTradeRatioThreshold = 200000; // currency value for ratio cancellations
        private int _valueOfCancelledTradeThreshold = 1000000; // currency value, aggregate over many trades
        private int _valueOfSingularCancelledTradeThreshold = 5000000; // currency value

        public TradingHeartbeatCancelledTradeProcess(
            ILogger logger,
            ITradeStrategy<Order> orderStrategy,
            IPulsatingHeartbeat heartbeat) 
            : base(logger, orderStrategy)
        {
            _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
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
                var cancellationOrderTotal = DiscreteUniform.Sample(3, 20);
                var cancellationOrderTactic = DiscreteUniform.Sample(1, 3);
                var orders = new Order[0];
                orders = SetCancellationOrder(cancellationSecurity, cancellationOrderTotal, cancellationOrderTactic, orders);

                foreach (var item in orders)
                {
                    TradeStream.Add(item);
                }
            }
        }

        private Order[] SetCancellationOrder(EquityInstrumentIntraDayTimeBar cancellationSecurity, int cancellationOrderTotal, int cancellationOrderTactic, Order[] orders)
        {
            switch (cancellationOrderTactic)
            {
                case 0:
                    // cannot be hit due to discrete uniform spread as we're not currently coding for detecting it on the surveillance service
                    orders = SingularCancelledOrder(cancellationSecurity, _lastFrame.Exchange);
                    break;
                case 1:
                    orders = CancellationOrdersByValue(cancellationSecurity, cancellationOrderTotal);
                    break;
                case 2:
                    orders = CancellationOrdersByRatio(cancellationSecurity, cancellationOrderTotal);
                    break;
                case 3:
                    orders = CancellationOrdersByPercentOfVolume(cancellationSecurity, cancellationOrderTotal);
                    break;
            }

            return orders;
        }

        private Order[] CancellationOrdersByValue(EquityInstrumentIntraDayTimeBar security, int totalOrders)
        {
            if (totalOrders == 0
                || security == null)
            {
                return new Order[0];
            }

            var ordersToCancel = DiscreteUniform.Sample(1, totalOrders);
            var ordersToFulfill = totalOrders - ordersToCancel;
            // ReSharper disable RedundantCast
            var minimumPerOrderValue = (int)((decimal)_valueOfCancelledTradeThreshold * ((decimal)(1m / ordersToCancel)) + 1);
            // ReSharper restore RedundantCast

            var orders = new List<Order>();

            for (var x = 0; x < ordersToCancel; x++)
            {
                orders.Add(OrderForValue(OrderStatus.Cancelled, minimumPerOrderValue, security, _lastFrame.Exchange));
            }

            for (var x = 0; x < ordersToFulfill; x++)
            {
                var fulfilledOrderValue = DiscreteUniform.Sample(0, minimumPerOrderValue * 3);
                orders.Add(OrderForValue(OrderStatus.Filled, fulfilledOrderValue, security, _lastFrame.Exchange));
            }

            return orders.ToArray();
        }

        private Order[] CancellationOrdersByRatio(EquityInstrumentIntraDayTimeBar security, int totalOrders)
        {
            if (totalOrders == 0
                || security == null)
            {
                return new Order[0];
            }

            var ordersToCancel = Math.Min((int)(totalOrders * _cancellationOfOrdersSubmittedThresholdPercentage) + 1, totalOrders);
            var ordersToFulfill = totalOrders - ordersToCancel;
            // ReSharper disable RedundantCast
            var minimumPerOrderValue = (int)((decimal)_valueOfCancelledTradeRatioThreshold * ((decimal)(1m / ordersToCancel)) + 1);
            // ReSharper restore RedundantCast

            var orders = new List<Order>();

            for (var x = 0; x < ordersToCancel; x++)
            {
                orders.Add(OrderForValue(OrderStatus.Cancelled, minimumPerOrderValue, security, _lastFrame.Exchange));
            }

            for (var x = 0; x < ordersToFulfill; x++)
            {
                var fulfilledOrderValue = DiscreteUniform.Sample(0, minimumPerOrderValue);
                orders.Add(OrderForValue(OrderStatus.Booked, fulfilledOrderValue, security, _lastFrame.Exchange));
            }

            return orders.ToArray();
        }

        private Order[] CancellationOrdersByPercentOfVolume(EquityInstrumentIntraDayTimeBar security, int totalOrders)
        {
            if (totalOrders == 0
                || security == null)
            {
                return new Order[0];
            }

            var cancelledOrderPositionSize = 10000000;
            var ordersToCancel = 1;
            var ordersToFulfill = totalOrders - ordersToCancel;
            var minimumPerOrderValue = cancelledOrderPositionSize * _cancellationOfPositionVolumeThresholdPercentage;
            var remainingOrderValue = cancelledOrderPositionSize * (1 - _cancellationOfPositionVolumeThresholdPercentage);
            // ReSharper disable RedundantCast
            var remainingOrderValuePerOrder = (int)((decimal)remainingOrderValue * ((decimal)(1m / ordersToFulfill)) + 1);
            // ReSharper restore RedundantCast

            var orders = new List<Order>();

            for (var x = 0; x < ordersToCancel; x++)
            {
                orders.Add(OrderForValue(OrderStatus.Cancelled, minimumPerOrderValue, security, _lastFrame.Exchange));
            }

            for (var x = 0; x < ordersToFulfill; x++)
            {
                orders.Add(OrderForValue(OrderStatus.Booked, remainingOrderValuePerOrder, security, _lastFrame.Exchange));
            }

            return orders.ToArray();
        }

        private Order[] SingularCancelledOrder(EquityInstrumentIntraDayTimeBar security, Market exchange)
        {
            var cancelledTradeOrderValue = DiscreteUniform.Sample(_valueOfSingularCancelledTradeThreshold, 10000000);
            var order = OrderForValue(OrderStatus.Cancelled, cancelledTradeOrderValue, security, exchange);

            return new[] { order };
        }

        private Order OrderForValue(OrderStatus status, decimal value, EquityInstrumentIntraDayTimeBar security, Market exchange)
        {
            var volume = (int)((value / security.SpreadTimeBar.Ask.Value) + 1);
            var orderPosition = (OrderDirections)DiscreteUniform.Sample(0, 3);

            var cancelledDate = status == OrderStatus.Cancelled ? (DateTime?) DateTime.UtcNow : null;
            var filledDate = status == OrderStatus.Filled ? (DateTime?)DateTime.UtcNow : null;

            var order =
                new Order(
                    security.Security,
                    exchange,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,
                    "order-v1",
                    "order-v1",
                    "order-group-v1",
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    null,
                    null,
                    cancelledDate,
                    filledDate,
                    OrderTypes.MARKET,
                    orderPosition,
                    security.SpreadTimeBar.Price.Currency,
                    security.SpreadTimeBar.Price.Currency,
                    OrderCleanDirty.NONE,
                    null,
                    security.SpreadTimeBar.Price,
                    security.SpreadTimeBar.Price,
                    volume,
                    volume,
                    "trader-1",
                    "trader one",
                    "clearing-agent",
                    "dealing-instructions",
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);
            
            return order;
        }
    }
}