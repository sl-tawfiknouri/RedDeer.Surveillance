using Domain.Equity.TimeBars;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IExchangeSeriesInitialiser
    {
        EquityIntraDayTimeBarCollection InitialFrame();
    }
}