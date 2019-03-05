using Domain.Trading;
using System;

namespace Domain.Core.Trading
{
    /// <summary>
    /// Aggregate root (DDD)
    /// </summary>
    public class Portfolio
    {
        public Portfolio(Holdings holding, OrderLedger ledger)
        {
            Holdings = holding ?? throw new ArgumentNullException(nameof(holding));
            Ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
        }

        public Holdings Holdings { get; }

        public OrderLedger Ledger { get; }

        public ProfitAndLossStatement Accounts()
        {
            return null;
        }

        public BalanceSheetStatement BalanceSheets()
        {
            return null;
        }

        public void Add(Order order)
        {
            // add to ledger
            // add to holdings
            // update accounts
        }
    }
}
