using System.Collections.Generic;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class NetworkFailoverLocalMemory<T> : INetworkFailover<T>
    {
        private List<T> _storedData;
        private object _lock = new object();

        public NetworkFailoverLocalMemory()
        {
            _storedData = new List<T>();
        }

        public void Store(T value)
        {
            lock (_lock)
            {
                if (value == null)
                {
                    return;
                }

                _storedData.Add(value);
            }
        }

        public IReadOnlyCollection<T> Retrieve()
        {
            lock (_lock)
            {
                return _storedData;
            }
        }

        public IReadOnlyCollection<T> RetrieveAndRemove()
        {
            lock (_lock)
            {
                var newStoredData = new List<T>(_storedData);
                _storedData = new List<T>();

                return newStoredData;
            }
        }
    }
}
