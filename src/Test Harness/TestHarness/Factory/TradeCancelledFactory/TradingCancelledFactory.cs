using System;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Factory.TradeCancelledFactory
{
    public class TradingCancelledFactory
    {
        private readonly IAppFactory _appFactory;

        public TradingCancelledFactory(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public IOrderDataGenerator Create()
        {
            var cancelledTradeProcess = (IOrderDataGenerator)null;

            return cancelledTradeProcess;
        }
    }
}
