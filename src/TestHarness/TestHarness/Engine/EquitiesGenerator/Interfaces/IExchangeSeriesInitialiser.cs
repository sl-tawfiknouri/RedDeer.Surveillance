using Domain.Core.Markets.Collections;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IExchangeSeriesInitialiser
    {
        EquityIntraDayTimeBarCollection InitialFrame();
    }
}