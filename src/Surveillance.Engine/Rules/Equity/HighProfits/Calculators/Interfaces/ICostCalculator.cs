namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading.Orders;

    using Surveillance.Auditing.Context.Interfaces;

    public interface ICostCalculator
    {
        Task<Money?> CalculateCostOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx);
    }
}