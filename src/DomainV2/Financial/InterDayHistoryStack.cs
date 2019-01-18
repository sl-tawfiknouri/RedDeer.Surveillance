using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial.Interfaces;

namespace DomainV2.Financial
{
    public class InterDayHistoryStack : IInterDayHistoryStack
    {
        private readonly Stack<EquityInterDayTimeBarCollection> _activeStack;
        private readonly Queue<EquityInterDayTimeBarCollection> _history;
        private Market _market;

        private readonly object _lock = new object();
        private readonly TimeSpan _activeTradeDuration;

        public InterDayHistoryStack(TimeSpan activeTradeDuration)
        {
            _activeStack = new Stack<EquityInterDayTimeBarCollection>();
            _history = new Queue<EquityInterDayTimeBarCollection>();
            _activeTradeDuration = activeTradeDuration;
        }

        public void Add(EquityInterDayTimeBarCollection frame, DateTime currentTime)
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
                var counterPartyStack = new Stack<EquityInterDayTimeBarCollection>();

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
        public Stack<EquityInterDayTimeBarCollection> ActiveMarketHistory()
        {
            lock (_lock)
            {
                var tradeStackCopy = new Stack<EquityInterDayTimeBarCollection>(_activeStack);
                var reverseCopyOfTradeStack = new Stack<EquityInterDayTimeBarCollection>(tradeStackCopy);

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
