namespace TestHarness.Factory.TradingSpoofingFactory
{
    using System;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Factory.Interfaces;
    using TestHarness.Factory.TradingSpoofingFactory.Interfaces;

    public class TradingSpoofingProcessFactory : ITradingSpoofingProcessFactory
    {
        private readonly IAppFactory _appFactory;

        public TradingSpoofingProcessFactory(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public IOrderDataGenerator Create()
        {
            var spoofingProcess = new TradingHeartbeatSpoofingProcess(
                this._appFactory.SpoofedTradeHeartbeat,
                this._appFactory.Logger,
                new StubTradeStrategy());

            return spoofingProcess;
        }
    }
}