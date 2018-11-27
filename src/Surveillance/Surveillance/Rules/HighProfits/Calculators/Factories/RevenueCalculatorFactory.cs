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
        private readonly ILogger<RevenueCurrencyConvertingCalculator> _currencyConvertingLogger;
        private readonly ILogger<RevenueCalculator> _logger;

        public RevenueCalculatorFactory(
            ICurrencyConverter currencyConverter,
            ILogger<RevenueCurrencyConvertingCalculator> currencyConvertingLogger,
            ILogger<RevenueCalculator> logger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _currencyConvertingLogger = currencyConvertingLogger ?? throw new ArgumentNullException(nameof(currencyConvertingLogger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IRevenueCalculator RevenueCalculator()
        {
            return new RevenueCalculator(_logger);
        }

        public IRevenueCalculator RevenueCalculatorMarkingTheClose()
        {
            return new RevenueMarkingCloseCalculator(_logger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingCalculator(Domain.Finance.Currency currency)
        {
            return new RevenueCurrencyConvertingCalculator(currency, _currencyConverter, _currencyConvertingLogger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(Domain.Finance.Currency currency)
        {
           return new RevenueCurrencyConvertingMarkingCloseCalculator(currency, _currencyConverter, _currencyConvertingLogger);
        }
    }
}
