using System;
using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace Domain.Core.Trading.Interfaces
{
    public interface IOrderLedger
    {
        void Add(Order order);
        IReadOnlyCollection<Order> FullLedger();
        IReadOnlyCollection<Order> LedgerEntries(DateTime from, TimeSpan to);
        long VolumeInLedger();
        long VolumeInLedgerWithStatus(OrderStatus status);
        decimal PercentageInStatusByOrder(OrderStatus status);
        decimal PercentageInStatusByVolume(OrderStatus status);
    }
}