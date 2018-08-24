using System.Threading;

namespace Utilities.Network_IO.Websocket_Connections.Interfaces
{
    public interface INetworkTrunk
    {
        bool Active { get; }
        bool Initiate(string domain, string port, CancellationToken token);
        bool Send<T>(T value);
        void Terminate();
    }
}