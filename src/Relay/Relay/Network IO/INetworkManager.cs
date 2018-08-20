using Domain.Equity.Trading.Streams.Interfaces;

namespace Relay.Network_IO
{
    public interface INetworkManager
    {
        void InitiateConnections(ITradeOrderStream tradeStream);
        void TerminateConnections();
    }
}