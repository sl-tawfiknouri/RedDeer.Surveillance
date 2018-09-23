using NLog;
using System;
using TestHarness.Display;
using TestHarness.Network_IO.Subscribers.Interfaces;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;

namespace TestHarness.Network_IO.Subscribers
{
    public class StockMarketWebsocketSubscriberFactory : IStockMarketWebsocketSubscriberFactory
    {
        private readonly IWebsocketConnectionFactory _websocketFactory;
        private readonly IConsole _console;
        private readonly ILogger _logger;

        public StockMarketWebsocketSubscriberFactory(
             IWebsocketConnectionFactory websocketFactory,
             IConsole console,
             ILogger logger)
        {
            _websocketFactory = websocketFactory ?? throw new ArgumentNullException(nameof(websocketFactory));
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IStockMarketWebsocketSubscriber Build()
        {
            var networkTrunk = new NetworkTrunk(_websocketFactory, _console);
            var networkFailover = new NetworkFailOverLocalMemory();
            var networkSwitch = new NetworkSwitch(networkTrunk, networkFailover);
            var networkDuplexer = new DuplexMessageFactory();

            return new StockMarketWebsocketSubscriber(networkSwitch, networkDuplexer, _logger);
        }
    }
}
