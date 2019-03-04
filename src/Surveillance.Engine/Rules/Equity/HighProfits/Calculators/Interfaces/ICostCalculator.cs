using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Trading;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces
{
    public interface ICostCalculator
    {
        Task<Money?> CalculateCostOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx);
    }
}
