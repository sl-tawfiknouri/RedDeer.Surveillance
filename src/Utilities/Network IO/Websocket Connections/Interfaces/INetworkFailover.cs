using System.Collections.Generic;

namespace Utilities.Network_IO.Websocket_Connections.Interfaces
{
    public interface INetworkFailover
    {
        void Store<T>(T value);
        Dictionary<System.Type, List<object>> Retrieve();
        Dictionary<System.Type, List<object>> RetrieveAndRemove();
    }
}