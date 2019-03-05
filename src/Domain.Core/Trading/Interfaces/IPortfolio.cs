using System;
using System.Collections.Generic;
using Domain.Trading;

namespace Domain.Core.Trading
{
    public interface IPortfolio
    {
        IHoldings Holdings { get; }
        IOrderLedger Ledger { get; }

        ProfitAndLossStatement Accounts(DateTime from, TimeSpan fors);
        BalanceSheetStatement BalanceSheets();

        void Add(IReadOnlyCollection<Order> orders);
        void Add(Order order);
    }
}