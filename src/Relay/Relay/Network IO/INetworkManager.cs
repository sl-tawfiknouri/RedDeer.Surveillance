using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;

namespace Relay.Network_IO
{
    public interface INetworkManager
    {
        void InitiateConnections(ITradeOrderStream<TradeOrderFrame> tradeStream);
        void TerminateConnections();
    }
}