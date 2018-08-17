using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using System;

namespace TestHarness.Display
{
    public class Console : IConsole
    {
        private RingBuffer<TradeOrderFrame> _tradeOrders;
        private object _lock = new object();

        private int _marketFrameOffset = 6;
        private int _tradeFrameOffset = 10;

        public Console()
        {
            _tradeOrders = new RingBuffer<TradeOrderFrame>(10); // don't display more than last 10 trade orders
        }

        public void OutputMarketFrame(ExchangeFrame frame)
        {
            lock (_lock)
            {
                if (frame == null)
                {
                    WriteToLine(_marketFrameOffset, "Market. Empty frame");
                    return;
                }

                WriteToLine(_marketFrameOffset, "*****************************");
                WriteToLine(_marketFrameOffset + 1, $"Market Frame. {frame.ToString()}");
                WriteToLine(_marketFrameOffset + 2, "*****************************");
            }
        }

        public void OutputTradeFrame(TradeOrderFrame frame)
        {
            lock (_lock)
            {
                if (frame == null)
                {
                    WriteToLine(_tradeFrameOffset, "Trade Frame. Empty frame");
                    return;
                }

                _tradeOrders.Add(frame);
                WriteToLine(_tradeFrameOffset, $"Trade Frame. {frame.ToString()}");
            }
        }

        public void OutputException(Exception e)
        {
            lock (_lock)
            {
                if (e == null)
                {
                    System.Console.WriteLine("Exception. Empty exception");
                    return;
                }

                WriteToLine(0, $"Exception. {e.ToString()}.{e.InnerException.ToString()}");
            }
        }

        private void WriteToLine(int targetLine, string message)
        {
            System.Console.SetCursorPosition(0, targetLine);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(5, targetLine);
            System.Console.Write(message);
        }
    }
}