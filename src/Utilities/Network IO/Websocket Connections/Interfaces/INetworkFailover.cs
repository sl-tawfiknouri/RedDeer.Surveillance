using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

namespace Utilities.Network_IO.Websocket_Connections.Interfaces
{
    public interface INetworkFailOver
    {
        bool HasData();
        void Store<T>(T value);
        void RemoveItem(System.Type key, object item);
        Dictionary<System.Type, List<object>> Retrieve();
        Dictionary<System.Type, List<object>> RetrieveAndRemove();
    }
}