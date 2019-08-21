namespace Surveillance.Engine.Rules.Trades
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class TradingHistoryStack : ITradingHistoryStack
    {
        private readonly Stack<Order> _activeStack;

        private readonly TimeSpan _activeTradeDuration;

        private readonly Func<Order, DateTime> _getFrameTime;

        private readonly object _lock = new object();

        private readonly ILogger<TradingHistoryStack> _logger;

        private Market _market;

        public TradingHistoryStack(
            TimeSpan activeTradeDuration,
            Func<Order, DateTime> getFrameTime,
            ILogger<TradingHistoryStack> logger)
        {
            this._activeStack = new Stack<Order>();
            this._activeTradeDuration = activeTradeDuration;
            this._getFrameTime = getFrameTime;

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Does not provide access to the underlying collection via reference
        ///     Instead it returns a new list with the same underlying elements
        /// </summary>
        public Stack<Order> ActiveTradeHistory()
        {
            lock (this._lock)
            {
                var tradeStackCopy = new Stack<Order>(this._activeStack);
                var reverseCopyOfTradeStack = new Stack<Order>(tradeStackCopy);

                // copy twice in order to restore initial order of elements
                return reverseCopyOfTradeStack;
            }
        }

        public void Add(Order order, DateTime currentTime)
        {
            if (order == null)
            {
                this._logger.LogInformation($"add received a null frame at {currentTime}");
                return;
            }

            lock (this._lock)
            {
                if (currentTime.Subtract(this._getFrameTime(order)) <= this._activeTradeDuration)
                {
                    this._logger.LogTrace($"adding reddeer-order-id {order?.ReddeerOrderId} at {currentTime}");
                    this._activeStack.Push(order);
                }
                else
                {
                    this._logger.LogTrace(
                        $"adding reddeer-order-id {order?.ReddeerOrderId} at {currentTime}. Found it was outdated for an active trade duration of {this._activeTradeDuration} so discarding it");
                }
            }
        }

        public void ArchiveExpiredActiveItems(DateTime currentTime)
        {
            lock (this._lock)
            {
                var initialActiveStackCount = this._activeStack.Count;
                var counterPartyStack = new Stack<Order>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = this._activeStack.Pop();

                    if (currentTime.Subtract(this._getFrameTime(poppedItem)) <= this._activeTradeDuration)
                        counterPartyStack.Push(poppedItem);

                    initialActiveStackCount--;
                }

                var counterPartyStackCount = counterPartyStack.Count;

                while (counterPartyStackCount > 0)
                {
                    var replayItem = counterPartyStack.Pop();
                    this._activeStack.Push(replayItem);

                    counterPartyStackCount--;
                }
            }
        }

        public Market Exchange()
        {
            if (this._market != null) return this._market;

            // ReSharper disable once InconsistentlySynchronizedField
            if (!this._activeStack?.Any() ?? true) return null;

            // ReSharper disable once InconsistentlySynchronizedField
            this._market = this._activeStack?.Peek()?.Market;

            return this._market;
        }
    }
}