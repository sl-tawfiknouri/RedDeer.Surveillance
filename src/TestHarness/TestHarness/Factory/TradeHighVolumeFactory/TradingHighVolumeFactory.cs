namespace TestHarness.Factory.TradeHighVolumeFactory
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Factory.TradeHighVolumeFactory.Interfaces;

    public class TradingHighVolumeFactory : ITradingHighVolumeFactory
    {
        private readonly ILogger _logger;

        public TradingHighVolumeFactory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<string> sedols)
        {
            sedols = sedols ?? new string[0];

            return new TradingHighVolumeTradeProcess(sedols, new StubTradeStrategy(), this._logger);
        }
    }
}