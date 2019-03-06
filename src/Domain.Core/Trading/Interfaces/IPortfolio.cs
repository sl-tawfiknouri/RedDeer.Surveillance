using System;
using System.Collections.Generic;
using Domain.Trading;

namespace Domain.Core.Trading
{
    public interface IPortfolio
    {
        IHoldings Holdings { get; }
        IOrderLedger Ledger { get; }

        ProfitAndLossStatement ProfitAndLoss(DateTime from, TimeSpan span);
        BalanceSheetStatement BalanceSheets();

        void Add(IReadOnlyCollection<Order> orders);
        void Add(Order order);
    }
}