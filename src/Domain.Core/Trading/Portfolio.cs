namespace Domain.Core.Trading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Accounts;
    using Domain.Core.Financial.Money;
    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;

    /// <summary>
    ///     Aggregate root (DDD)
    /// </summary>
    public class Portfolio : IPortfolio
    {
        public Portfolio(IOrderLedger ledger)
        {
            this.Ledger = ledger ?? throw new ArgumentNullException(nameof(ledger));
        }

        /// <summary>
        ///     History of order activity for the portfolio
        /// </summary>
        public IOrderLedger Ledger { get; }

        public void Add(IReadOnlyCollection<Order> orders)
        {
            if (orders == null || !orders.Any()) return;

            foreach (var order in orders) this.Add(order);
        }

        public void Add(Order order)
        {
            if (order == null) return;

            this.Ledger.Add(order);
        }

        public IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLoss(DateTime from, TimeSpan span)
        {
            var orders = this.Ledger.LedgerEntries(from, span);

            return this.ProfitAndLossForOrders(orders);
        }

        public IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLossTotal()
        {
            var orders = this.Ledger.FullLedger();

            return this.ProfitAndLossForOrders(orders);
        }

        private Money Costs(IReadOnlyCollection<Order> orders, Currency denominatedCurrency)
        {
            return this.ValueTradedInDirections(
                orders,
                new[] { OrderDirections.BUY, OrderDirections.COVER },
                denominatedCurrency);
        }

        private IReadOnlyCollection<ProfitAndLossStatement> ProfitAndLossForOrders(IReadOnlyCollection<Order> orders)
        {
            var ordersByCurrency = orders.GroupBy(i => i.OrderCurrency);

            var profitAndLossAccounts =
                ordersByCurrency.Select(i => this.ProfitAndLossStatement(i.ToList(), i.Key)).ToList();

            if (profitAndLossAccounts == null || !profitAndLossAccounts.Any())
                profitAndLossAccounts?.Add(Accounts.ProfitAndLossStatement.Empty());

            return profitAndLossAccounts;
        }

        private ProfitAndLossStatement ProfitAndLossStatement(
            IReadOnlyCollection<Order> orders,
            Currency denominatedCurrency)
        {
            var revenues = this.Revenues(orders, denominatedCurrency);
            var costs = this.Costs(orders, denominatedCurrency);

            return new ProfitAndLossStatement(denominatedCurrency, revenues, costs);
        }

        private Money Revenues(IReadOnlyCollection<Order> orders, Currency denominatedCurrency)
        {
            return this.ValueTradedInDirections(
                orders,
                new[] { OrderDirections.SELL, OrderDirections.SHORT },
                denominatedCurrency);
        }

        private Money ValueTradedInDirections(
            IReadOnlyCollection<Order> orders,
            IReadOnlyCollection<OrderDirections> directions,
            Currency denominatedCurrency)
        {
            if (orders == null || !orders.Any() || directions == null || !directions.Any())
                return new Money(0, denominatedCurrency);

            var order = orders.Where(i => directions.Contains(i.OrderDirection)).ToList();

            var costs = order.Sum(i => (i.OrderAverageFillPrice?.Value ?? 0) * i.OrderFilledVolume);

            return new Money(costs, denominatedCurrency);
        }
    }
}