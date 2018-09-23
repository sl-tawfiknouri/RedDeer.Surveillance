using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace TestHarness.Network_IO.Interfaces
{
    public interface INetworkManager
    {
        bool InitiateAllNetworkConnections();
        void TerminateAllNetworkConnections();
        bool AttachTradeOrderSubscriberToStream(ITradeOrderStream<TradeOrderFrame> orderStream);
        void DetatchTradeOrderSubscriber();
        bool AttachStockExchangeSubscriberToStream(IStockExchangeStream exchangeStream);
        void DetatchStockExchangeSubscriber();
    }
}