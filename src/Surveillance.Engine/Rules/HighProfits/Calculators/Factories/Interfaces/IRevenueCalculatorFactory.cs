using Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighProfits.Calculators.Factories.Interfaces
{
    public interface IRevenueCalculatorFactory
    {
        IRevenueCalculator RevenueCalculator();
        IRevenueCalculator RevenueCalculatorMarketClosureCalculator();

        IRevenueCalculator RevenueCurrencyConvertingCalculator(DomainV2.Financial.Currency currency);
        IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(DomainV2.Financial.Currency currency);
    }
}