﻿namespace TestHarness.Factory.TradeWashTradeFactory
{
    using System;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Engine.Plans;
    using TestHarness.Factory.TradeWashTradeFactory.Interfaces;

    public class TradeWashTradeFactory : ITradeWashTradeFactory
    {
        private readonly ILogger _logger;

        public TradeWashTradeFactory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(DataGenerationPlan plan)
        {
            return new TradingWashTradeProcess(new StubTradeStrategy(), plan, this._logger);
        }
    }
}