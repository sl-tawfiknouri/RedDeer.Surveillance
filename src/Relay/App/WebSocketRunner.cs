using Domain.Equity.Trading.Streams.Interfaces;
using Relay.Network_IO;
using Relay.Network_IO.RelaySubscribers;
using Relay.Trades;
using System;
using System.Threading.Tasks;

namespace RedDeer.Relay.App
{
    public class WebSocketRunner : IStartUpTaskRunner
    {
        private INetworkManager _networkManager;
        private ITradeOrderStream _tradeOrderStream;
        private ITradeProcessor _tradeProcessor;
        private ITradeRelaySubscriber _tradeRelaySubscriber;

        public WebSocketRunner(
            INetworkManager networkManager,
            ITradeOrderStream tradeOrderStream,
            ITradeProcessor tradeProcessor,
            ITradeRelaySubscriber tradeRelaySubscriber)
        {
            _networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
            _tradeOrderStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
            _tradeProcessor = tradeProcessor ?? throw new ArgumentNullException(nameof(tradeProcessor));
            _tradeRelaySubscriber = tradeRelaySubscriber ?? throw new ArgumentNullException(nameof(tradeRelaySubscriber));
        }

        public async Task Run()
        {
            await Task.Run(() => 
            {
                // attach internal pub-sub trade processor to initial stream
                _tradeOrderStream.Subscribe(_tradeProcessor);
                // fire up trade data relay
                _tradeRelaySubscriber.Initiate("localhost", "9069");
                // attach trade data relay to the internal trade processor to pass data onto surveillance service
                _tradeProcessor.Subscribe(_tradeRelaySubscriber);

                _networkManager.InitiateConnections(_tradeOrderStream);
            });
        }
    }
}
