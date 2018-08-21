using Fleck;

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
