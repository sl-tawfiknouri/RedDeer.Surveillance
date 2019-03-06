using Domain.Trading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Core.Trading
{
    public class OrderLedger : IOrderLedger
    {
        private readonly List<Order> _ledger;

        public OrderLedger()
        {
            _ledger = new List<Order>();
        }

        public void Add(Order order)
        {
            if (order == null)
            {
                return;
            }

            _ledger.Add(order);
        }

        public IReadOnlyCollection<Order> FullLedger()
        {
            return _ledger;
        }

        public IReadOnlyCollection<Order> LedgerEntries(DateTime from, TimeSpan to)
        {
            var toDate = from.Add(to);

            var orders = _ledger
                .Where(i => i.PlacedDate >= from)
                .Where(i => i.MostRecentDateEvent() <= toDate)
                .ToList();

            return orders;
        }
    }
}
