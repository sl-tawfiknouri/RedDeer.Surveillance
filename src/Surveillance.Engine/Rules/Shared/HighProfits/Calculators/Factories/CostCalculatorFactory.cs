using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories
{
    public class CostCalculatorFactory : ICostCalculatorFactory
    {
        private readonly ICurrencyConverterService _currencyConverterService;

        private readonly ILogger<CostCurrencyConvertingCalculator> _currencyLogger;

        private readonly ILogger<CostCalculator> _logger;

        public CostCalculatorFactory(
            ICurrencyConverterService currencyConverterService,
            ILogger<CostCalculator> logger,
            ILogger<CostCurrencyConvertingCalculator> currencyLogger)
        {
            this._currencyConverterService = currencyConverterService
                                             ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._currencyLogger = currencyLogger ?? throw new ArgumentNullException(nameof(currencyLogger));
        }

        public ICostCalculator CostCalculator()
        {
            return new CostCalculator(this._logger);
        }

        public ICostCalculator CurrencyConvertingCalculator(Domain.Core.Financial.Money.Currency currency)
        {
            return new CostCurrencyConvertingCalculator(this._currencyConverterService, currency, this._currencyLogger);
        }
    }
}