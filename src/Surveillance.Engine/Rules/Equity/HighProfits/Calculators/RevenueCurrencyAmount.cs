namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    using Domain.Core.Financial.Money;

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