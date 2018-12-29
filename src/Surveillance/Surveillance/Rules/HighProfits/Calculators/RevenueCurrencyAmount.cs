using DomainV2.Financial;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueCurrencyAmount
    {
        public RevenueCurrencyAmount(
            bool hadMissingMarketData,
            CurrencyAmount? currencyAmount)
        {
            HadMissingMarketData = hadMissingMarketData;
            CurrencyAmount = currencyAmount;
        }

        public bool HadMissingMarketData { get; }
        public CurrencyAmount? CurrencyAmount { get; }
    }
}
