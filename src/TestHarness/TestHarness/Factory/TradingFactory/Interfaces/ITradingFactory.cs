namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFactory
    {
        ITradingFactoryHeartbeatOrMarketUpdateSelector Create();
    }
}