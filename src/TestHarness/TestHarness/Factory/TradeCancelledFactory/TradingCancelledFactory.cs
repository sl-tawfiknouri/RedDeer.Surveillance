namespace TestHarness.Factory.TradeCancelledFactory
{
    using System;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Factory.Interfaces;
    using TestHarness.Factory.TradeCancelledFactory.Interfaces;

    public class TradingCancelledFactory : ITradingCancelledFactory
    {
        private readonly IAppFactory _appFactory;

        public TradingCancelledFactory(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public IOrderDataGenerator Create()
        {
            var cancelledTradeProcess = new TradingHeartbeatCancelledTradeProcess(
                this._appFactory.Logger,
                new StubTradeStrategy(),
                this._appFactory.CancelTradeHeartbeat);

            return cancelledTradeProcess;
        }
    }
}