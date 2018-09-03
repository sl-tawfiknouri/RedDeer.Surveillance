using Microsoft.Extensions.Logging;
using Relay.Managers.Interfaces;
using Relay.Network_IO;
using Relay.Network_IO.RelaySubscribers.Interfaces;
using System;
using Domain.Streams;
using Domain.Trades.Orders;
using Domain.Trades.Streams;
using Domain.Trades.Streams.Interfaces;
using Relay.Configuration;
using Relay.Processors;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay.Managers
{
    public class TradeOrderStreamManager : ITradeOrderStreamManager
    {
        private readonly ITradeOrderStream<TradeOrderFrame> _tradeOrderStream;
        private readonly ITradeRelaySubscriber _tradeRelaySubscriber;
        private readonly IWebsocketHostFactory _websocketHostFactory;
        private readonly INetworkConfiguration _networkConfiguration;

        private readonly ILogger<TradeProcessor<TradeOrderFrame>> _tpLogger;
        private readonly ILogger<NetworkExchange> _exchangeLogger;

        public TradeOrderStreamManager(
            ITradeOrderStream<TradeOrderFrame> tradeOrderStream,
            ITradeRelaySubscriber tradeRelaySubscriber,
            IWebsocketHostFactory websocketHostFactory,
            INetworkConfiguration networkConfiguration,
            ILogger<TradeProcessor<TradeOrderFrame>> tpLogger,
            ILogger<NetworkExchange> exchangeLogger)
        {
            _tradeOrderStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
            _tradeRelaySubscriber = tradeRelaySubscriber ?? throw new ArgumentNullException(nameof(tradeRelaySubscriber));
            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));

            _networkConfiguration =
                networkConfiguration 
                ?? throw new ArgumentNullException(nameof(networkConfiguration));

            _tpLogger = tpLogger ?? throw new ArgumentNullException(nameof(tpLogger));
            _exchangeLogger = exchangeLogger ?? throw new ArgumentNullException(nameof(exchangeLogger));
        }

        public void Initialise()
        {
            var unsubscriberFactory = new UnsubscriberFactory<TradeOrderFrame>();
            var tradeProcessorOrderStream = new TradeOrderStream<TradeOrderFrame>(unsubscriberFactory);
            var tradeProcessor = new TradeProcessor<TradeOrderFrame>(_tpLogger, tradeProcessorOrderStream);
            tradeProcessorOrderStream.Subscribe(_tradeRelaySubscriber);

            // hook the relay subscriber to begin comms with the outgoing network stream
            _tradeRelaySubscriber.Initiate(
                _networkConfiguration.SurveillanceServiceTradeDomain,
                _networkConfiguration.SurveillanceServiceTradePort);

            // hook the trade processor to receieve the incoming network stream
            _tradeOrderStream.Subscribe(tradeProcessor);

            // begin hosting connection for downstream processes (i.e. surveillance service)
            var networkDuplexer = new RelayTradeNetworkDuplexer(_tradeOrderStream);
            var exchange = new NetworkExchange(_websocketHostFactory, networkDuplexer, _exchangeLogger);

            exchange.Initialise(
                $"ws://{_networkConfiguration.RelayServiceTradeDomain}:{_networkConfiguration.RelayServiceTradePort}");
        }
    }
}