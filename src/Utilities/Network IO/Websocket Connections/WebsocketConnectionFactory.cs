using System;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class WebsocketConnectionFactory : IWebsocketConnectionFactory
    {
        public IConnectionWebsocket Build(string connection)
        {
            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            return new RedDeerWebsocketConnection(connection);
        }
    }
}
