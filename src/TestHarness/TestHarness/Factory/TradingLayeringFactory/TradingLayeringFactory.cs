using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Engine.Plans;
using TestHarness.Factory.TradingLayeringFactory.Interfaces;

namespace TestHarness.Factory.TradingLayeringFactory
{
    public class TradingLayeringFactory : ITradingLayeringFactory
    {
        private readonly ILogger _logger;

        public TradingLayeringFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<DataGenerationPlan> plan)
        {
            return new TradingLayeringProcess(plan, new StubTradeStrategy(), _logger);
        }
    }
}
