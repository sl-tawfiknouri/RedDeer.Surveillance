using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestHarness.Display
{
    public class Console : IConsole
    {
        private Stack<TradeOrderFrame> _tradeOrders;
        private object _lock = new object();

        private int _marketFrameOffset = 6;
        private int _tradeFrameOffset = 10;
        private int _tradeLimitToPrint = 10;

        private long _id = 0;

        public Console()
        {
            _tradeOrders = new Stack<TradeOrderFrame>();

            InitialConfiguration();
        }

        private void InitialConfiguration()
        {
            System.Console.Title = "Red Deer Surveillance | Test Harness";
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.BackgroundColor = ConsoleColor.Black;

            var height = Math.Min(30, System.Console.LargestWindowHeight);
            System.Console.SetWindowSize(System.Console.LargestWindowWidth, height);

            WriteToLine(2, "RED DEER SURVEILLANCE");
            WriteToLine(4, "TEST HARNESS");
        }

        public void OutputMarketFrame(ExchangeFrame frame)
        {
            lock (_lock)
            {
                if (frame == null)
                {
                    WriteToLine(_marketFrameOffset, "*****************************");
                    WriteToLine(_marketFrameOffset + 1, "Market. Empty frame");
                    WriteToLine(_marketFrameOffset + 2, "*****************************");
                    return;
                }

                WriteToLine(_marketFrameOffset, "*****************************");
                WriteToLine(_marketFrameOffset + 1, $"Market Frame ({DateTime.Now}). {frame.ToString()}");
                WriteToLine(_marketFrameOffset + 2, "*****************************");
            }
        }

        /// <summary>
        /// Paint the last (_tradeLimitToPrint) trades most recent first
        /// </summary>
        public void OutputTradeFrame(TradeOrderFrame frame)
        {
            lock (_lock)
            {
                if (frame == null)
                {
                    WriteToLine(_tradeFrameOffset, "Trade Frame. Empty frame");
                    return;
                }

                WriteToLine(_tradeFrameOffset, $"Trades to date. {_id}");

                var newStack = new Stack<TradeOrderFrame>();

                _tradeOrders.Push(frame);

                var loopSize = _tradeOrders.Count();
                for (var x = 1; x <= loopSize; x++)
                {
                    var order = _tradeOrders.Pop();
                    WriteToLine(_tradeFrameOffset + x, $"Trade. {order.ToString()}");

                    if (x < _tradeLimitToPrint)
                    {
                        newStack.Push(order);
                    }
                }

                var reversedStack = new Stack<TradeOrderFrame>();
                var reverseLoopSize = newStack.Count();
                for (var x = 1; x <= reverseLoopSize; x++)
                {
                    reversedStack.Push(newStack.Pop());
                }

                _tradeOrders = reversedStack;

                _id++;
            }
        }

        public void OutputException(Exception e)
        {
            lock (_lock)
            {
                if (e == null)
                {
                    WriteToLine(0, "Exception. Empty exception");
                    return;
                }

                WriteToLine(0, $"Exception. {e.ToString()}.{e.InnerException.ToString()}");
            }
        }

        public void ClearCommandInputLine()
        {
            WriteToLine(0, string.Empty);
        }

        public static void WriteToLine(int targetLine, string message)
        {
            System.Console.SetCursorPosition(0, targetLine);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(5, targetLine);
            System.Console.Write(message);
            System.Console.SetCursorPosition(0, 0);
        }
    }
}