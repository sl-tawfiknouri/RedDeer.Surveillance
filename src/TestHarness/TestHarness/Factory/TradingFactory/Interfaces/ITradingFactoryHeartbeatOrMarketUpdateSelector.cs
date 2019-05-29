namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFactoryHeartbeatOrMarketUpdateSelector
    {
        ITradingFactoryHeartbeatSelector Heartbeat();
        ITradingFactoryVolumeStrategySelector MarketUpdate();
    }
}
