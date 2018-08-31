namespace TestHarness.Network_IO.Subscribers.Interfaces
{
    public interface ITradeOrderWebsocketSubscriberFactory
    {
        ITradeOrderWebsocketSubscriber Build();
    }
}