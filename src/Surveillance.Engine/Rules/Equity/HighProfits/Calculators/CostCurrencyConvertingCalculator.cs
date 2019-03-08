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

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    public class CostCurrencyConvertingCalculator : ICostCalculator
    {
        private readonly ICurrencyConverterService _currencyConverterService;
        private readonly Domain.Core.Financial.Money.Currency _targetCurrency;
        private readonly ILogger<CostCurrencyConvertingCalculator> _logger;

        public CostCurrencyConvertingCalculator(
            ICurrencyConverterService currencyConverterService,
            Domain.Core.Financial.Money.Currency targetCurrency,
            ILogger<CostCurrencyConvertingCalculator> logger)
        {
            _currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            _targetCurrency = targetCurrency;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Money?> CalculateCostOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                _logger.LogInformation($"CostCurrencyConvertingCalculator CalculateCostOfPosition had null or empty active fulfilled trade orders. Returning.");
                return null;
            }

            var purchaseOrders =
                activeFulfilledTradeOrders
                    .Where(afto => afto.OrderDirection == OrderDirections.BUY
                                   || afto.OrderDirection == OrderDirections.COVER)
                    .Select(afto => 
                        new Money(
                            afto.OrderFilledVolume.GetValueOrDefault(0) * afto.OrderAverageFillPrice.GetValueOrDefault().Value,
                            afto.OrderCurrency))
                    .ToList();

            var adjustedToCurrencyPurchaseOrders = await _currencyConverterService.Convert(purchaseOrders, _targetCurrency, universeDateTime, ctx);

            _logger.LogInformation($"CostCurrencyConvertingCalculator CalculateCostOfPosition calculated for {activeFulfilledTradeOrders.FirstOrDefault()?.Instrument?.Identifiers} a cost of ({adjustedToCurrencyPurchaseOrders?.Currency}) {adjustedToCurrencyPurchaseOrders?.Value}");

            return adjustedToCurrencyPurchaseOrders;
        }
    }
}
