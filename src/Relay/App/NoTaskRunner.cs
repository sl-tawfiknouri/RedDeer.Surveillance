using Relay.Network_IO;
using System;
using System.Threading.Tasks;

namespace RedDeer.Relay.App
{
    public class WebSocketRunner : IStartUpTaskRunner
    {
        private INetworkManager _networkManager;

        public WebSocketRunner(INetworkManager networkManager)
        {
            _networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
        }

        public async Task Run()
        {
            await Task.Run(() => {
                _networkManager.InitiateConnections();
            });
        }
    }
}
