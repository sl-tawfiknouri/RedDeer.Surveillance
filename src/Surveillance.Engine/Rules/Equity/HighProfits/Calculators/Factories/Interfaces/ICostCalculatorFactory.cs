namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces
{
    using Domain.Core.Financial.Money;

    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

    public interface ICostCalculatorFactory
    {
        ICostCalculator CostCalculator();

        ICostCalculator CurrencyConvertingCalculator(Currency currency);
    }
}