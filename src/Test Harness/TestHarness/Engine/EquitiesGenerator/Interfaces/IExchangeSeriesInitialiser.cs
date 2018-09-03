using Domain.Equity.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IExchangeSeriesInitialiser
    {
        ExchangeFrame InitialFrame();
    }
}