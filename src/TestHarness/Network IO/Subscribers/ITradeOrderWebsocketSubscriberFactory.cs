namespace TestHarness.Network_IO.Subscribers
{
    public interface ITradeOrderWebsocketSubscriberFactory
    {
        ITradeOrderWebsocketSubscriber Build();
    }
}