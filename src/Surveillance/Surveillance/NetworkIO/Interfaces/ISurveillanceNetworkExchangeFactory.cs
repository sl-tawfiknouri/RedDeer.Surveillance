using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.NetworkIO.Interfaces
{
    public interface ISurveillanceNetworkExchangeFactory
    {
        NetworkExchange Create(ISurveillanceNetworkDuplexer networkDuplexer);
    }
}