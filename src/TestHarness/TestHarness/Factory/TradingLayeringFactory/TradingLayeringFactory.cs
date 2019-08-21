namespace TestHarness.Factory.TradingLayeringFactory
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Engine.Plans;
    using TestHarness.Factory.TradingLayeringFactory.Interfaces;

    public class TradingLayeringFactory : ITradingLayeringFactory
    {
        private readonly ILogger _logger;

        public TradingLayeringFactory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<DataGenerationPlan> plan)
        {
            return new TradingLayeringProcess(plan, new StubTradeStrategy(), this._logger);
        }
    }
}