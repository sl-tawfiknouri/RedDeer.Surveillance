﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Trading;
using Surveillance.Markets.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Interfaces
{
    public interface IRevenueCalculator
    {
        Task<CurrencyAmount?> CalculateRevenueOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IUniverseMarketCache universeMarketCache);
    }
}