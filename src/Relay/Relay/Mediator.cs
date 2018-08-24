using Domain.Equity.Trading;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Network_IO;
using Relay.Network_IO.RelaySubscribers;
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
        private IWebsocketHostFactory _websocketHostFactory;

        private ILogger _logger;
        private ILogger<TradeProcessor<TradeOrderFrame>> _tpLogger;
        private ILogger<NetworkExchange> _exchLogger;

        public Mediator(
            ITradeOrderStream<TradeOrderFrame> tradeOrderStream,
            ITradeRelaySubscriber tradeRelaySubscriber,
            IWebsocketHostFactory websocketHostFactory,
            ILogger<Mediator> logger,
            ILogger<TradeProcessor<TradeOrderFrame>> tpLogger,
            ILogger<NetworkExchange> exchLogger)
        {
            _tradeOrderStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
            _tradeRelaySubscriber = tradeRelaySubscriber ?? throw new ArgumentNullException(nameof(tradeRelaySubscriber));
            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tpLogger = tpLogger;
            _exchLogger = exchLogger;
        }

        public async Task Initiate()
        {
            _logger.LogInformation("Initiating relay in mediator");

            var unsubFactory = new UnsubscriberFactory<TradeOrderFrame>();
            var tradeProcessorOrderStream = new TradeOrderStream<TradeOrderFrame>(unsubFactory); // from trade processor TO relay
            var tradeProcessor = new TradeProcessor<TradeOrderFrame>(_tpLogger, tradeProcessorOrderStream);
            tradeProcessorOrderStream.Subscribe(_tradeRelaySubscriber);

            // hook the relay subscriber to begin comms with the outgoing network stream
            _tradeRelaySubscriber.Initiate("localhost", "9069");

            // hook the trade processor to receieve the incoming network stream
            _tradeOrderStream.Subscribe(tradeProcessor);

            // begin hosting connection for downstream processes (i.e. surveillance service)

            var networkDuplexer = new RelayNetworkDuplexer(_tradeOrderStream);
            var exchange = new NetworkExchange(_websocketHostFactory, networkDuplexer, _exchLogger);
            exchange.Initialise("ws://0.0.0.0:9067");

            _logger.LogInformation("Completed initiating relay in mediator");
        }
    }
}
