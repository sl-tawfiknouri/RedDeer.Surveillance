using System;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Factory.TradingSpoofingFactory
{
    public class TradingSpoofingProcessFactory : ITradingSpoofingProcessFactory
    {
        private IAppFactory _appFactory;

        public TradingSpoofingProcessFactory(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public IOrderDataGenerator Create()
        {
            var spoofingProcess =
                new TradingHeartbeatSpoofingProcess(
                    _appFactory.SpoofedTradeHeartbeat,
                    _appFactory.Logger,
                    new StubTradeStrategy());

            return spoofingProcess;
        }
    }
}
