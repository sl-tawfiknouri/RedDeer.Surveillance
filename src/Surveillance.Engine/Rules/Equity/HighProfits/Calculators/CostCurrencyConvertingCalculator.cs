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
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

    public class CostCurrencyConvertingCalculator : ICostCalculator
    {
        private readonly ICurrencyConverterService _currencyConverterService;

        private readonly ILogger<CostCurrencyConvertingCalculator> _logger;

        private readonly Currency _targetCurrency;

        public CostCurrencyConvertingCalculator(
            ICurrencyConverterService currencyConverterService,
            Currency targetCurrency,
            ILogger<CostCurrencyConvertingCalculator> logger)
        {
            this._currencyConverterService = currencyConverterService
                                             ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this._targetCurrency = targetCurrency;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Money?> CalculateCostOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
        {
            if (activeFulfilledTradeOrders == null || !activeFulfilledTradeOrders.Any())
            {
                this._logger.LogInformation(
                    "CostCurrencyConvertingCalculator CalculateCostOfPosition had null or empty active fulfilled trade orders. Returning.");
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

            var adjustedToCurrencyPurchaseOrders = await this._currencyConverterService.Convert(
                                                       purchaseOrders,
                                                       this._targetCurrency,
                                                       universeDateTime,
                                                       ctx);

            this._logger.LogInformation(
                $"CostCurrencyConvertingCalculator CalculateCostOfPosition calculated for {activeFulfilledTradeOrders.FirstOrDefault()?.Instrument?.Identifiers} a cost of ({adjustedToCurrencyPurchaseOrders?.Currency}) {adjustedToCurrencyPurchaseOrders?.Value}");

            return adjustedToCurrencyPurchaseOrders;
        }
    }
}