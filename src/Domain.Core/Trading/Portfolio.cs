using Domain.Core.Financial;
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

        public ProfitAndLossStatement ProfitAndLoss(DateTime from, TimeSpan span)
        {
            // calculate P&L in class
            var orders = Ledger.LedgerEntries(from, span);
            var revenues = Revenues(orders);
            var costs = Costs(orders);

            return new ProfitAndLossStatement(revenues, costs);
        }

        private Money Revenues(IReadOnlyCollection<Order> orders)
        {
            if (orders == null
                || !orders.Any())
            {
                return new Money(0, "GBX");
            }

            var order = 
                orders
                .Where(i => 
                    i.OrderDirection == OrderDirections.SELL || i.OrderDirection == OrderDirections.SHORT)
                .ToList();

            var revenues = order.Sum(i => (i.OrderAverageFillPrice?.Value ?? 0) * i.OrderFilledVolume);

            return new Money(revenues, "GBX");
        }

        private Money Costs(IReadOnlyCollection<Order> orders)
        {
            if (orders == null
                || !orders.Any())
            {
                return new Money(0, "GBX");
            }

            var order =
                orders
                .Where(i =>
                    i.OrderDirection == OrderDirections.BUY || i.OrderDirection == OrderDirections.COVER)
                .ToList();

            var costs = order.Sum(i => (i.OrderAverageFillPrice?.Value ?? 0) * i.OrderFilledVolume);

            return new Money(costs, "GBX");
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
            if (order == null)
            {
                return;
            }

            Ledger.Add(order);
        }
    }
}
