using System;

namespace Utilities.Websockets
{
    public class WebsocketFactory : IWebsocketFactory
    {
        public IWebsocket Build(string connection)
        {
            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            return new RedDeerWebsocket(connection);
        }
    }
}
