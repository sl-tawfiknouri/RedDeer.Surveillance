using Relay.Network_IO;
using StructureMap;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Domain.Equity.Trading.Streams.Interfaces;
using Domain.Equity.Trading;
using Relay.Trades;
using Relay.Network_IO.RelaySubscribers;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Relay;

namespace RedDeer.Relay
{
    public class RelayRegistry : Registry
    {
        public RelayRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMediator>().Use<Mediator>();
            For<INetworkManager>().Use<NetworkManager>();
            For<ITradeOrderStream>().Use<TradeOrderStream>();
            For<ITradeProcessor>().Use<TradeProcessor>();
            For<ITradeRelaySubscriber>().Use<TradeRelaySubscriber>();
            For<IWebsocketConnectionFactory>().Use<WebsocketConnectionFactory>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));
        }
    }
}
