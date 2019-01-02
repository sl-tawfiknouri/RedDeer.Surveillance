using System;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories
{
    public class CostCalculatorFactory : ICostCalculatorFactory
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly ILogger<CostCalculator> _logger;
        private readonly ILogger<CostCurrencyConvertingCalculator> _currencyLogger;

        public CostCalculatorFactory(
            ICurrencyConverter currencyConverter,
            ILogger<CostCalculator> logger, 
            ILogger<CostCurrencyConvertingCalculator> currencyLogger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currencyLogger = currencyLogger ?? throw new ArgumentNullException(nameof(currencyLogger));
        }

        public ICostCalculator CostCalculator()
        {
            return new CostCalculator(_logger);
        }

        public ICostCalculator CurrencyConvertingCalculator(DomainV2.Financial.Currency currency)
        {
            return new CostCurrencyConvertingCalculator(_currencyConverter, currency, _currencyLogger);
        }
    }
}
