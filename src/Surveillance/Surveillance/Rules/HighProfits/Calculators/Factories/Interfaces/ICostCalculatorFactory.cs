using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces
{
    public interface ICostCalculatorFactory
    {
        ICostCalculator CostCalculator();
        ICostCalculator CurrencyConvertingCalculator(Domain.Finance.Currency currency);
    }
}