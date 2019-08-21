namespace TestHarness.Factory.TradingSpoofingV2Factory
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Factory.TradingSpoofingV2Factory.Interfaces;

    public class TradingSpoofingV2Factory : ITradingSpoofingV2Factory
    {
        private readonly ILogger _logger;

        public TradingSpoofingV2Factory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<string> sedols)
        {
            return new TradingSpoofingProcess(sedols, new StubTradeStrategy(), this._logger);
        }
    }
}