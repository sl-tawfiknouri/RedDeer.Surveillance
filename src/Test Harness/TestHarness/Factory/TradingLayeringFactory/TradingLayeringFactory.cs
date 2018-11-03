using System;
using System.Collections.Generic;
using NLog;
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

        public IOrderDataGenerator Build(IReadOnlyCollection<string> sedols, IReadOnlyCollection<DataGenerationPlan> plan)
        {
            sedols = sedols ?? new string[0];
            return new TradingLayeringProcess(sedols, plan, new StubTradeStrategy(), _logger);
        }
    }
}
