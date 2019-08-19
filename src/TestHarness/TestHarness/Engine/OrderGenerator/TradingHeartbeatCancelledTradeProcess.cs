namespace TestHarness.Engine.OrderGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;

    using MathNet.Numerics.Distributions;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.Heartbeat.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    /// <summary>
    ///     Generate a bunch of cancelled trades that are either
    ///     over 1 million in value or over 50% cancelled by either trade number or volume
    /// </summary>
    public class TradingHeartbeatCancelledTradeProcess : BaseTradingProcess
    {
        private readonly decimal _cancellationOfOrdersSubmittedThresholdPercentage = 0.8m; // of total orders

        private readonly decimal
            _cancellationOfPositionVolumeThresholdPercentage =
                0.8m; // % of position cancelled aggregated over all trades in a given direction

        private readonly IPulsatingHeartbeat _heartbeat;

        private readonly object _lock = new object();

        private readonly int _valueOfCancelledTradeRatioThreshold = 200000; // currency value for ratio cancellations

        private readonly int _valueOfCancelledTradeThreshold = 1000000; // currency value, aggregate over many trades

        private readonly int _valueOfSingularCancelledTradeThreshold = 5000000; // currency value

        private volatile bool _initiated;

        private EquityIntraDayTimeBarCollection _lastFrame;

        public TradingHeartbeatCancelledTradeProcess(
            ILogger logger,
            ITradeStrategy<Order> orderStrategy,
            IPulsatingHeartbeat heartbeat)
            : base(logger, orderStrategy)
        {
            this._heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            lock (this._lock)
            {
                if (!this._initiated)
                {
                    this._heartbeat.OnBeat(this.TradeOnHeartbeat);
                    this._initiated = true;
                }

                if (value == null) return;

                this._lastFrame = value;
            }
        }

        protected override void _InitiateTrading()
        {
            this._heartbeat.Start();
        }

        protected override void _TerminateTradingStrategy()
        {
            this._heartbeat.Stop();
        }

        private Order[] CancellationOrdersByPercentOfVolume(EquityInstrumentIntraDayTimeBar security, int totalOrders)
        {
            if (totalOrders == 0 || security == null) return new Order[0];

            var cancelledOrderPositionSize = 10000000;
            var ordersToCancel = 1;
            var ordersToFulfill = totalOrders - ordersToCancel;
            var minimumPerOrderValue =
                cancelledOrderPositionSize * this._cancellationOfPositionVolumeThresholdPercentage;
            var remainingOrderValue =
                cancelledOrderPositionSize * (1 - this._cancellationOfPositionVolumeThresholdPercentage);

            // ReSharper disable RedundantCast
            var remainingOrderValuePerOrder = (int)((decimal)remainingOrderValue * (decimal)(1m / ordersToFulfill) + 1);

            // ReSharper restore RedundantCast
            var orders = new List<Order>();

            for (var x = 0; x < ordersToCancel; x++)
                orders.Add(
                    this.OrderForValue(
                        OrderStatus.Cancelled,
                        minimumPerOrderValue,
                        security,
                        this._lastFrame.Exchange));

            for (var x = 0; x < ordersToFulfill; x++)
                orders.Add(
                    this.OrderForValue(
                        OrderStatus.Booked,
                        remainingOrderValuePerOrder,
                        security,
                        this._lastFrame.Exchange));

            return orders.ToArray();
        }

        private Order[] CancellationOrdersByRatio(EquityInstrumentIntraDayTimeBar security, int totalOrders)
        {
            if (totalOrders == 0 || security == null) return new Order[0];

            var ordersToCancel = Math.Min(
                (int)(totalOrders * this._cancellationOfOrdersSubmittedThresholdPercentage) + 1,
                totalOrders);
            var ordersToFulfill = totalOrders - ordersToCancel;

            // ReSharper disable RedundantCast
            var minimumPerOrderValue =
                (int)((decimal)this._valueOfCancelledTradeRatioThreshold * (decimal)(1m / ordersToCancel) + 1);

            // ReSharper restore RedundantCast
            var orders = new List<Order>();

            for (var x = 0; x < ordersToCancel; x++)
                orders.Add(
                    this.OrderForValue(
                        OrderStatus.Cancelled,
                        minimumPerOrderValue,
                        security,
                        this._lastFrame.Exchange));

            for (var x = 0; x < ordersToFulfill; x++)
            {
                var fulfilledOrderValue = DiscreteUniform.Sample(0, minimumPerOrderValue);
                orders.Add(
                    this.OrderForValue(OrderStatus.Booked, fulfilledOrderValue, security, this._lastFrame.Exchange));
            }

            return orders.ToArray();
        }

        private Order[] CancellationOrdersByValue(EquityInstrumentIntraDayTimeBar security, int totalOrders)
        {
            if (totalOrders == 0 || security == null) return new Order[0];

            var ordersToCancel = DiscreteUniform.Sample(1, totalOrders);
            var ordersToFulfill = totalOrders - ordersToCancel;

            // ReSharper disable RedundantCast
            var minimumPerOrderValue =
                (int)((decimal)this._valueOfCancelledTradeThreshold * (decimal)(1m / ordersToCancel) + 1);

            // ReSharper restore RedundantCast
            var orders = new List<Order>();

            for (var x = 0; x < ordersToCancel; x++)
                orders.Add(
                    this.OrderForValue(
                        OrderStatus.Cancelled,
                        minimumPerOrderValue,
                        security,
                        this._lastFrame.Exchange));

            for (var x = 0; x < ordersToFulfill; x++)
            {
                var fulfilledOrderValue = DiscreteUniform.Sample(0, minimumPerOrderValue * 3);
                orders.Add(
                    this.OrderForValue(OrderStatus.Filled, fulfilledOrderValue, security, this._lastFrame.Exchange));
            }

            return orders.ToArray();
        }

        private Order OrderForValue(
            OrderStatus status,
            decimal value,
            EquityInstrumentIntraDayTimeBar security,
            Market exchange)
        {
            var volume = (int)(value / security.SpreadTimeBar.Ask.Value + 1);
            var orderPosition = (OrderDirections)DiscreteUniform.Sample(0, 3);

            var cancelledDate = status == OrderStatus.Cancelled ? (DateTime?)DateTime.UtcNow : null;
            var filledDate = status == OrderStatus.Filled ? (DateTime?)DateTime.UtcNow : null;

            var order = new Order(
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
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.Now, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);

            return order;
        }

        private Order[] SetCancellationOrder(
            EquityInstrumentIntraDayTimeBar cancellationSecurity,
            int cancellationOrderTotal,
            int cancellationOrderTactic,
            Order[] orders)
        {
            switch (cancellationOrderTactic)
            {
                case 0:
                    // cannot be hit due to discrete uniform spread as we're not currently coding for detecting it on the surveillance service
                    orders = this.SingularCancelledOrder(cancellationSecurity, this._lastFrame.Exchange);
                    break;
                case 1:
                    orders = this.CancellationOrdersByValue(cancellationSecurity, cancellationOrderTotal);
                    break;
                case 2:
                    orders = this.CancellationOrdersByRatio(cancellationSecurity, cancellationOrderTotal);
                    break;
                case 3:
                    orders = this.CancellationOrdersByPercentOfVolume(cancellationSecurity, cancellationOrderTotal);
                    break;
            }

            return orders;
        }

        private Order[] SingularCancelledOrder(EquityInstrumentIntraDayTimeBar security, Market exchange)
        {
            var cancelledTradeOrderValue = DiscreteUniform.Sample(
                this._valueOfSingularCancelledTradeThreshold,
                10000000);
            var order = this.OrderForValue(OrderStatus.Cancelled, cancelledTradeOrderValue, security, exchange);

            return new[] { order };
        }

        private void TradeOnHeartbeat(object sender, EventArgs e)
        {
            lock (this._lock)
            {
                if (this._lastFrame == null || !this._lastFrame.Securities.Any()) return;

                var selectSecurityToCancelTradesFor = DiscreteUniform.Sample(0, this._lastFrame.Securities.Count - 1);
                var cancellationSecurity =
                    this._lastFrame.Securities.Skip(selectSecurityToCancelTradesFor).FirstOrDefault();
                var cancellationOrderTotal = DiscreteUniform.Sample(3, 20);
                var cancellationOrderTactic = DiscreteUniform.Sample(1, 3);
                var orders = new Order[0];
                orders = this.SetCancellationOrder(
                    cancellationSecurity,
                    cancellationOrderTotal,
                    cancellationOrderTactic,
                    orders);

                foreach (var item in orders) this.TradeStream.Add(item);
            }
        }
    }
}