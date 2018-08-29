using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Equities;
using Relay.Network_IO;
using Relay.Network_IO.RelaySubscribers;
using Relay.Network_IO.RelaySubscribers.Interfaces;
using Relay.Trades;
using System;
using System.Threading.Tasks;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay
{
    public class Mediator : IMediator
    {
        private ITradeOrderStream<TradeOrderFrame> _tradeOrderStream;
        private ITradeRelaySubscriber _tradeRelaySubscriber;

        private IStockExchangeStream _stockExchangeStream;
        private IEquityRelaySubscriber _equityRelaySubscriber;

        private IWebsocketHostFactory _websocketHostFactory;

        private ILogger _logger;
        private ILogger<TradeProcessor<TradeOrderFrame>> _tpLogger;
        private ILogger<EquityProcessor> _epLogger;
        private ILogger<NetworkExchange> _exchLogger;

        public Mediator(
            ITradeOrderStream<TradeOrderFrame> tradeOrderStream,
            ITradeRelaySubscriber tradeRelaySubscriber,
            IStockExchangeStream stockExchangeStream,
            IEquityRelaySubscriber equityRelaySubscriber,
            IWebsocketHostFactory websocketHostFactory,
            ILogger<Mediator> logger,
            ILogger<TradeProcessor<TradeOrderFrame>> tpLogger,
            ILogger<EquityProcessor> epLogger,
            ILogger<NetworkExchange> exchLogger)
        {
            _tradeOrderStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
            _tradeRelaySubscriber = tradeRelaySubscriber ?? throw new ArgumentNullException(nameof(tradeRelaySubscriber));

            _stockExchangeStream = stockExchangeStream ?? throw new ArgumentNullException(nameof(stockExchangeStream));
            _equityRelaySubscriber = equityRelaySubscriber ?? throw new ArgumentNullException(nameof(equityRelaySubscriber));

            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tpLogger = tpLogger;
            _epLogger = epLogger;
            _exchLogger = exchLogger;
        }

        public async Task Initiate()
        {
            _logger.LogInformation("Initiating relay in mediator");

            CreateTradeOrderStreams();
            CreateStockExchangeStreams();

            _logger.LogInformation("Completed initiating relay in mediator");
        }

        private void CreateTradeOrderStreams()
        {
            var unsubFactory = new UnsubscriberFactory<TradeOrderFrame>();
            var tradeProcessorOrderStream = new TradeOrderStream<TradeOrderFrame>(unsubFactory); // from trade processor TO relay
            var tradeProcessor = new TradeProcessor<TradeOrderFrame>(_tpLogger, tradeProcessorOrderStream);
            tradeProcessorOrderStream.Subscribe(_tradeRelaySubscriber);

            // hook the relay subscriber to begin comms with the outgoing network stream
            _tradeRelaySubscriber.Initiate("localhost", "9069");

            // hook the trade processor to receieve the incoming network stream
            _tradeOrderStream.Subscribe(tradeProcessor);

            // begin hosting connection for downstream processes (i.e. surveillance service)

            var networkDuplexer = new RelayTradeNetworkDuplexer(_tradeOrderStream);
            var exchange = new NetworkExchange(_websocketHostFactory, networkDuplexer, _exchLogger);
            exchange.Initialise("ws://0.0.0.0:9067");
        }

        private void CreateStockExchangeStreams()
        {
            var unsubFactory = new UnsubscriberFactory<ExchangeFrame>();
            var stockExchangeStream = new StockExchangeStream(unsubFactory); // from stock processor TO relay
            var equityProcessor = new EquityProcessor(_epLogger, stockExchangeStream);
            stockExchangeStream.Subscribe(_equityRelaySubscriber);

            //inject stock relay subscriber
            _equityRelaySubscriber.Initiate("localhost", "9070");

            // hook the equity processor to receive the incoming network stream
            _stockExchangeStream.Subscribe(equityProcessor);

            // begin hosting connection for downstream processes (i.e. surveillance service)

            var networkDuplexer = new RelayEquityNetworkDuplexer(_stockExchangeStream);
            var exchange = new NetworkExchange(_websocketHostFactory, networkDuplexer, _exchLogger);
            exchange.Initialise("ws://0.0.0.0:9068");
        }
    }
}