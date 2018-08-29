﻿using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Surveillance.Network_IO;
using Surveillance.Network_IO.Interfaces;
using Surveillance.Rules;
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

        public ReddeerTradeService(
            ISurveillanceNetworkExchangeFactory networkExchangeFactory,
            IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory,
            IUnsubscriberFactory<ExchangeFrame> equityUnsubscriberFactory,
            IRuleManager ruleManager)
        {
            _networkExchangeFactory = networkExchangeFactory ?? throw new ArgumentNullException(nameof(networkExchangeFactory));
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _equityUnsubscriberFactory = 
                equityUnsubscriberFactory 
                ?? throw new ArgumentNullException(nameof(equityUnsubscriberFactory));
            _ruleManager = ruleManager ?? throw new ArgumentNullException(nameof(ruleManager));
        }

        public void Initialise()
        {
            HostTradeProcessingPipeline();
            HostEquityProcessingPipeline();
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

        private void HostEquityProcessingPipeline()
        {
            var stream = new StockExchangeStream(_equityUnsubscriberFactory);
            var duplexer = new SurveillanceEquityNetworkDuplexer(stream);
            _equityNetworkExchange = _networkExchangeFactory.Create(duplexer);
            _equityNetworkExchange.Initialise("ws://0.0.0.0:9070");

            // equity stream only rules
            _ruleManager.RegisterEquityRules(stream);
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
