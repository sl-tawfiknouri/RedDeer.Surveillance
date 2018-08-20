using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IExchangeSeriesInitialiser
    {
        ExchangeFrame InitialFrame();
    }
}