using System;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using NLog;
using TestHarness.Configuration;
using TestHarness.Network_IO.Subscribers;

namespace TestHarness.Network_IO
{
    public class NetworkManager : INetworkManager
    {
        private object _stateTransition = new object();

        INetworkConfiguration _networkConfiguration;
        private ITradeOrderWebsocketSubscriberFactory _tradeOrderSocketSubscriberFactory;
        private ITradeOrderWebsocketSubscriber _tradeOrderWebsocketSubscriber;
        private IDisposable _tradeOrderUnsubscriber;
        private ILogger _logger;

        public NetworkManager(
            ITradeOrderWebsocketSubscriberFactory tradeOrderSocketSubscriberFactory,
            INetworkConfiguration networkConfiguration,
            ILogger logger)
        {
            _tradeOrderSocketSubscriberFactory =
                tradeOrderSocketSubscriberFactory 
                ?? throw new ArgumentNullException(nameof(tradeOrderSocketSubscriberFactory));

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
                _networkConfiguration.TradeDomainUriDomainSegment,
                _networkConfiguration.TradeDomainUriPort);
        }

        private bool _InitiateStockMarketNetworkConnections()
        {
            _logger.Log(LogLevel.Info, "Network Manager initiating stock market network connections");

            return true;
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
    }
}