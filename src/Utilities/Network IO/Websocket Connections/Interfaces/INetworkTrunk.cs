using System.Threading;

namespace Utilities.Network_IO.Websocket_Connections.Interfaces
{
    public interface INetworkTrunk
    {
        bool Initiate(string domain, string port, CancellationToken token);
        void Send<T>(T value);
        void Terminate();
    }
}