using System;
using Domain.Core.Financial.Interfaces;

namespace Domain.Core.Financial
{
    public class IntraDayHistoryStack : IIntraDayHistoryStack
    {
        private readonly Stack<EquityIntraDayTimeBarCollection> _activeStack;
        private readonly Queue<EquityIntraDayTimeBarCollection> _history;
        private Market _market;

        private readonly object _lock = new object();
        private readonly TimeSpan _activeTradeDuration;

        public IntraDayHistoryStack(TimeSpan activeTradeDuration)
        {
            _activeStack = new Stack<EquityIntraDayTimeBarCollection>();
            _history = new Queue<EquityIntraDayTimeBarCollection>();
            _activeTradeDuration = activeTradeDuration;
        }

        public void Add(EquityIntraDayTimeBarCollection frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract((DateTime) frame.Epoch) <= _activeTradeDuration)
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
                var counterPartyStack = new Stack<EquityIntraDayTimeBarCollection>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = _activeStack.Pop();
                    if (currentTime.Subtract((DateTime) poppedItem.Epoch) > _activeTradeDuration)
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
        public Stack<EquityIntraDayTimeBarCollection> ActiveMarketHistory()
        {
            lock (_lock)
            {
                var tradeStackCopy = new Stack<EquityIntraDayTimeBarCollection>(_activeStack);
                var reverseCopyOfTradeStack = new Stack<EquityIntraDayTimeBarCollection>(tradeStackCopy);

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
