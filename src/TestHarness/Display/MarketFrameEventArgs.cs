using Domain.Equity.Trading.Frames;
using System;

namespace TestHarness.Display
{
    public class MarketFrameEventArgs : EventArgs
    {
        public MarketFrameEventArgs(ExchangeFrame frame)
        {
            Frame = frame;
        }

        public ExchangeFrame Frame { get; }
    }
}
