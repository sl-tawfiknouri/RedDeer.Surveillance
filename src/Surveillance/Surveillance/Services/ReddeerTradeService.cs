﻿using Surveillance.Recorders.Interfaces;
using Surveillance.Services.Interfaces;
using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams;
using Surveillance.Configuration.Interfaces;
using Surveillance.NetworkIO;
using Surveillance.NetworkIO.Interfaces;
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
        private readonly IRedDeerAuroraTradeRecorderAutoSchedule _reddeerTradeRecorder;
        private readonly IRedDeerStockExchangeRecorder _reddeerStockExchangeRecorder;
        private readonly INetworkConfiguration _configuration;

        public ReddeerTradeService(
            ISurveillanceNetworkExchangeFactory networkExchangeFactory,
            IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory,
            IUnsubscriberFactory<ExchangeFrame> equityUnsubscriberFactory,
            IRedDeerAuroraTradeRecorderAutoSchedule reddeerTradeRecorder,
            IRedDeerStockExchangeRecorder reddeerStockExchangeRecorder,
            INetworkConfiguration configuration)
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
            _reddeerTradeRecorder = 
                reddeerTradeRecorder 
                ?? throw new ArgumentNullException(nameof(reddeerTradeRecorder));
            _reddeerStockExchangeRecorder = 
                reddeerStockExchangeRecorder 
                ?? throw new ArgumentNullException(nameof(reddeerStockExchangeRecorder));
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Initialise()
        {
            var tradeStream = new TradeOrderStream<TradeOrderFrame>(_unsubscriberFactory);
            tradeStream.Subscribe(_reddeerTradeRecorder);

            var exchangeStream = new StockExchangeStream(_equityUnsubscriberFactory);
            exchangeStream.Subscribe(_reddeerStockExchangeRecorder);

            var duplexer = new SurveillanceNetworkDuplexer(tradeStream, exchangeStream);

            // T R A D E S
            _tradeNetworkExchange = _networkExchangeFactory.Create(duplexer);

            _tradeNetworkExchange.Initialise(
                $"ws://{_configuration.SurveillanceServiceTradeDomain}:{_configuration.SurveillanceServiceTradePort}");

            // E Q U I T Y
            _equityNetworkExchange = _networkExchangeFactory.Create(duplexer);
            _equityNetworkExchange.Initialise(
                $"ws://{_configuration.SurveillanceServiceEquityDomain}:{_configuration.SurveillanceServiceEquityPort}");
        }

        public void Dispose()
        {
            _tradeNetworkExchange?.TerminateConnections();
            _equityNetworkExchange?.TerminateConnections();
        }
    }
}
