using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Interfaces;
using Domain.Core.Trading.Orders;

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

        public decimal VolumeInLedger()
        {
            return _ledger.Sum(i => i.OrderFilledVolume ?? i.OrderOrderedVolume ?? 0);
        }

        public decimal VolumeInLedgerWithStatus(OrderStatus orderStatus)
        {
            return _ledger.Where(i => i.OrderStatus() == orderStatus).Sum(i => i.OrderFilledVolume ?? i.OrderOrderedVolume ?? 0);
        }

        public decimal PercentageInStatusByOrder(OrderStatus orderStatus)
        {
            var ledgerTotal = (decimal)_ledger.Count;
            if (ledgerTotal == 0)
            {
                return 0;
            }

            var inStatus = (decimal)_ledger.Count(i => i.OrderStatus() == orderStatus);
            if (inStatus == 0)
            {
                return 0;
            }

            var percentage = inStatus / ledgerTotal;

            return Math.Round(percentage, 2, MidpointRounding.AwayFromZero);
        }

        public decimal PercentageInStatusByVolume(OrderStatus orderStatus)
        {
            var ledgerTotal = (decimal)_ledger.Sum(i => i.OrderFilledVolume ?? i.OrderOrderedVolume ?? 0);
            if (ledgerTotal == 0)
            {
                return 0;
            }

            var inStatus = (decimal)_ledger.Where(i => i.OrderStatus() == orderStatus).Sum(i => i.OrderFilledVolume ?? i.OrderOrderedVolume ?? 0);
            if (inStatus == 0)
            {
                return 0;
            }

            var percentage = inStatus / ledgerTotal;

            return Math.Round(percentage, 2, MidpointRounding.AwayFromZero);
        }
    }
}
