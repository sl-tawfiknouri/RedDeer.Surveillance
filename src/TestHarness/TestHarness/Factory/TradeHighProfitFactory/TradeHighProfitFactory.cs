namespace TestHarness.Factory.TradeHighProfitFactory
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Engine.Plans;
    using TestHarness.Factory.TradeHighProfitFactory.Interfaces;

    public class TradeHighProfitFactory : ITradeHighProfitFactory
    {
        private readonly ILogger _logger;

        public TradeHighProfitFactory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<DataGenerationPlan> plans)
        {
            plans = plans ?? new DataGenerationPlan[0];
            return new TradingHighProfitProcess(plans, new StubTradeStrategy(), this._logger);
        }
    }
}