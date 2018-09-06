using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using NLog;
using TestHarness.Configuration.Interfaces;
using TestHarness.Network_IO.Interfaces;
using TestHarness.Network_IO.Subscribers.Interfaces;

namespace TestHarness.Network_IO
{
    public class NetworkManager : INetworkManager
    {
        private readonly object _stateTransition = new object();

        private readonly INetworkConfiguration _networkConfiguration;
        private readonly ITradeOrderWebsocketSubscriberFactory _tradeOrderSocketSubscriberFactory;
        private ITradeOrderWebsocketSubscriber _tradeOrderWebsocketSubscriber;
        private readonly IStockMarketWebsocketSubscriberFactory _stockMarketSocketSubscriberFactory;
        private IStockMarketWebsocketSubscriber _stockMarketWebsocketSubscriber;
        private IDisposable _tradeOrderUnsubscriber;
        private IDisposable _stockMarketUnsubscriber;
        private readonly ILogger _logger;

        public NetworkManager(
            ITradeOrderWebsocketSubscriberFactory tradeOrderSocketSubscriberFactory,
            IStockMarketWebsocketSubscriberFactory stockMarketSocketSubscriberFactory,
            INetworkConfiguration networkConfiguration,
            ILogger logger)
        {
            _tradeOrderSocketSubscriberFactory =
                tradeOrderSocketSubscriberFactory 
                ?? throw new ArgumentNullException(nameof(tradeOrderSocketSubscriberFactory));

            _stockMarketSocketSubscriberFactory = stockMarketSocketSubscriberFactory
                ?? throw new ArgumentNullException(nameof(stockMarketSocketSubscriberFactory));

            _networkConfiguration = 
                networkConfiguration
                ?? throw new ArgumentNullException(nameof(networkConfiguration));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool InitiateAllNetworkConnections()
        {
            lock (_stateTransition)
            {
                return 
                    _InitiateTradingNetworkConnections()
                    && _InitiateStockMarketNetworkConnections();
            }
        }

        private bool _InitiateTradingNetworkConnections()
        {

            _logger.Log(LogLevel.Info, "Network Manager initiating trading network connections");

            if (_tradeOrderWebsocketSubscriber == null)
            {
                _tradeOrderWebsocketSubscriber = _tradeOrderSocketSubscriberFactory.Build();
            }

            return _tradeOrderWebsocketSubscriber.Initiate(
                _networkConfiguration.TradeWebsocketUriDomain,
                _networkConfiguration.TradeWebsocketUriPort);
        }

        private bool _InitiateStockMarketNetworkConnections()
        {
            _logger.Log(LogLevel.Info, "Network Manager initiating stock market network connections");

            if (_stockMarketWebsocketSubscriber == null)
            {
                _stockMarketWebsocketSubscriber = _stockMarketSocketSubscriberFactory.Build();
            }

            return _stockMarketWebsocketSubscriber.Initiate(
                _networkConfiguration.StockExchangeDomainUriDomainSegment,
                _networkConfiguration.StockExchangeDomainUriPort);
        }

        public void TerminateAllNetworkConnections()
        {
            lock (_stateTransition)
            {
                _logger.Log(LogLevel.Info, "Network Manager terminating network connections");

                if (_tradeOrderWebsocketSubscriber != null)
                {
                    _tradeOrderWebsocketSubscriber.Terminate();
                }

                if (_stockMarketWebsocketSubscriber != null)
                {
                    _stockMarketWebsocketSubscriber.Terminate();
                }
            }
        }

        /// <summary>
        /// Join the trade order stream to the websocket connections
        /// </summary>
        public bool AttachTradeOrderSubscriberToStream(ITradeOrderStream<TradeOrderFrame> orderStream)
        {
            lock (_stateTransition)
            {
                if (_tradeOrderWebsocketSubscriber == null)
                {
                    var successfullyInitiated = _InitiateTradingNetworkConnections();
                    if (!successfullyInitiated)
                    {
                        return false;
                    }
                }

                if (orderStream != null
                    && _tradeOrderWebsocketSubscriber != null)
                {
                    _logger.Log(LogLevel.Info, "Network Manager attaching trade order subscriber to stream");
                    _tradeOrderUnsubscriber = orderStream.Subscribe(_tradeOrderWebsocketSubscriber);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Detatch the trade order stream by calling the unsubscriber for the websocket connections
        /// </summary>
        public void DetatchTradeOrderSubscriber()
        {
            lock (_stateTransition)
            {
                _logger.Log(LogLevel.Info, "Network Manager detatching trade order subscriber from stream");

                if (_tradeOrderUnsubscriber != null)
                {
                    _tradeOrderUnsubscriber.Dispose();
                }
            }
        }

        /// <summary>
        /// Join the stock exchange stream to the websocket connections
        /// </summary>
        public bool AttachStockExchangeSubscriberToStream(IStockExchangeStream exchangeStream)
        {
            lock (_stateTransition)
            {
                if (_stockMarketWebsocketSubscriber == null)
                {
                    var successfullyInitiated = _InitiateStockMarketNetworkConnections();
                    if (!successfullyInitiated)
                    {
                        return false;
                    }
                }

                if (exchangeStream != null
                    && _stockMarketWebsocketSubscriber != null)
                {
                    _logger.Log(LogLevel.Info, "Network Manager attaching stock exchange subscriber to stream");
                    _stockMarketUnsubscriber = exchangeStream.Subscribe(_stockMarketWebsocketSubscriber);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Detatch the stock exchange stream by calling the unsubscriber for the websocket connections
        /// </summary>
        public void DetatchStockExchangeSubscriber()
        {
            lock (_stateTransition)
            {
                _logger.Log(LogLevel.Info, "Network Manager detatching stock exchange subscriber from stream");

                if (_stockMarketUnsubscriber != null)
                {
                    _stockMarketUnsubscriber.Dispose();
                }
            }
        }
    }
}