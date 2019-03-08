using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories
{
    public class RevenueCalculatorFactory : IRevenueCalculatorFactory
    {
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly ICurrencyConverterService _currencyConverterService;
        private readonly ILogger<RevenueCurrencyConvertingCalculator> _currencyConvertingLogger;
        private readonly ILogger<RevenueCalculator> _logger;

        public RevenueCalculatorFactory(
            IMarketTradingHoursService tradingHoursService,
            ICurrencyConverterService currencyConverterService,
            ILogger<RevenueCurrencyConvertingCalculator> currencyConvertingLogger,
            ILogger<RevenueCalculator> logger)
        {
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            _currencyConvertingLogger = currencyConvertingLogger ?? throw new ArgumentNullException(nameof(currencyConvertingLogger));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IRevenueCalculator RevenueCalculator()
        {
            return new RevenueCalculator(_tradingHoursService, _logger);
        }

        public IRevenueCalculator RevenueCalculatorMarketClosureCalculator()
        {
            return new RevenueMarkingCloseCalculator(_tradingHoursService, _logger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingCalculator(Domain.Core.Financial.Money.Currency currency)
        {
            return new RevenueCurrencyConvertingCalculator(currency, _currencyConverterService, _tradingHoursService, _currencyConvertingLogger);
        }

        public IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(Domain.Core.Financial.Money.Currency currency)
        {
           return new RevenueCurrencyConvertingMarkingCloseCalculator(currency, _currencyConverterService, _tradingHoursService, _currencyConvertingLogger);
        }
    }
}
