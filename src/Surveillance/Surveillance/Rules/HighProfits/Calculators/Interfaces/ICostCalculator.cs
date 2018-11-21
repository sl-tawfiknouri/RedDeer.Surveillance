using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Finance;
using Domain.Trades.Orders;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Interfaces
{
    public interface ICostCalculator
    {
        Task<CurrencyAmount?> CalculateCostOfPosition(
            IList<TradeOrderFrame> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx);
    }
}
