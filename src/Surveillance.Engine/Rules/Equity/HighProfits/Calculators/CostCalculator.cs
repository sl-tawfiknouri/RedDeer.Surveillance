using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    /// <summary>
    /// Long equity cost calculator
    /// </summary>
    public class CostCalculator : ICostCalculator
    {
        private readonly ILogger<CostCalculator> _logger;

        public CostCalculator(ILogger<CostCalculator> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                _logger.LogInformation($"CostCalculator CalculateCostOfPosition had null active trade orders or empty. Returning null.");
                return null;
            }

            var purchaseOrders =
                activeFulfilledTradeOrders
                    .Where(afto => afto.OrderDirection == OrderDirections.BUY
                                   || afto.OrderDirection == OrderDirections.COVER)
                    .Select(afto => 
                        new CurrencyAmount(
                            afto.OrderFilledVolume.GetValueOrDefault(0) * afto.OrderAverageFillPrice.GetValueOrDefault().Value,
                            afto.OrderCurrency))
                    .ToList();

            var currencyAmount = new CurrencyAmount(purchaseOrders.Sum(po => po.Value), purchaseOrders.FirstOrDefault().Currency);

            _logger.LogInformation($"CostCalculator CalculateCostOfPosition had calculated costs for {activeFulfilledTradeOrders.FirstOrDefault()?.Instrument?.Identifiers} at {universeDateTime} as ({currencyAmount.Currency}) {currencyAmount.Value}.");

            return currencyAmount;
        }
    }
}
