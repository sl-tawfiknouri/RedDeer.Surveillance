using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories
{
    public class CostCalculatorFactory : ICostCalculatorFactory
    {
        private readonly ICurrencyConverterService _currencyConverterService;
        private readonly ILogger<CostCalculator> _logger;
        private readonly ILogger<CostCurrencyConvertingCalculator> _currencyLogger;

        public CostCalculatorFactory(
            ICurrencyConverterService currencyConverterService,
            ILogger<CostCalculator> logger, 
            ILogger<CostCurrencyConvertingCalculator> currencyLogger)
        {
            _currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currencyLogger = currencyLogger ?? throw new ArgumentNullException(nameof(currencyLogger));
        }

        public ICostCalculator CostCalculator()
        {
            return new CostCalculator(_logger);
        }

        public ICostCalculator CurrencyConvertingCalculator(Domain.Core.Financial.Money.Currency currency)
        {
            return new CostCurrencyConvertingCalculator(_currencyConverterService, currency, _currencyLogger);
        }
    }
}
