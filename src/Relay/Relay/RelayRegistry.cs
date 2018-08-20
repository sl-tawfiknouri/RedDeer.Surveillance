using Relay.Network_IO;
using StructureMap;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Domain.Equity.Trading.Streams.Interfaces;
using Domain.Equity.Trading;
using Relay.Trades;
using Relay.Network_IO.RelaySubscribers;
using Utilities.Websockets;

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
            For<ITradeOrderStream>().Use<TradeOrderStream>();
            For<ITradeProcessor>().Use<TradeProcessor>();
            For<ITradeRelaySubscriber>().Use<TradeRelaySubscriber>();
            For<IWebsocketFactory>().Use<WebsocketFactory>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));
        }
    }
}
