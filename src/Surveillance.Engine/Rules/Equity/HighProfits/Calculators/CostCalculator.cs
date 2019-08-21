namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

    /// <summary>
    ///     Long equity cost calculator
    /// </summary>
    public class CostCalculator : ICostCalculator
    {
        private readonly ILogger<CostCalculator> _logger;

        public CostCalculator(ILogger<CostCalculator> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Sum the total buy in for the position
        /// </summary>
        public async Task<Money?> CalculateCostOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
        {
            if (activeFulfilledTradeOrders == null || !activeFulfilledTradeOrders.Any())
            {
                this._logger.LogInformation(
                    "CostCalculator CalculateCostOfPosition had null active trade orders or empty. Returning null.");
                return null;
            }

            var purchaseOrders = activeFulfilledTradeOrders
                .Where(
                    afto => afto.OrderDirection == OrderDirections.BUY || afto.OrderDirection == OrderDirections.COVER)
                .Select(
                    afto => new Money(
                        afto.OrderFilledVolume.GetValueOrDefault(0)
                        * afto.OrderAverageFillPrice.GetValueOrDefault().Value,
                        afto.OrderCurrency)).ToList();

            var money = new Money(purchaseOrders.Sum(po => po.Value), purchaseOrders.FirstOrDefault().Currency);

            this._logger.LogInformation(
                $"CostCalculator CalculateCostOfPosition had calculated costs for {activeFulfilledTradeOrders.FirstOrDefault()?.Instrument?.Identifiers} at {universeDateTime} as ({money.Currency}) {money.Value}.");

            return await Task.FromResult(money);
        }
    }
}