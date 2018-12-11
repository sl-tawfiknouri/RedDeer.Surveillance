using System;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Network_IO
{
    public class StubNetworkManager : INetworkManager
    {
        private readonly ILogger _logger;

        public StubNetworkManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool AttachStockExchangeSubscriberToStream(IStockExchangeStream exchangeStream)
        {
            _logger.LogInformation("Stub Network Manager. Attach stock exchange subscriber to stream called.");
            return true;
        }

        public bool AttachTradeOrderSubscriberToStream(IOrderStream<Order> orderStream)
        {
            _logger.LogInformation("Stub Network Manager. Attach trade order subscriber to stream called.");
            return true;
        }

        public void DetachStockExchangeSubscriber()
        {
            _logger.LogInformation("Stub Network Manager. Detach stock exchange subscriber called.");
        }

        public void DetachTradeOrderSubscriber()
        {
            _logger.LogInformation("Stub Network Manager. Detach trade order subscriber called.");
        }

        public bool InitiateAllNetworkConnections()
        {
            _logger.LogInformation("Stub Network Manager. Initiate network connections called.");
            return true;
        }

        public void TerminateAllNetworkConnections()
        {
            _logger.LogInformation("Stub Network Manager. Terminate network connections called.");
        }
    }
}
