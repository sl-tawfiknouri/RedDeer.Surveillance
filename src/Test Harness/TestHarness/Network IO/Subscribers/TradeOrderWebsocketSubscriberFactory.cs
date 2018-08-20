using NLog;
using System;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriberFactory : ITradeOrderWebsocketSubscriberFactory
    {
        private IWebsocketFactory _websocketFactory;
        private ILogger _logger;

        public TradeOrderWebsocketSubscriberFactory(IWebsocketFactory websocketFactory, ILogger logger)
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