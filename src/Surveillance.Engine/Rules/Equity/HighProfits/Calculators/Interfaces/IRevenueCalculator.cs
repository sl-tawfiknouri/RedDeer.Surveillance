using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Trading;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces
{
    public interface IRevenueCalculator
    {
        Task<RevenueCurrencyAmount> CalculateRevenueOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IMarketDataCacheStrategy cacheStrategy);
    }
}