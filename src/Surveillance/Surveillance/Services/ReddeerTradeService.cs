using Domain.Equity.Trading;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Surveillance.Network_IO;
using Surveillance.Network_IO.Interfaces;
using Surveillance.Rules;
using Surveillance.Services.Interfaces;
using System;
using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.Services
{
    public class ReddeerTradeService : IReddeerTradeService
    {
        private INetworkExchange _tradeNetworkExchange;
        private ISurveillanceNetworkExchangeFactory _networkExchangeFactory;
        private IUnsubscriberFactory<TradeOrderFrame> _unsubscriberFactory;
        private IRuleManager _ruleManager;

        public ReddeerTradeService(
            ISurveillanceNetworkExchangeFactory networkExchangeFactory,
            IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory,
            IRuleManager ruleManager)
        {
            _networkExchangeFactory = networkExchangeFactory ?? throw new ArgumentNullException(nameof(networkExchangeFactory));
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _ruleManager = ruleManager ?? throw new ArgumentNullException(nameof(ruleManager));
        }

        public void Initialise()
        {
            HostTradeProcessingPipeline();
        }

        private void HostTradeProcessingPipeline()
        {
            var stream = new TradeOrderStream<TradeOrderFrame>(_unsubscriberFactory);
            var duplexer = new SurveillanceTradeNetworkDuplexer(stream);
            _tradeNetworkExchange = _networkExchangeFactory.Create(duplexer);
            _tradeNetworkExchange.Initialise("ws://0.0.0.0:9069");

            // order stream only rules
            _ruleManager.RegisterTradingRules(stream);
        }

        public void Dispose()
        {
            if (_tradeNetworkExchange != null)
            {
                _tradeNetworkExchange.TerminateConnections();
            }
        }
    }
}
