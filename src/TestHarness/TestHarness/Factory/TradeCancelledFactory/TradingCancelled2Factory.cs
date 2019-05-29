using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Factory.TradeCancelledFactory.Interfaces;

namespace TestHarness.Factory.TradeCancelledFactory
{
    public class TradingCancelled2Factory : ITradingCancelled2Factory
    {
        private readonly ILogger _logger;

        public TradingCancelled2Factory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TradingCancelledOrderTradeProcess Build(DateTime trigger, List<string> sedols)
        {
            return new TradingCancelledOrderTradeProcess(new StubTradeStrategy(), sedols, trigger, _logger);
        }
    }
}
