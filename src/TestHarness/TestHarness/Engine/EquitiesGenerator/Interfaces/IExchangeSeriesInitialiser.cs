namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    using Domain.Core.Markets.Collections;

    public interface IExchangeSeriesInitialiser
    {
        EquityIntraDayTimeBarCollection InitialFrame();
    }
}