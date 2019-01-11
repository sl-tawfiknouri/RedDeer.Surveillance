using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial.Interfaces;

namespace DomainV2.Financial
{
    public class MarketHistoryStack : IMarketHistoryStack
    {
        private readonly Stack<MarketTimeBarCollection> _activeStack;
        private readonly Queue<MarketTimeBarCollection> _history;
        private Market _market;

        private readonly object _lock = new object();
        private readonly TimeSpan _activeTradeDuration;

        public MarketHistoryStack(TimeSpan activeTradeDuration)
        {
            _activeStack = new Stack<MarketTimeBarCollection>();
            _history = new Queue<MarketTimeBarCollection>();
            _activeTradeDuration = activeTradeDuration;
        }

        public void Add(MarketTimeBarCollection frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract(frame.Epoch) <= _activeTradeDuration)
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
                var counterPartyStack = new Stack<MarketTimeBarCollection>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = _activeStack.Pop();
                    if (currentTime.Subtract(poppedItem.Epoch) > _activeTradeDuration)
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
        public Stack<MarketTimeBarCollection> ActiveMarketHistory()
        {
            lock (_lock)
            {
                var tradeStackCopy = new Stack<MarketTimeBarCollection>(_activeStack);
                var reverseCopyOfTradeStack = new Stack<MarketTimeBarCollection>(tradeStackCopy);

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
            _market = _activeStack?.Peek()?.Exchange
                ?? _history?.FirstOrDefault()?.Exchange;

            return _market;
        }
    }
}
