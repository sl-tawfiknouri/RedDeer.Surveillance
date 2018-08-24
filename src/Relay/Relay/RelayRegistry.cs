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
using Utilities.Network_IO.Interfaces;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;

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
            For(typeof(ITradeOrderStream<>)).Use(typeof(TradeOrderStream<>));
            For(typeof(ITradeProcessor<>)).Use(typeof(TradeProcessor<>));
            For<ITradeRelaySubscriber>().Use<TradeRelaySubscriber>();
            For<IWebsocketConnectionFactory>().Use<WebsocketConnectionFactory>();
            For<IMessageWriter>().Use<LoggingMessageWriter>();
            For<INetworkTrunk>().Use<NetworkTrunk>();
            For<INetworkFailover>().Use<NetworkFailoverLocalMemory>();
            For<INetworkSwitch>().Use<NetworkSwitch>();

            For<INetworkExchange>().Use<NetworkExchange>();
            For<INetworkDuplexer>().Use<RelayNetworkDuplexer>();
            For<IWebsocketHostFactory>().Use<WebsocketHostFactory>();
            For<IWebsocketHost>().Use<RedDeerWebsocketHost>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));
        }
    }
}
