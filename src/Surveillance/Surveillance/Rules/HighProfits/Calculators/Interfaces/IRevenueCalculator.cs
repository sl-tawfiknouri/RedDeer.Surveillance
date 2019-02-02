﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Trading;
using Surveillance.Markets.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Interfaces
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