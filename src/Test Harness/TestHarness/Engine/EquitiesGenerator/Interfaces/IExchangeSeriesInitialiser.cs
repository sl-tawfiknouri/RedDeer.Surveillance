using DomainV2.Equity.TimeBars;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IExchangeSeriesInitialiser
    {
        MarketTimeBarCollection InitialFrame();
    }
}