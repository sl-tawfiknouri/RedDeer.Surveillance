using NLog;
using System;
using TestHarness.Display;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriberFactory : ITradeOrderWebsocketSubscriberFactory
    {
        private IWebsocketConnectionFactory _websocketFactory;
        private IConsole _console;
        private ILogger _logger;

        public TradeOrderWebsocketSubscriberFactory(
            IWebsocketConnectionFactory websocketFactory,
            IConsole console,
            ILogger logger)
        {
            _websocketFactory = websocketFactory ?? throw new ArgumentNullException(nameof(websocketFactory));
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITradeOrderWebsocketSubscriber Build()
        {
            var networkTrunk = new NetworkTrunk(_websocketFactory, _console);
            var networkFailover = new NetworkFailoverLocalMemory();
            var networkSwitch = new NetworkSwitch(networkTrunk, networkFailover);
            var networkDuplexer = new DuplexMessageFactory();

            return new TradeOrderWebsocketSubscriber(networkSwitch, networkDuplexer, _logger);
        }
    }
}