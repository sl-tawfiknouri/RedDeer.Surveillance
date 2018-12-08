using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace TestHarness.Network_IO.Interfaces
{
    public interface INetworkManager
    {
        bool InitiateAllNetworkConnections();
        void TerminateAllNetworkConnections();
        bool AttachTradeOrderSubscriberToStream(IOrderStream<Order> orderStream);
        void DetachTradeOrderSubscriber();
        bool AttachStockExchangeSubscriberToStream(IStockExchangeStream exchangeStream);
        void DetachStockExchangeSubscriber();
    }
}