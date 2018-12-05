using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Trades.Orders;
using DomainV2.Financial;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    /// <summary>
    /// Long equity cost calculator
    /// </summary>
    public class CostCalculator : ICostCalculator
    {
        /// <summary>
        /// Sum the total buy in for the position
        /// </summary>
        public async Task<CurrencyAmount?> CalculateCostOfPosition(
            IList<TradeOrderFrame> activeFulfilledTradeOrders, 
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                return null;
            }

            var purchaseOrders =
                activeFulfilledTradeOrders
                    .Where(afto => afto.Position == OrderPosition.Buy)
                    .Select(afto => new CurrencyAmount(afto.FulfilledVolume * afto.ExecutedPrice?.Value ?? 0, afto.OrderCurrency))
                    .ToList();

            return new CurrencyAmount(purchaseOrders.Sum(po => po.Value), purchaseOrders.FirstOrDefault().Currency);
        }
    }
}
