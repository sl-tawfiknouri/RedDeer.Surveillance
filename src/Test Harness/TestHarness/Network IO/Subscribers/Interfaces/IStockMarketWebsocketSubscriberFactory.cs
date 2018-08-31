namespace TestHarness.Network_IO.Subscribers.Interfaces
{
    public interface IStockMarketWebsocketSubscriberFactory
    {
        IStockMarketWebsocketSubscriber Build();
    }
}