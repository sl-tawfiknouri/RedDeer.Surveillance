using Fleck;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Utilities.Network_IO.Websocket_Hosts
{
    public class RedDeerWebsocketHost : WebSocketServer, IWebsocketHost
    {
        public RedDeerWebsocketHost(string location) 
            : base(location)
        {
        }
    }
}
