using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.Network_IO.Interfaces
{
    public interface ISurveillanceNetworkExchangeFactory
    {
        NetworkExchange Create(ISurveillanceNetworkDuplexer networkDuplexer);
    }
}