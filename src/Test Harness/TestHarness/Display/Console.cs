using Domain.Equity.Trading;
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

        private long _id = 0;

        public Console()
        {
            _tradeOrders = new Stack<TradeOrderFrame>();
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
                WriteToLine(_marketFrameOffset + 1, $"Market Frame ({DateTime.Now}). {frame.ToString()}");
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

                if (_tradeOrders.Count > 10)
                    _tradeOrders.Pop();

                _tradeOrders.Reverse();
                _tradeOrders.Push(frame);

                WriteToLine(_tradeFrameOffset, $"Trades to date. {_id}");

                var newStack = new Stack<TradeOrderFrame>();

                var loopSize = _tradeOrders.Count();
                for (var x = 1; x <= loopSize; x++)
                {
                    var order = _tradeOrders.Pop();
                    WriteToLine(_tradeFrameOffset + x, $"Trade Frame. {order.ToString()}");
                    newStack.Push(order);
                }

                newStack.Reverse();

                _tradeOrders = newStack;

                _id++;
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