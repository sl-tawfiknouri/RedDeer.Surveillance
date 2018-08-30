using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Surveillance.Network_IO;
using Surveillance.Network_IO.Interfaces;
using Surveillance.Recorders.Interfaces;
using Surveillance.Rules;
using Surveillance.Services.Interfaces;
using System;
using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.Services
{
    public class ReddeerTradeService : IReddeerTradeService
    {
        private INetworkExchange _tradeNetworkExchange;
        private INetworkExchange _equityNetworkExchange;
        private ISurveillanceNetworkExchangeFactory _networkExchangeFactory;
        private IUnsubscriberFactory<TradeOrderFrame> _unsubscriberFactory;
        private IUnsubscriberFactory<ExchangeFrame> _equityUnsubscriberFactory;
        private IRuleManager _ruleManager;
        private IRedDeerTradeRecorder _reddeerTradeRecorder;

        public ReddeerTradeService(
            ISurveillanceNetworkExchangeFactory networkExchangeFactory,
            IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory,
            IUnsubscriberFactory<ExchangeFrame> equityUnsubscriberFactory,
            IRuleManager ruleManager,
            IRedDeerTradeRecorder reddeerTradeRecorder)
        {
            _networkExchangeFactory = networkExchangeFactory ?? throw new ArgumentNullException(nameof(networkExchangeFactory));
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _equityUnsubscriberFactory = 
                equityUnsubscriberFactory 
                ?? throw new ArgumentNullException(nameof(equityUnsubscriberFactory));
            _ruleManager = ruleManager ?? throw new ArgumentNullException(nameof(ruleManager));
            _reddeerTradeRecorder = reddeerTradeRecorder ?? throw new ArgumentNullException(nameof(reddeerTradeRecorder));
        }

        public void Initialise()
        {
            var tradeStream = new TradeOrderStream<TradeOrderFrame>(_unsubscriberFactory);
            tradeStream.Subscribe(_reddeerTradeRecorder);

            var exchangeStream = new StockExchangeStream(_equityUnsubscriberFactory);
            
            var duplexer = new SurveillanceNetworkDuplexer(tradeStream, exchangeStream);

            _tradeNetworkExchange = _networkExchangeFactory.Create(duplexer);
            _tradeNetworkExchange.Initialise("ws://0.0.0.0:9069");

            // order stream only rules
            _ruleManager.RegisterTradingRules(tradeStream);           

            _equityNetworkExchange = _networkExchangeFactory.Create(duplexer);
            _equityNetworkExchange.Initialise("ws://0.0.0.0:9070");
            // equity stream only rules
            _ruleManager.RegisterEquityRules(exchangeStream);
        }

        private void HostEquityProcessingPipeline()
        {
        }

        public void Dispose()
        {
            if (_tradeNetworkExchange != null)
            {
                _tradeNetworkExchange.TerminateConnections();
            }

            if (_equityNetworkExchange != null)
            {
                _equityNetworkExchange.TerminateConnections();
            }
        }
    }
}
