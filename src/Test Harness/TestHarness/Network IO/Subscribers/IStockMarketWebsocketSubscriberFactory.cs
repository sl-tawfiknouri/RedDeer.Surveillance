namespace TestHarness.Network_IO.Subscribers
{
    public interface IStockMarketWebsocketSubscriberFactory
    {
        IStockMarketWebsocketSubscriber Build();
    }
}