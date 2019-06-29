using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Markets;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Trades
{
    public class TradingHistoryDelayedDecorator : ITradingHistoryDelayedStack
    {
        private readonly ITradingHistoryStack _stack;
        private readonly TimeSpan _delay;
        private readonly Queue<DelayedOrder> _delayedOrders;

        public TradingHistoryDelayedDecorator(ITradingHistoryStack stack, TimeSpan delay)
        {
            _stack = stack ?? throw new ArgumentNullException(nameof(stack));
            _delay = delay;
            _delayedOrders = new Queue<DelayedOrder>();
        }

        public Stack<Order> ActiveTradeHistory()
        {
            return _stack.ActiveTradeHistory();
        }

        public void Add(Order order, DateTime currentTime)
        {
            var delay = currentTime + _delay;
            var delayedOrder = new DelayedOrder(order, delay);
            _delayedOrders.Enqueue(delayedOrder);

            MoveDelayedOrdersToHistory(currentTime);
        }

        public void ArchiveExpiredActiveItems(DateTime currentTime)
        {
            MoveDelayedOrdersToHistory(currentTime);

            var delayedTime = currentTime - _delay;
            _stack.ArchiveExpiredActiveItems(delayedTime);
        }

        private void MoveDelayedOrdersToHistory(DateTime currentTime)
        {
            while (_delayedOrders.Any() && _delayedOrders.Peek().Dequeue <= currentTime)
            {
                var orderToTransfer = _delayedOrders.Dequeue();
                _stack.Add(orderToTransfer.Order, orderToTransfer.Dequeue);
            }
        }

        public Market Exchange()
        {
            return _stack.Exchange();
        }

        private class DelayedOrder
        {
            public DelayedOrder(Order order, DateTime dequeue)
            {
                Order = order;
                Dequeue = dequeue;
            }

            public Order Order { get; }
            public DateTime Dequeue { get; }
        }
    }
}
