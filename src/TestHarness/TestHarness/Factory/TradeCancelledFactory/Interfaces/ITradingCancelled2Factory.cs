namespace TestHarness.Factory.TradeCancelledFactory.Interfaces
{
    using System;
    using System.Collections.Generic;

    using TestHarness.Engine.OrderGenerator;

    public interface ITradingCancelled2Factory
    {
        TradingCancelledOrderTradeProcess Build(DateTime trigger, List<string> sedols);
    }
}