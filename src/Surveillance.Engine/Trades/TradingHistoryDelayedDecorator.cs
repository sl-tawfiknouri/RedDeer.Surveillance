namespace Surveillance.Engine.Rules.Trades
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class TradingHistoryDelayedDecorator : ITradingHistoryDelayedStack
    {
        private readonly TimeSpan _delay;

        private readonly Queue<DelayedOrder> _delayedOrders;

        private readonly ITradingHistoryStack _stack;

        public TradingHistoryDelayedDecorator(ITradingHistoryStack stack, TimeSpan delay)
        {
            this._stack = stack ?? throw new ArgumentNullException(nameof(stack));
            this._delay = delay;
            this._delayedOrders = new Queue<DelayedOrder>();
        }

        public Stack<Order> ActiveTradeHistory()
        {
            return this._stack.ActiveTradeHistory();
        }

        public void Add(Order order, DateTime currentTime)
        {
            var delay = currentTime + this._delay;
            var delayedOrder = new DelayedOrder(order, delay);
            this._delayedOrders.Enqueue(delayedOrder);

            this.MoveDelayedOrdersToHistory(currentTime);
        }

        public void ArchiveExpiredActiveItems(DateTime currentTime)
        {
            this.MoveDelayedOrdersToHistory(currentTime);

            var delayedTime = currentTime - this._delay;
            this._stack.ArchiveExpiredActiveItems(delayedTime);
        }

        public Market Exchange()
        {
            return this._stack.Exchange();
        }

        private void MoveDelayedOrdersToHistory(DateTime currentTime)
        {
            while (this._delayedOrders.Any() && this._delayedOrders.Peek().Dequeue <= currentTime)
            {
                var orderToTransfer = this._delayedOrders.Dequeue();
                this._stack.Add(orderToTransfer.Order, orderToTransfer.Dequeue);
            }
        }

        private class DelayedOrder
        {
            public DelayedOrder(Order order, DateTime dequeue)
            {
                this.Order = order;
                this.Dequeue = dequeue;
            }

            public DateTime Dequeue { get; }

            public Order Order { get; }
        }
    }
}