using System.Collections.Generic;
using System.Linq;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class NetworkFailOverLocalMemory : INetworkFailOver
    {
        private readonly object _lock = new object();
        private Dictionary<System.Type, List<object>> _dict;

        public NetworkFailOverLocalMemory()
        {
            _dict = new Dictionary<System.Type, List<object>>();
        }

        public bool HasData()
        {
            return 
                Retrieve()
                ?.Any(ret => ret.Value != null && ret.Value.Any())
                ?? false;
        }

        public void Store<T>(T value)
        {
            lock (_lock)
            {
                if (value == null)
                {
                    return;
                }

                _dict.TryGetValue(typeof(T), out var itemList);

                if (itemList == null)
                {
                    itemList = new List<object>() { value };
                    _dict.Add(typeof(T), itemList);
                }
                else
                {
                    itemList.Add(value);
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

        public void RemoveItem(System.Type key, object item)
        {
            if (item == null)
            {
                return;
            }

            lock (_lock)
            {
                _dict.TryGetValue(key, out var listWithRemovedItem);

                listWithRemovedItem?.RemoveAll(listElement => listElement == item);
            }
        }
    }
}