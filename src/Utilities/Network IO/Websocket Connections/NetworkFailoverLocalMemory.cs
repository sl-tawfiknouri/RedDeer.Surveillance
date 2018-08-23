using System.Collections.Generic;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class NetworkFailoverLocalMemory : INetworkFailover
    {
        private Dictionary<System.Type, List<object>> _dict;
        private object _lock = new object();

        public NetworkFailoverLocalMemory()
        {
            _dict = new Dictionary<System.Type, List<object>>();
        }

        public void Store<T>(T value)
        {
            lock (_lock)
            {
                if (value == null)
                {
                    return;
                }

                _dict.TryGetValue(typeof(T), out List<object> _itemList);

                if (_itemList == null)
                {
                    _itemList = new List<object>() { value };
                    _dict.Add(typeof(T), _itemList);
                }
                else
                {
                    _itemList.Add(value);
                }
            }
        }

        public Dictionary<System.Type, List<object>> Retrieve()
        {
            lock (_lock)
            {
                return new Dictionary<System.Type, List<object>>(_dict);
            }
        }

        public Dictionary<System.Type, List<object>> RetrieveAndRemove()
        {
            lock (_lock)
            {
                var newStoredData = new Dictionary<System.Type, List<object>>(_dict);
                _dict = new Dictionary<System.Type, List<object>>();

                return newStoredData;
            }
        }
    }
}