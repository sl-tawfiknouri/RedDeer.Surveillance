using Relay.Network_IO;
using StructureMap;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;

namespace RedDeer.Relay
{
    public class RelayRegistry : Registry
    {
        public RelayRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<INetworkManager>().Use<NetworkManager>();
        }
    }
}
