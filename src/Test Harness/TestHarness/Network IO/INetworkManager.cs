using Domain.Equity.Trading.Streams.Interfaces;

namespace TestHarness.Network_IO
{
    public interface INetworkManager
    {
        bool InitiateNetworkConnections();
        void TerminateNetworkConnections();
        bool AttachTradeOrderSubscriberToStream(ITradeOrderStream orderStream);
        void DetatchTradeOrderSubscriber();
    }
}