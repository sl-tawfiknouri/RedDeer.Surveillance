using System;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class NetworkSwitch
    {
        private INetworkTrunk _trunk;
        private INetworkFailover _failover;

        public NetworkSwitch(INetworkTrunk trunk, INetworkFailover failover)
        {
            _trunk = trunk ?? throw new ArgumentNullException(nameof(trunk));
            _failover = failover ?? throw new ArgumentNullException(nameof(failover));
        }

        public void Add<T>(T value)
        {
            
        }
    }
}
