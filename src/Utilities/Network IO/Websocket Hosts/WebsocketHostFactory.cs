using System;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Utilities.Network_IO.Websocket_Hosts
{
    public class WebsocketHostFactory : IWebsocketHostFactory
    {
        public IWebsocketHost Build(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException(nameof(location)); // need to be a valid URI

            return new RedDeerWebsocketHost(location);
        }
    }
}
