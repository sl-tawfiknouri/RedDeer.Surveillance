namespace Utilities.Network_IO.Websocket_Hosts.Interfaces
{
    public interface INetworkExchange
    {
        void Initialise(string hostUrl);
        void TerminateConnections();
    }
}