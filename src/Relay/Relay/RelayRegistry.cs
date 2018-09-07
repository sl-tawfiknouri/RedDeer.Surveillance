using Domain.Equity.Frames;
using Domain.Equity.Streams;
using Domain.Equity.Streams.Interfaces;
using Domain.Streams;
using Domain.Trades.Streams;
using Domain.Trades.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Relay.Interfaces;
using Relay.Managers;
using Relay.Managers.Interfaces;
using Relay.Network_IO;
using Relay.Network_IO.Interfaces;
using Relay.Network_IO.RelaySubscribers;
using Relay.Network_IO.RelaySubscribers.Interfaces;
using Relay.Processors;
using Relay.Processors.Interfaces;
using StructureMap;
using Utilities.Network_IO.Interfaces;
using Utilities.Network_IO.Websocket_Connections;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay
{
    public class RelayRegistry : Registry
    {
        public RelayRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMediator>().Use<Mediator>();

            For(typeof(ITradeProcessor<>)).Use(typeof(TradeProcessor<>));
            For<IEquityProcessor<ExchangeFrame>>().Use<EquityProcessor>();

            For(typeof(ITradeOrderStream<>)).Use(typeof(TradeOrderStream<>));
            For<IStockExchangeStream>().Use<StockExchangeStream>();

            For<ITradeRelaySubscriber>().Use<TradeRelaySubscriber>();
            For<IEquityRelaySubscriber>().Use<EquityRelaySubscriber>();

            For<IWebsocketConnectionFactory>().Use<WebsocketConnectionFactory>();
            For<IMessageWriter>().Use<LoggingMessageWriter>();

            For<INetworkTrunk>().Use<NetworkTrunk>();
            For<INetworkFailover>().Use<NetworkFailoverLocalMemory>();
            For<INetworkSwitch>().Use<NetworkSwitch>();

            For<INetworkExchange>().Use<NetworkExchange>();
            For<IRelayTradeNetworkDuplexer>().Use<RelayTradeNetworkDuplexer>();
            For<IRelayEquityNetworkDuplexer>().Use<RelayEquityNetworkDuplexer>();

            For<ITradeOrderStreamManager>().Use<TradeOrderStreamManager>();
            For<IStockExchangeStreamManager>().Use<StockExchangeStreamManager>();

            For<IDuplexMessageFactory>().Use<DuplexMessageFactory>();
            For<IWebsocketHostFactory>().Use<WebsocketHostFactory>();
            For<IWebsocketHost>().Use<RedDeerWebsocketHost>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));
        }
    }
}
