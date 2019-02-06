using System;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories
{
    public class RevenueCalculatorFactory : IRevenueCalculatorFactory
    {
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly ILogger<RevenueCurrencyConvertingCalculator> _currencyConvertingLogger;
        private readonly ILogger<RevenueCalculator> _logger;

        public RevenueCalculatorFactory(
            IMarketTradingHoursManager tradingHoursManager,
            ICurrencyConverter currencyConverter,
            ILogger<RevenueCurrencyConvertingCalculator> currencyConvertingLogger,
            ILogger<RevenueCalculator> logger)
        {
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _currencyConvertingLogger = currencyConvertingLogger ?? throw new ArgumentNullException(nameof(currencyConvertingLogger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IRevenueCalculator RevenueCalculator()
        {
            return new RevenueCalculator(_tradingHoursManager, _logger);
        }

        public IRevenueCalculator RevenueCalculatorMarketClosureCalculator()
        {
            return new RevenueMarkingCloseCalculator(_tradingHoursManager, _logger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingCalculator(DomainV2.Financial.Currency currency)
        {
            return new RevenueCurrencyConvertingCalculator(currency, _currencyConverter, _tradingHoursManager, _currencyConvertingLogger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(DomainV2.Financial.Currency currency)
        {
           return new RevenueCurrencyConvertingMarkingCloseCalculator(currency, _currencyConverter, _tradingHoursManager, _currencyConvertingLogger);
        }
    }
}
