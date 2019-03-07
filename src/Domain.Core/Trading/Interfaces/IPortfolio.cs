using System;
using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace Domain.Core.Trading
{
    public interface IPortfolio
    {
        IHoldings Holdings { get; }
        IOrderLedger Ledger { get; }

        IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLossTotal();
        IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLoss(DateTime from, TimeSpan span);

        BalanceSheetStatement BalanceSheets();

        void Add(IReadOnlyCollection<Order> orders);
        void Add(Order order);
    }
}