namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces
{
    using Domain.Core.Financial.Money;

    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

    public interface IRevenueCalculatorFactory
    {
        IRevenueCalculator RevenueCalculator();

        IRevenueCalculator RevenueCalculatorMarketClosureCalculator();

        IRevenueCalculator RevenueCurrencyConvertingCalculator(Currency currency);

        IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(Currency currency);
    }
}