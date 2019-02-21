using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces
{
    public interface IRevenueCalculatorFactory
    {
        IRevenueCalculator RevenueCalculator();
        IRevenueCalculator RevenueCalculatorMarketClosureCalculator();

        IRevenueCalculator RevenueCurrencyConvertingCalculator(Domain.Financial.Currency currency);
        IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(Domain.Financial.Currency currency);
    }
}