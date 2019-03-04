namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    public class RevenueMoney
    {
        public RevenueMoney(
            bool hadMissingMarketData,
            Money? Money)
        {
            HadMissingMarketData = hadMissingMarketData;
            Money = Money;
        }

        public bool HadMissingMarketData { get; }
        public Money? Money { get; }
    }
}
