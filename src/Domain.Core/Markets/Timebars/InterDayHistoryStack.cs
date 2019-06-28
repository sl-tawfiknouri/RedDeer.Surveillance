using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Interfaces;

namespace Domain.Core.Markets.Timebars
{
    public class InterDayHistoryStack : IInterDayHistoryStack
    {
        private readonly Stack<EquityInterDayTimeBarCollection> _activeStack;
        private Market _market;

        private readonly object _lock = new object();

        public InterDayHistoryStack()
        {
            _activeStack = new Stack<EquityInterDayTimeBarCollection>();
        }

        public void Add(EquityInterDayTimeBarCollection frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Date == (frame.Epoch.Date))
                {
                    _activeStack.Push(frame);
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
                    if (currentTime.Date == poppedItem.Epoch.Date)
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
            if (!_activeStack?.Any() ?? true)
            {
                return null;
            }

            // ReSharper disable once InconsistentlySynchronizedField
            _market = _activeStack?.Peek()?.Exchange;

            return _market;
        }
    }
}
