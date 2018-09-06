using System;
using Domain.Trades.Orders;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Factory.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;

namespace TestHarness.Factory.TradingFactory
{
    public class TradingFileRelayProcessFactory : ITradingFileRelayProcessFactory
    {
        private readonly IAppFactory _appFactory;

        public TradingFileRelayProcessFactory(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public TradingFileRelayProcess Build(string filePath)
        {
            return new TradingFileRelayProcess(
                _appFactory.Logger,
                new TradeOrderCsvToDtoMapper(),
                filePath);
        }
    }
}
