using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces
{
    public interface IRevenueCalculatorFactory
    {
        IRevenueCalculator RevenueCalculator();

        IRevenueCalculator RevenueCalculatorMarketClosureCalculator();

        IRevenueCalculator RevenueCurrencyConvertingCalculator(Domain.Core.Financial.Money.Currency currency);

        IRevenueCalculator RevenueCurrencyConvertingMarketClosureCalculator(Domain.Core.Financial.Money.Currency currency);
    }
}