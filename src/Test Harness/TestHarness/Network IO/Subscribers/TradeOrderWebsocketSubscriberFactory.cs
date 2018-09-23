using NLog;
using System;
using TestHarness.Display;
using TestHarness.Network_IO.Subscribers.Interfaces;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriberFactory : ITradeOrderWebsocketSubscriberFactory
    {
        private readonly IWebsocketConnectionFactory _websocketFactory;
        private readonly IConsole _console;
        private readonly ILogger _logger;

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
            var networkFailOver = new NetworkFailOverLocalMemory();
            var networkSwitch = new NetworkSwitch(networkTrunk, networkFailOver);
            var networkDuplexer = new DuplexMessageFactory();

            return new TradeOrderWebsocketSubscriber(networkSwitch, networkDuplexer, _logger);
        }
    }
}