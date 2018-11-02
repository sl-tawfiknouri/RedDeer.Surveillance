﻿using System;
using System.Collections.Generic;
using TestHarness.Engine.OrderGenerator;

namespace TestHarness.Factory.TradeCancelledFactory
{
    public interface ITradingCancelled2Factory
    {
        TradingCancelledOrderTradeProcess Build(DateTime trigger, List<string> sedols);
    }
}