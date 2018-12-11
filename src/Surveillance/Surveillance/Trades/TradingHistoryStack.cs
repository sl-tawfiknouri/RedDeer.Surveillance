using Surveillance.Trades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;

namespace Surveillance.Trades
{
    public class TradingHistoryStack : ITradingHistoryStack
    {
        private readonly Stack<Order> _activeStack;
        private readonly Queue<Order> _history;
        private Market _market;

        private readonly object _lock = new object();
        private readonly TimeSpan _activeTradeDuration;
        private readonly Func<Order, DateTime> _getFrameTime;
        
        public TradingHistoryStack(
            TimeSpan activeTradeDuration,
            Func<Order, DateTime> getFrameTime)
        {
            _activeStack = new Stack<Order>();
            _history = new Queue<Order>();
            _activeTradeDuration = activeTradeDuration;
            _getFrameTime = getFrameTime;
        }

        public void Add(Order frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract(_getFrameTime(frame)) <= _activeTradeDuration)
                {
                    _activeStack.Push(frame);
                }
                else
                {
                    _history.Enqueue(frame);
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
