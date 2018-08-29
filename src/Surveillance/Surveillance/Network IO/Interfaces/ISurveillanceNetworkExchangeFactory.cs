using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Surveillance.Network_IO.Interfaces
{
    public interface ISurveillanceNetworkExchangeFactory
    {
        NetworkExchange Create(INetworkDuplexer networkDuplexer);
    }
}