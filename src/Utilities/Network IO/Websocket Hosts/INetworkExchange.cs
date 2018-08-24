namespace Utilities.Network_IO.Websocket_Hosts
{
    public interface INetworkExchange
    {
        void Initialise(string hostUrl);
        void TerminateConnections();
    }
}