using System;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Factory.Interfaces;
using TestHarness.Factory.TradeCancelledFactory.Interfaces;

namespace TestHarness.Factory.TradeCancelledFactory
{
    public class TradingCancelledFactory : ITradingCancelledFactory
    {
        private readonly IAppFactory _appFactory;

        public TradingCancelledFactory(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public IOrderDataGenerator Create()
        {
            var cancelledTradeProcess = 
                new TradingHeartbeatCancelledTradeProcess(
                    _appFactory.Logger,
                    new StubTradeStrategy(),
                    _appFactory.CancelTradeHeartbeat);

            return cancelledTradeProcess;
        }
    }
}
