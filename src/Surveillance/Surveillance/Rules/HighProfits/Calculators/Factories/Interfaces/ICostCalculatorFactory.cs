using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces
{
    public interface ICostCalculatorFactory
    {
        ICostCalculator CostCalculator();
        ICostCalculator CurrencyConvertingCalculator(DomainV2.Financial.Currency currency);
    }
}