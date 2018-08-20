using Domain.Equity.Trading.Streams.Interfaces;

namespace TestHarness.Network_IO
{
    public interface INetworkManager
    {
        void AttachTradeOrderSubscriberToStream(ITradeOrderStream orderStream);
        void DetatchTradeOrderSubscriber();
        void InitiateNetworkConnections();
        void TerminateNetworkConnections();
    }
}