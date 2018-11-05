using System;
using System.Collections.Generic;
using NLog;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Factory.TradingSpoofingV2Factory.Interfaces;

namespace TestHarness.Factory.TradingSpoofingV2Factory
{
    public class TradingSpoofingV2Factory : ITradingSpoofingV2Factory
    {
        private readonly ILogger _logger;

        public TradingSpoofingV2Factory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<string> sedols)
        {
            return new TradingSpoofingProcess(sedols, new StubTradeStrategy(), _logger);
        }
    }
}
