namespace Domain.Core.Trading.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    public interface IOrderLedger
    {
        void Add(Order order);

        IReadOnlyCollection<Order> FullLedger();

        IReadOnlyCollection<Order> LedgerEntries(DateTime from, TimeSpan to);

        decimal PercentageInStatusByOrder(OrderStatus status);

        decimal PercentageInStatusByVolume(OrderStatus status);

        decimal VolumeInLedger();

        decimal VolumeInLedgerWithStatus(OrderStatus status);
    }
}