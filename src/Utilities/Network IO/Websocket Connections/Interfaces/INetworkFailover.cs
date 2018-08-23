using System.Collections.Generic;

namespace Utilities.Network_IO.Websocket_Connections.Interfaces
{
    public interface INetworkFailover<T>
    {
        void Store(T value);
        IReadOnlyCollection<T> Retrieve();
        IReadOnlyCollection<T> RetrieveAndRemove();
    }
}