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

        public IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLoss(DateTime from, TimeSpan span)
        {
            var orders = Ledger.LedgerEntries(from, span);
            var ordersByCurrency = orders.GroupBy(i => i.OrderCurrency);

            var profitAndLossAccounts =
                ordersByCurrency
                .Select(i => ProfitAndLossStatement(i.ToList(), i.Key))
                .ToList();

            if (profitAndLossAccounts == null
                || !profitAndLossAccounts.Any())
            {
                profitAndLossAccounts.Add(Trading.ProfitAndLossStatement.Empty());
            }

            return profitAndLossAccounts;
        }

        private ProfitAndLossStatement ProfitAndLossStatement(IReadOnlyCollection<Order> orders, Currency denominatedCurrency)
        {
            var revenues = Revenues(orders, denominatedCurrency);
            var costs = Costs(orders, denominatedCurrency);

            return new ProfitAndLossStatement(denominatedCurrency, revenues, costs);
        }

        private Money Revenues(IReadOnlyCollection<Order> orders, Currency denominatedCurrency)
        {
            return ValueTradedInDirections(
                orders,
                new[] { OrderDirections.SELL, OrderDirections.SHORT },
                denominatedCurrency);
        }

        private Money Costs(IReadOnlyCollection<Order> orders, Currency denominatedCurrency)
        {
            return ValueTradedInDirections(
                orders,
                new[] { OrderDirections.BUY, OrderDirections.COVER },
                denominatedCurrency);
        }

        private Money ValueTradedInDirections(IReadOnlyCollection<Order> orders, IReadOnlyCollection<OrderDirections> directions, Currency denominatedCurrency)
        {
            if (orders == null
                || !orders.Any()
                || directions == null
                || !directions.Any())
            {
                return new Money(0, denominatedCurrency);
            }

            var order =
                orders
                .Where(i => directions.Contains(i.OrderDirection))
                .ToList();

            var costs = order.Sum(i => (i.OrderAverageFillPrice?.Value ?? 0) * i.OrderFilledVolume);

            return new Money(costs, denominatedCurrency);
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
