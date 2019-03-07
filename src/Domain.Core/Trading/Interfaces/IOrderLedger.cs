using System;
using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace Domain.Core.Trading
{
    public interface IOrderLedger
    {
        void Add(Order order);
        IReadOnlyCollection<Order> FullLedger();
        IReadOnlyCollection<Order> LedgerEntries(DateTime from, TimeSpan to);
    }
}