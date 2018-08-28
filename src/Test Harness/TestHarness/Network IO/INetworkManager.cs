using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;

namespace TestHarness.Network_IO
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