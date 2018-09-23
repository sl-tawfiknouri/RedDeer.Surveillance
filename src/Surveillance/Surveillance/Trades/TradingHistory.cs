using System;
using System.Linq;
using System.Collections.Generic;
using Domain.Trades.Orders;
using Surveillance.Trades.Interfaces;
// ReSharper disable InconsistentlySynchronizedField

namespace Surveillance.Trades
{
    public class TradingHistory : ITradingHistory
    {
        private IList<TradeOrderFrame> _active;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly IList<TradeOrderFrame> _history;
        private readonly TimeSpan _activeTradeDuration;

        private readonly object _lock = new object();

        public TradingHistory(TimeSpan activeTradeDuration)
        {
            _history = new List<TradeOrderFrame>();
            _activeTradeDuration = activeTradeDuration;
        }

        public void Add(TradeOrderFrame frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract(frame.StatusChangedOn) <= _activeTradeDuration)
                {
                    _active.Add(frame);
                }
                else
                {
                    _history.Add(frame);
                }
            }
        }

        public void ArchiveExpiredActiveItems(DateTime currentTime)
        {
            lock (_lock)
            {
                var itemsToArchive =
                    _active
                        .Where(item => currentTime.Subtract(item.StatusChangedOn) > _activeTradeDuration)
                        .ToList();

                var newActive = _active.Except(itemsToArchive).ToList();
                _active = newActive;

                foreach (var item in itemsToArchive)
                {
                    _history.Add(item);
                }
            }
        }

        /// <summary>
        /// Does not provide access to the underlying collection via reference
        /// Instead it returns a new list with the same underlying elements
        /// </summary>
        public IList<TradeOrderFrame> ActiveTradeHistory()
        {
            var activeTrades = new List<TradeOrderFrame>(_active);

            return activeTrades;
        }
    }
}
