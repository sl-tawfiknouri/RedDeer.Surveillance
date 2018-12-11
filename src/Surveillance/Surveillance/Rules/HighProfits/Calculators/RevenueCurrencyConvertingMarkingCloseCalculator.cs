using DomainV2.Equity.Frames;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueCurrencyConvertingMarkingCloseCalculator : RevenueCurrencyConvertingCalculator
    {
        public RevenueCurrencyConvertingMarkingCloseCalculator(
            DomainV2.Financial.Currency targetCurrency,
            ICurrencyConverter currencyConverter,
            ILogger<RevenueCurrencyConvertingCalculator> logger)
            : base(targetCurrency, currencyConverter, logger)
        { }

        protected override CurrencyAmount? SecurityTickToPrice(SecurityTick tick)
        {
            if (tick == null)
            {
                return null;
            }

            return tick.IntradayPrices.Close ?? tick.Spread.Price;
        }
    }
}
