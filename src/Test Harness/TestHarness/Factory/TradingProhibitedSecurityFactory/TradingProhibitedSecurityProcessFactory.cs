using System;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using TestHarness.Factory.Interfaces;
using TestHarness.Factory.TradingProhibitedSecurityFactory.Interfaces;

namespace TestHarness.Factory.TradingProhibitedSecurityFactory
{
    public class TradingProhibitedSecurityProcessFactory : ITradingProhibitedSecurityProcessFactory
    {
        private IAppFactory _appFactory;

        public TradingProhibitedSecurityProcessFactory(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public IOrderDataGenerator Create()
        {
            var prohibitedTradeProcess = new TradingHeartbeatProhibitedSecuritiesProcess(
                _appFactory.ProhibitedSecurityHeartbeat,
                _appFactory.Logger,
                new StubTradeStrategy());

            return prohibitedTradeProcess;
        }
    }
}