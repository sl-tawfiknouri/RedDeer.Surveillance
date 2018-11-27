using System;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories
{
    public class CostCalculatorFactory : ICostCalculatorFactory
    {
        private readonly ICurrencyConverter _currencyConverter;

        public CostCalculatorFactory(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
        }

        public ICostCalculator CostCalculator()
        {
            return new CostCalculator();
        }

        public ICostCalculator CurrencyConvertingCalculator(Domain.Finance.Currency currency)
        {
            return new CostCurrencyConvertingCalculator(_currencyConverter, currency);
        }
    }
}
