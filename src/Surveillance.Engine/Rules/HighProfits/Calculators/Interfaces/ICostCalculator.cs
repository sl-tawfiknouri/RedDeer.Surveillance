using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Financial;
using Domain.Trading;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces
{
    public interface ICostCalculator
    {
        Task<CurrencyAmount?> CalculateCostOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx);
    }
}
