using Domain.Trading;
using System;

namespace Domain.Core.Trading
{
    /// <summary>
    /// Aggregate root (DDD)
    /// </summary>
    public class Portfolio
    {
        public Portfolio(Holdings holding, OrderLedger ledger, ProfitAndLossAccount accounts, BalanceSheet balanceSheets)
        {
            Holdings = holding ?? throw new ArgumentNullException(nameof(holding));
            Ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
            Accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
            BalanceSheets = balanceSheets ?? throw new ArgumentNullException(nameof(balanceSheets));
        }

        public Holdings Holdings { get; }

        public OrderLedger Ledger { get; }

        public ProfitAndLossAccount Accounts { get; }

        public BalanceSheet BalanceSheets { get; }

        public void Add(Order order)
        {
            // add to ledger
            // add to holdings
            // update accounts
        }
    }
}
