using System;

namespace TestHarness.Display.Interfaces
{
    public interface IBaseDisplay
    {
        event EventHandler<MarketFrameEventArgs> MarketFrameEvent;
        event EventHandler<TradeFrameEventArgs> TradeFrameEvent;
    }
}