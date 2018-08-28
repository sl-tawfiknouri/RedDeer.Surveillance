using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Surveillance.Network_IO
{
    public interface ISurveillanceNetworkExchangeFactory
    {
        NetworkExchange Create(INetworkDuplexer networkDuplexer);
    }
}