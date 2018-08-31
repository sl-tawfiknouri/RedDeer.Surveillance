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
using Domain.Equity.Trading.Frames;
using Relay.Equities;
using Relay.Network_IO.Interfaces;
using Relay.Network_IO.RelaySubscribers.Interfaces;
using Relay.Processors.Interfaces;
using Relay.Managers.Interfaces;
using Relay.Managers;

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
