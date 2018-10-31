using System;
using TestHarness.Engine.OrderGenerator;

namespace TestHarness.Factory.TradeCancelledFactory
{
    public interface ITradingCancelled2Factory
    {
        TradingCancelledOrderTradeProcess Build(DateTime trigger, params string[] sedols);
    }
}