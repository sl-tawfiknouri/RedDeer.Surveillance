using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces
{
    public interface ICostCalculatorFactory
    {
        ICostCalculator CostCalculator();
        ICostCalculator CurrencyConvertingCalculator(Domain.Core.Financial.Money.Currency currency);
    }
}