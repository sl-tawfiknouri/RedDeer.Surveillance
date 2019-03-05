using System;
using System.Collections.Generic;
using Domain.Trading;

namespace Domain.Core.Trading
{
    public interface IPortfolio
    {
        Holdings Holdings { get; }
        OrderLedger Ledger { get; }

        ProfitAndLossStatement Accounts(DateTime from, TimeSpan fors);
        BalanceSheetStatement BalanceSheets();

        void Add(IReadOnlyCollection<Order> orders);
        void Add(Order order);
    }
}