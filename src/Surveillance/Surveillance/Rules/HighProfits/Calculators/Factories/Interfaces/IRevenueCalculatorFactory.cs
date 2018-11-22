using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces
{
    public interface IRevenueCalculatorFactory
    {
        IRevenueCalculator RevenueCalculator();
        IRevenueCalculator RevenueCalculatorMarkingTheClose();

        IRevenueCalculator RevenueCurrencyConvertingCalculator(Domain.Finance.Currency currency);
        IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(Domain.Finance.Currency currency);
    }
}