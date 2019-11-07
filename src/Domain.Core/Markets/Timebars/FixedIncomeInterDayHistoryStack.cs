using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Core.Markets.Timebars
{
    public class FixedIncomeInterDayHistoryStack : IFixedIncomeInterDayHistoryStack
    {
        private readonly Stack<FixedIncomeInterDayTimeBarCollection> _activeStack;

        private readonly object _lock = new object();

        private Market _market;

        public FixedIncomeInterDayHistoryStack()
        {
            this._activeStack = new Stack<FixedIncomeInterDayTimeBarCollection>();
        }

        /// <summary>
        ///     Does not provide access to the underlying collection via reference
        ///     Instead it returns a new list with the same underlying elements
        /// </summary>
        public Stack<FixedIncomeInterDayTimeBarCollection> ActiveMarketHistory()
        {
            lock (this._lock)
            {
                var tradeStackCopy = new Stack<FixedIncomeInterDayTimeBarCollection>(this._activeStack);
                var reverseCopyOfTradeStack = new Stack<FixedIncomeInterDayTimeBarCollection>(tradeStackCopy);

                // copy twice in order to restore initial order of elements
                return reverseCopyOfTradeStack;
            }
        }

        public void Add(FixedIncomeInterDayTimeBarCollection frame, DateTime currentTime)
        {
            if (frame == null) return;

            lock (this._lock)
            {
                // Ensure all contents have the same date (may not work well in pacific zone with the international date line ++ trading hours - should be OK for Japan/Tokyo and USA/SanFran) - RT
                if (currentTime.Date == frame.Epoch.Date) this._activeStack.Push(frame);
            }
        }

        public void ArchiveExpiredActiveItems(DateTime currentTime)
        {
            lock (this._lock)
            {
                var initialActiveStackCount = this._activeStack.Count;
                var counterPartyStack = new Stack<FixedIncomeInterDayTimeBarCollection>();

                while (initialActiveStackCount > 0)
                {
                    var poppedItem = this._activeStack.Pop();
                    if (currentTime.Date == poppedItem.Epoch.Date) counterPartyStack.Push(poppedItem);

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
