namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    using Domain.Core.Financial.Money;

    public class RevenueMoney
    {
        public RevenueMoney(bool hadMissingMarketData,
            Money? money,
            HighProfitComponents components)
        {
            this.HadMissingMarketData = hadMissingMarketData;
            this.Money = money;
            this.Components = components;
        }

        public bool HadMissingMarketData { get; }

        public Money? Money { get; }

        public HighProfitComponents Components { get; }
    }
}