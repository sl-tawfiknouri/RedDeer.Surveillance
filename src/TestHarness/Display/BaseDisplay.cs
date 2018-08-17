using System;
using TestHarness.Display.Interfaces;

namespace TestHarness.Display
{
    public abstract class BaseDisplay : IBaseDisplay
    {
        public event EventHandler<MarketFrameEventArgs> MarketFrameEvent;
        public event EventHandler<TradeFrameEventArgs> TradeFrameEvent;
    }
}