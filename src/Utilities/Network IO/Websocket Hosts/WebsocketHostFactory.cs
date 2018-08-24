using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Utilities.Network_IO.Websocket_Hosts
{
    public class WebsocketHostFactory : IWebsocketHostFactory
    {
        public IWebsocketHost Build(string location)
        {
            if (location == null)
                location = string.Empty;

            return new RedDeerWebsocketHost(location);
        }
    }
}
