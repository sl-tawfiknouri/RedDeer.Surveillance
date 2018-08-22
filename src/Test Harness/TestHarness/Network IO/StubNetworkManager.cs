using Domain.Equity.Trading.Streams.Interfaces;
using NLog;
using System;

namespace TestHarness.Network_IO
{
    public class StubNetworkManager : INetworkManager
    {
        private ILogger _logger;

        public StubNetworkManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void AttachTradeOrderSubscriberToStream(ITradeOrderStream orderStream)
        {
            _logger.Info("Stub Network Manager. Attach trade order subscriber to stream called.");
        }

        public void DetatchTradeOrderSubscriber()
        {
            _logger.Info("Stub Network Manager. Detatch trade order subscriber called.");
        }

        public void InitiateNetworkConnections()
        {
            _logger.Info("Stub Network Manager. Initiate network connections called.");
        }

        public void TerminateNetworkConnections()
        {
            _logger.Info("Stub Network Manager. Terminate network connections called.");
        }
    }
}
