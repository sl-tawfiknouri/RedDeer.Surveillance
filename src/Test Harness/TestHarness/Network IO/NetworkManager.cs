using System;
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

        public void InitiateNetworkConnections()
        {
            lock (_stateTransition)
            {
                _InitiateNetworkConnections();
            }
        }

        private void _InitiateNetworkConnections()
        {
            _logger.Log(LogLevel.Info, "Network Manager initiating network connections");

            if (_tradeOrderWebsocketSubscriber == null)
            {
                _tradeOrderWebsocketSubscriber = _tradeOrderSocketSubscriberFactory.Build();
            }

            _tradeOrderWebsocketSubscriber.Initiate(
                _networkConfiguration.TradeDomainUriDomainSegment,
                _networkConfiguration.TradeDomainUriPort);
        }

        public void TerminateNetworkConnections()
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

        public void AttachTradeOrderSubscriberToStream(ITradeOrderStream orderStream)
        {
            lock (_stateTransition)
            {
                if (_tradeOrderWebsocketSubscriber == null)
                {
                    _InitiateNetworkConnections();
                }

                if (orderStream != null
                    && _tradeOrderWebsocketSubscriber != null)
                {
                    _logger.Log(LogLevel.Info, "Network Manager attaching trade order subscriber to stream");
                    _tradeOrderUnsubscriber = orderStream.Subscribe(_tradeOrderWebsocketSubscriber);
                }
            }
        }

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