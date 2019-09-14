using Domain.Core.Financial.Money;

namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators
{
    public class RevenueMoney
    {
        public RevenueMoney(bool hadMissingMarketData, Money? money)
        {
            this.HadMissingMarketData = hadMissingMarketData;
            this.Money = money;
        }

        public bool HadMissingMarketData { get; }

        public Money? Money { get; }
    }
}