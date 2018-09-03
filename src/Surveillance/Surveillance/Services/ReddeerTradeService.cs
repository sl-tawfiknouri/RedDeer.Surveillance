using Surveillance.Network_IO;
using Surveillance.Network_IO.Interfaces;
using Surveillance.Recorders.Interfaces;
using Surveillance.Rules.Interfaces;
using Surveillance.Services.Interfaces;
using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Surveillance.Services
{
    public class ReddeerTradeService : IReddeerTradeService
    {
        private INetworkExchange _tradeNetworkExchange;
        private INetworkExchange _equityNetworkExchange;
        private readonly ISurveillanceNetworkExchangeFactory _networkExchangeFactory;
        private readonly IUnsubscriberFactory<TradeOrderFrame> _unsubscriberFactory;
        private readonly IUnsubscriberFactory<ExchangeFrame> _equityUnsubscriberFactory;
        private readonly IRuleManager _ruleManager;
        private readonly IRedDeerTradeRecorder _reddeerTradeRecorder;
        private readonly IRedDeerStockExchangeRecorder _reddeerStockExchangeRecorder;

        public ReddeerTradeService(
            ISurveillanceNetworkExchangeFactory networkExchangeFactory,
            IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory,
            IUnsubscriberFactory<ExchangeFrame> equityUnsubscriberFactory,
            IRuleManager ruleManager,
            IRedDeerTradeRecorder reddeerTradeRecorder,
            IRedDeerStockExchangeRecorder reddeerStockExchangeRecorder)
        {
            _networkExchangeFactory = 
                networkExchangeFactory 
                ?? throw new ArgumentNullException(nameof(networkExchangeFactory));
            _unsubscriberFactory = 
                unsubscriberFactory 
                ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _equityUnsubscriberFactory = 
                equityUnsubscriberFactory 
                ?? throw new ArgumentNullException(nameof(equityUnsubscriberFactory));
            _ruleManager = ruleManager ?? throw new ArgumentNullException(nameof(ruleManager));
            _reddeerTradeRecorder = 
                reddeerTradeRecorder 
                ?? throw new ArgumentNullException(nameof(reddeerTradeRecorder));
            _reddeerStockExchangeRecorder = 
                reddeerStockExchangeRecorder 
                ?? throw new ArgumentNullException(nameof(reddeerStockExchangeRecorder));
        }

        public void Initialise()
        {
            var tradeStream = new TradeOrderStream<TradeOrderFrame>(_unsubscriberFactory);
            tradeStream.Subscribe(_reddeerTradeRecorder);

            var exchangeStream = new StockExchangeStream(_equityUnsubscriberFactory);
            exchangeStream.Subscribe(_reddeerStockExchangeRecorder);

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
