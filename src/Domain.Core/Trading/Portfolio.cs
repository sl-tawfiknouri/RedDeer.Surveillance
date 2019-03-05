using Domain.Trading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Core.Trading
{
    /// <summary>
    /// Aggregate root (DDD)
    /// </summary>
    public class Portfolio : IPortfolio
    {
        public Portfolio(IHoldings holding, IOrderLedger ledger)
        {
            Holdings = holding ?? throw new ArgumentNullException(nameof(holding));
            Ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
        }

        public IHoldings Holdings { get; }

        public IOrderLedger Ledger { get; }

        public ProfitAndLossStatement Accounts(DateTime from, TimeSpan fors)
        {
            // calculate P&L in class

            return null;
        }

        public BalanceSheetStatement BalanceSheets()
        {
            // calculate balance sheet in class

            return null;
        }

        public void Add(IReadOnlyCollection<Order> orders)
        {
            if (orders == null
                || !orders.Any())
            {
                return;
            }

            foreach (var order in orders)
                Add(order);
        }

        public void Add(Order order)
        {
            // add to ledger
            // add to holdings
            // update accounts
        }
    }
}
