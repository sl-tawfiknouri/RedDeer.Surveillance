using Surveillance.Rules.HighProfits.Calculators.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces
{
    public interface IRevenueCalculatorFactory
    {
        IRevenueCalculator RevenueCalculator(Domain.Finance.Currency currency);
        IRevenueCalculator RevenueCurrencyConvertingCalculator(Domain.Finance.Currency currency);
    }
}