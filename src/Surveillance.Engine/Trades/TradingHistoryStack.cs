using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial.Markets;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Trades
{
    public class TradingHistoryStack : ITradingHistoryStack
    {
        private readonly Stack<Order> _activeStack;
        private readonly Queue<Order> _history;
        private Market _market;

        private readonly object _lock = new object();
        private readonly TimeSpan _activeTradeDuration;
        private readonly Func<Order, DateTime> _getFrameTime;
        private readonly ILogger<TradingHistoryStack> _logger;

        public TradingHistoryStack(
            TimeSpan activeTradeDuration,
            Func<Order, DateTime> getFrameTime,
            ILogger<TradingHistoryStack> logger)
        {
            _activeStack = new Stack<Order>();
            _history = new Queue<Order>();
            _activeTradeDuration = activeTradeDuration;
            _getFrameTime = getFrameTime;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Add(Order order, DateTime currentTime)
        {
            if (order == null)
            {
                _logger.LogInformation($"TradingHistoryStack add received a null frame at {currentTime}");
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract(_getFrameTime(order)) <= _activeTradeDuration)
                {
                    _logger.LogTrace($"TradingHistoryStack adding reddeer-order-id {order?.ReddeerOrderId} at {currentTime}");
                    _activeStack.Push(order);
                }
                else
                {
                    _logger.LogTrace($"TradingHistoryStack adding reddeer-order-id {order?.ReddeerOrderId} at {currentTime}. Found it was outdated for an active trade duration of {_activeTradeDuration} so adding it to history");
                    _history.Enqueue(order);
                }
            }
        }

        public void ArchiveExpiredActiveItems(DateTime currentTime)
        {
            lock (_lock)
            {
                var initialActiveStackCount = _activeStack.Count;
                var counterPartyStack = new Stack<Order>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = _activeStack.Pop();

                    if (currentTime.Subtract(_getFrameTime(poppedItem)) > _activeTradeDuration)
                    {
                        _logger.LogTrace($"TradingHistoryStack archiving for {currentTime} and duration of {_activeTradeDuration}. Order with reddeer-order-id of {poppedItem.ReddeerOrderId} archived.");

                        _history.Enqueue(poppedItem);
                    }
                    else
                    {
                        counterPartyStack.Push(poppedItem);
                    }

                    initialActiveStackCount--;
                }

                var counterPartyStackCount = counterPartyStack.Count;

                while (counterPartyStackCount > 0)
                {
                    var replayItem = counterPartyStack.Pop();
                    _activeStack.Push(replayItem);

                    counterPartyStackCount--;
                }
            }
        }

        /// <summary>
        /// Does not provide access to the underlying collection via reference
        /// Instead it returns a new list with the same underlying elements
        /// </summary>
        public Stack<Order> ActiveTradeHistory()
        {
            lock (_lock)
            {
                var tradeStackCopy = new Stack<Order>(_activeStack);
                var reverseCopyOfTradeStack = new Stack<Order>(tradeStackCopy);

                // copy twice in order to restore initial order of elements
                return reverseCopyOfTradeStack;
            }
        }

        public Market Exchange()
        {
            if (_market != null)
            {
                return _market;
            }
            
            // ReSharper disable once InconsistentlySynchronizedField
            _market = _activeStack?.Peek()?.Market
                ?? _history?.FirstOrDefault()?.Market;

            return _market;
        }
    }
}
