namespace Utilities.Network_IO.Websocket_Hosts.Interfaces
{
    public interface INetworkDuplexer
    {
        void Transmit(IDuplexedMessage message);
    }
}
