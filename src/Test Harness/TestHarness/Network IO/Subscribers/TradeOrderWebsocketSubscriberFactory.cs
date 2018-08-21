using NLog;
using System;
using Utilities.Network_IO.Websocket_Connections;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriberFactory : ITradeOrderWebsocketSubscriberFactory
    {
        private IWebsocketConnectionFactory _websocketFactory;
        private ILogger _logger;

        public TradeOrderWebsocketSubscriberFactory(IWebsocketConnectionFactory websocketFactory, ILogger logger)
        {
            _websocketFactory = websocketFactory ?? throw new ArgumentNullException(nameof(websocketFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITradeOrderWebsocketSubscriber Build()
        {
            return new TradeOrderWebsocketSubscriber(_websocketFactory, _logger);
        }
    }
}