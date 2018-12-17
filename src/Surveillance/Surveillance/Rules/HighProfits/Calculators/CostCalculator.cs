using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Trading;
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
            IList<Order> activeFulfilledTradeOrders, 
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
                    .Where(afto => afto.OrderPosition == OrderPositions.BUY
                                   || afto.OrderPosition == OrderPositions.SHORT)
                    .Select(afto => 
                        new CurrencyAmount(
                            afto.OrderFilledVolume.GetValueOrDefault(0) * afto.OrderAveragePrice.GetValueOrDefault().Value,
                            afto.OrderCurrency))
                    .ToList();

            return new CurrencyAmount(purchaseOrders.Sum(po => po.Value), purchaseOrders.FirstOrDefault().Currency);
        }
    }
}
