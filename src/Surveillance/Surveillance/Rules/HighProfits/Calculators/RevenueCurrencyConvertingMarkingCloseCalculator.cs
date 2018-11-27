using Domain.Equity;
using Domain.Equity.Frames;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueCurrencyConvertingMarkingCloseCalculator : RevenueCurrencyConvertingCalculator
    {
        public RevenueCurrencyConvertingMarkingCloseCalculator(
            Domain.Finance.Currency targetCurrency,
            ICurrencyConverter currencyConverter,
            ILogger<RevenueCurrencyConvertingCalculator> logger)
            : base(targetCurrency, currencyConverter, logger)
        { }

        protected override Price? SecurityTickToPrice(SecurityTick tick)
        {
            if (tick == null)
            {
                return null;
            }

            return tick.IntradayPrices.Close ?? tick.Spread.Price;
        }
    }
}
