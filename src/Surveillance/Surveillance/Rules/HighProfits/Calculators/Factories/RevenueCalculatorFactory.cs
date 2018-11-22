using System;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories
{
    public class RevenueCalculatorFactory : IRevenueCalculatorFactory
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly ILogger<RevenueCurrencyConvertingCalculator> _logger;

        public RevenueCalculatorFactory(
            ICurrencyConverter currencyConverter,
            ILogger<RevenueCurrencyConvertingCalculator> logger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IRevenueCalculator RevenueCalculator(Domain.Finance.Currency currency)
        {
            return new RevenueCurrencyConvertingCalculator(currency, _currencyConverter, _logger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingCalculator(Domain.Finance.Currency currency)
        {
            return new RevenueCurrencyConvertingCalculator(currency, _currencyConverter, _logger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(Domain.Finance.Currency currency)
        {
           return new RevenueCurrencyConvertingMarkingCloseCalculator(currency, _currencyConverter, _logger);
        }
    }
}
