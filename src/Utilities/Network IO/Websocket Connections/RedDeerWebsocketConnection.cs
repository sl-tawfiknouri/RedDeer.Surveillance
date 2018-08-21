using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using WebSocket4Net;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class RedDeerWebsocketConnection : WebSocket, IConnectionWebsocket
    {
        public RedDeerWebsocketConnection(
            string uri,
            string subProtocol,
            WebSocketVersion version) 
            : base(uri, subProtocol, version)
        {
        }

        public RedDeerWebsocketConnection(
            string uri,
            string subProtocol = "",
            List<KeyValuePair<string, string>> cookies = null,
            List<KeyValuePair<string, string>> customHeaderItems = null,
            string userAgent = "",
            string origin = "",
            WebSocketVersion version = WebSocketVersion.None,
            EndPoint httpConnectProxy = null,
            SslProtocols sslProtocols = SslProtocols.None,
            int receiveBufferSize = 0) 
            : base(
                  uri,
                  subProtocol,
                  cookies,
                  customHeaderItems,
                  userAgent,
                  origin,
                  version,
                  httpConnectProxy,
                  sslProtocols,
                  receiveBufferSize)
        {
        }


    }
}
