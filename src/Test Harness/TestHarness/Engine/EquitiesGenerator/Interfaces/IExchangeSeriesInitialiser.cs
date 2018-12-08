using DomainV2.Equity.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IExchangeSeriesInitialiser
    {
        ExchangeFrame InitialFrame();
    }
}