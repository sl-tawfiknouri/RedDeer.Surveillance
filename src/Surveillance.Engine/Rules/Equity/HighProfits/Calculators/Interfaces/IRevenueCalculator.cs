namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;

    public interface IRevenueCalculator
    {
        Task<RevenueMoney> CalculateRevenueOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IMarketDataCacheStrategy cacheStrategy);
    }
}