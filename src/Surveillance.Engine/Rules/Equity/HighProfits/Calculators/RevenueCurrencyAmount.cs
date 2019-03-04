using Domain.Core.Financial;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    public class RevenueMoney
    {
        public RevenueMoney(
            bool hadMissingMarketData,
            Money? money)
        {
            HadMissingMarketData = hadMissingMarketData;
            Money = money;
        }

        public bool HadMissingMarketData { get; }
        public Money? Money { get; }
    }
}
