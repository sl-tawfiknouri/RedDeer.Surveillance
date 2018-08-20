namespace Relay.Network_IO
{
    public interface INetworkManager
    {
        void InitiateConnections();
        void TerminateConnections();
    }
}