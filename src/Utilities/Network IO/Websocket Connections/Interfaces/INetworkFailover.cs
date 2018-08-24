using System.Collections.Generic;

namespace Utilities.Network_IO.Websocket_Connections.Interfaces
{
    public interface INetworkFailover
    {
        bool HasData();
        void Store<T>(T value);
        void RemoveItem(System.Type key, object item);
        Dictionary<System.Type, List<object>> Retrieve();
        Dictionary<System.Type, List<object>> RetrieveAndRemove();
    }
}