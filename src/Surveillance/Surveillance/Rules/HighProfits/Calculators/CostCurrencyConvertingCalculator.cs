using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Trading;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class CostCurrencyConvertingCalculator : ICostCalculator
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly DomainV2.Financial.Currency _targetCurrency;

        public CostCurrencyConvertingCalculator(
            ICurrencyConverter currencyConverter,
            DomainV2.Financial.Currency targetCurrency)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _targetCurrency = targetCurrency;
        }

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

            var adjustedToCurrencyPurchaseOrders = await _currencyConverter.Convert(purchaseOrders, _targetCurrency, universeDateTime, ctx);

            return adjustedToCurrencyPurchaseOrders;
        }
    }
}
