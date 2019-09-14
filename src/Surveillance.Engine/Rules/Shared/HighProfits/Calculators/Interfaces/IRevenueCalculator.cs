using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core.Trading.Orders;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces
{
    public interface IRevenueCalculator
    {
        Task<RevenueMoney> CalculateRevenueOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IMarketDataCacheStrategy cacheStrategy);
    }
}