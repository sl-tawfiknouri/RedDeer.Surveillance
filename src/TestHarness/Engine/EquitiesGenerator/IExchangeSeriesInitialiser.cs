using Domain.Equity.Trading;

namespace TestHarness.Engine.EquitiesGenerator
{
    public interface IExchangeSeriesInitialiser
    {
        ExchangeFrame InitialFrame();
    }
}