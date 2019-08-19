namespace Domain.Core.Trading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;

    public class OrderLedger : IOrderLedger
    {
        private readonly List<Order> _ledger;

        public OrderLedger()
        {
            this._ledger = new List<Order>();
        }

        public void Add(Order order)
        {
            if (order == null) return;

            this._ledger.Add(order);
        }

        public IReadOnlyCollection<Order> FullLedger()
        {
            return this._ledger;
        }

        public IReadOnlyCollection<Order> LedgerEntries(DateTime from, TimeSpan to)
        {
            var toDate = from.Add(to);

            var orders = this._ledger.Where(i => i.PlacedDate >= from).Where(i => i.MostRecentDateEvent() <= toDate)
                .ToList();

            return orders;
        }

        public decimal PercentageInStatusByOrder(OrderStatus orderStatus)
        {
            var ledgerTotal = (decimal)this._ledger.Count;
            if (ledgerTotal == 0) return 0;

            var inStatus = (decimal)this._ledger.Count(i => i.OrderStatus() == orderStatus);
            if (inStatus == 0) return 0;

            var percentage = inStatus / ledgerTotal;

            return Math.Round(percentage, 2, MidpointRounding.AwayFromZero);
        }

        public decimal PercentageInStatusByVolume(OrderStatus orderStatus)
        {
            var ledgerTotal = this._ledger.Sum(i => i.OrderFilledVolume ?? i.OrderOrderedVolume ?? 0);
            if (ledgerTotal == 0) return 0;

            var inStatus = this._ledger.Where(i => i.OrderStatus() == orderStatus)
                .Sum(i => i.OrderFilledVolume ?? i.OrderOrderedVolume ?? 0);
            if (inStatus == 0) return 0;

            var percentage = inStatus / ledgerTotal;

            return Math.Round(percentage, 2, MidpointRounding.AwayFromZero);
        }

        public decimal VolumeInLedger()
        {
            return this._ledger.Sum(i => i.OrderFilledVolume ?? i.OrderOrderedVolume ?? 0);
        }

        public decimal VolumeInLedgerWithStatus(OrderStatus orderStatus)
        {
            return this._ledger.Where(i => i.OrderStatus() == orderStatus)
                .Sum(i => i.OrderFilledVolume ?? i.OrderOrderedVolume ?? 0);
        }
    }
}