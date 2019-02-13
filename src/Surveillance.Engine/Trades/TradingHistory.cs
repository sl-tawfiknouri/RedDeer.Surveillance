using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Trading;
using Surveillance.Engine.Rules.Trades.Interfaces;

// ReSharper disable InconsistentlySynchronizedField

namespace Surveillance.Engine.Rules.Trades
{
    public class TradingHistory : ITradingHistory
    {
        private IList<Order> _active;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly IList<Order> _history;
        private readonly TimeSpan _activeTradeDuration;

        private readonly object _lock = new object();

        public TradingHistory(TimeSpan activeTradeDuration)
        {
            _history = new List<Order>();
            _activeTradeDuration = activeTradeDuration;
        }

        public void Add(Order frame, DateTime currentTime)
        {
            if (frame == null)
            {
                return;
            }

            lock (_lock)
            {
                if (currentTime.Subtract(frame.MostRecentDateEvent()) <= _activeTradeDuration)
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
                        .Where(item => currentTime.Subtract(item.MostRecentDateEvent()) > _activeTradeDuration)
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
        public IList<Order> ActiveTradeHistory()
        {
            var activeTrades = new List<Order>(_active);

            return activeTrades;
        }
    }
}
