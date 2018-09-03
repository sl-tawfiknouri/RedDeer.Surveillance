using NLog;
using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Network_IO
{
    public class StubNetworkManager : INetworkManager
    {
        private ILogger _logger;

        public StubNetworkManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool AttachStockExchangeSubscriberToStream(IStockExchangeStream exchangeStream)
        {
            _logger.Info("Stub Network Manager. Attach stock exchange subscriber to stream called.");
            return true;
        }

        public bool AttachTradeOrderSubscriberToStream(ITradeOrderStream<TradeOrderFrame> orderStream)
        {
            _logger.Info("Stub Network Manager. Attach trade order subscriber to stream called.");
            return true;
        }

        public void DetatchStockExchangeSubscriber()
        {
            _logger.Info("Stub Network Manager. Detatch stock exchange subscriber called.");
        }

        public void DetatchTradeOrderSubscriber()
        {
            _logger.Info("Stub Network Manager. Detatch trade order subscriber called.");
        }

        public bool InitiateAllNetworkConnections()
        {
            _logger.Info("Stub Network Manager. Initiate network connections called.");
            return true;
        }

        public void TerminateAllNetworkConnections()
        {
            _logger.Info("Stub Network Manager. Terminate network connections called.");
        }
    }
}
