namespace Domain.Core.Markets.Timebars
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Interfaces;

    public class EquityIntraDayHistoryStack : IEquityIntraDayHistoryStack
    {
        private readonly Stack<EquityIntraDayTimeBarCollection> _activeStack;

        private readonly TimeSpan _activeTradeDuration;

        private readonly object _lock = new object();

        private Market _market;

        public EquityIntraDayHistoryStack(TimeSpan activeTradeDuration)
        {
            this._activeStack = new Stack<EquityIntraDayTimeBarCollection>();
            this._activeTradeDuration = activeTradeDuration;
        }

        /// <summary>
        ///     Does not provide access to the underlying collection via reference
        ///     Instead it returns a new list with the same underlying elements
        /// </summary>
        public Stack<EquityIntraDayTimeBarCollection> ActiveMarketHistory()
        {
            lock (this._lock)
            {
                var tradeStackCopy = new Stack<EquityIntraDayTimeBarCollection>(this._activeStack);
                var reverseCopyOfTradeStack = new Stack<EquityIntraDayTimeBarCollection>(tradeStackCopy);

                // copy twice in order to restore initial order of elements
                return reverseCopyOfTradeStack;
            }
        }

        public void Add(EquityIntraDayTimeBarCollection frame, DateTime currentTime)
        {
            if (frame == null) return;

            lock (this._lock)
            {
                if (currentTime.Subtract(frame.Epoch) <= this._activeTradeDuration) this._activeStack.Push(frame);
            }
        }

        public void ArchiveExpiredActiveItems(DateTime currentTime)
        {
            lock (this._lock)
            {
                var initialActiveStackCount = this._activeStack.Count;
                var counterPartyStack = new Stack<EquityIntraDayTimeBarCollection>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = this._activeStack.Pop();
                    if (currentTime.Subtract(poppedItem.Epoch) <= this._activeTradeDuration)
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
            this._market = this._activeStack?.Peek()?.Exchange;

            return this._market;
        }
    }
}