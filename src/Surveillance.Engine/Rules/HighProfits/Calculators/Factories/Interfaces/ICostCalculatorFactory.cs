using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Factories.Interfaces
{
    public interface ICostCalculatorFactory
    {
        ICostCalculator CostCalculator();
        ICostCalculator CurrencyConvertingCalculator(DomainV2.Financial.Currency currency);
    }
}