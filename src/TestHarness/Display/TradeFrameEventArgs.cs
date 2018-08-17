using Domain.Equity.Trading.Orders;
using System;

namespace TestHarness.Display
{
    public class TradeFrameEventArgs : EventArgs
    {
        public TradeFrameEventArgs(TradeOrderFrame frame)
        {
            Frame = frame;
        }

        public TradeOrderFrame Frame { get; }
    }
}