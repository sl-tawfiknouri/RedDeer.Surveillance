using Domain.Trading;
using System;

namespace Domain.Core.Trading
{
    /// <summary>
    /// Aggregate root (DDD)
    /// </summary>
    public class Portfolio
    {
        public Portfolio(Holdings holding, OrderLedger ledger, ProfitAndLossAccount accounts)
        {
            Holdings = holding ?? throw new ArgumentNullException(nameof(holding));
            Ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
            Accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
        }

        public Holdings Holdings { get; }

        public OrderLedger Ledger { get; }

        public ProfitAndLossAccount Accounts { get; }

        public void Add(Order order)
        {
            // add to ledger
            // add to holdings
            // update accounts
        }
    }
}
