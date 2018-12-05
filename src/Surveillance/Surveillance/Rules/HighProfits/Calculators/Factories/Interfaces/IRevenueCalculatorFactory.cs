using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces
{
    public interface IRevenueCalculatorFactory
    {
        IRevenueCalculator RevenueCalculator();
        IRevenueCalculator RevenueCalculatorMarkingTheClose();

        IRevenueCalculator RevenueCurrencyConvertingCalculator(DomainV2.Financial.Currency currency);
        IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(DomainV2.Financial.Currency currency);
    }
}