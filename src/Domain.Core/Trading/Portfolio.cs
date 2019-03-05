using System;

namespace Domain.Core.Trading
{
    /// <summary>
    /// Aggregate root (DDD)
    /// </summary>
    public class Portfolio
    {
        public Portfolio(OrderLedger ledger)
        {
            Ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
        }

        public OrderLedger Ledger { get; }
    }
}
