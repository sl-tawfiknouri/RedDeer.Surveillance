namespace Domain.Core.Trading.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Accounts;
    using Domain.Core.Trading.Orders;

    public interface IPortfolio
    {
        IOrderLedger Ledger { get; }

        void Add(IReadOnlyCollection<Order> orders);

        void Add(Order order);

        IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLoss(DateTime from, TimeSpan span);

        IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLossTotal();
    }
}