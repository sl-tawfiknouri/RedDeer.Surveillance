using System;
using NLog;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Strategies;

namespace TestHarness.Factory.TradeCancelledFactory
{
    public class TradingCancelled2Factory : ITradingCancelled2Factory
    {
        private readonly ILogger _logger;

        public TradingCancelled2Factory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TradingCancelledOrderTradeProcess Build(DateTime trigger, params string[] sedols)
        {
            return new TradingCancelledOrderTradeProcess(new StubTradeStrategy(), sedols, trigger, _logger);
        }
    }
}
