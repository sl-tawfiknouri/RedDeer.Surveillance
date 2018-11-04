using System;
using System.Collections.Generic;
using NLog;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Engine.Plans;
using TestHarness.Factory.TradeHighProfitFactory.Interfaces;

namespace TestHarness.Factory.TradeHighProfitFactory
{
    public class TradeHighProfitFactory : ITradeHighProfitFactory
    {
        private readonly ILogger _logger;

        public TradeHighProfitFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<DataGenerationPlan> plans)
        {
            plans = plans ?? new DataGenerationPlan[0];
            return new TradingHighProfitProcess(plans, new StubTradeStrategy(), _logger);
        }
    }
}
