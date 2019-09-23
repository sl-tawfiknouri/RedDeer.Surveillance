namespace TestHarness.Display
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using TestHarness.Display.Interfaces;

    public class Console : IConsole
    {
        private readonly object _lock = new object();

        private readonly int _marketFrameOffset = 6;

        private readonly int _tradeFrameOffset = 10;

        private readonly int _tradeLimitToPrint = 10;

        private long _id;

        private Stack<Order> _tradeOrders;

        public Console()
        {
            this._tradeOrders = new Stack<Order>();

            this.InitialConfiguration();
        }

        public static void WriteToLine(int targetLine, string message)
        {
            System.Console.SetCursorPosition(0, targetLine);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(5, targetLine);
            System.Console.Write(message);
            System.Console.SetCursorPosition(0, 0);
        }

        public void ClearCommandInputLine()
        {
            WriteToLine(0, string.Empty);
        }

        public void OutputException(Exception e)
        {
            lock (this._lock)
            {
                if (e == null)
                {
                    WriteToLine(0, "Exception. Empty exception");
                    return;
                }

                WriteToLine(0, $"Exception. {e.Message}");
            }
        }

        public void OutputMarketFrame(EquityIntraDayTimeBarCollection frame)
        {
            lock (this._lock)
            {
                if (frame == null)
                {
                    WriteToLine(this._marketFrameOffset, "*****************************");
                    WriteToLine(this._marketFrameOffset + 1, "Market. Empty frame");
                    WriteToLine(this._marketFrameOffset + 2, "*****************************");
                    return;
                }

                WriteToLine(this._marketFrameOffset, "*****************************");
                WriteToLine(this._marketFrameOffset + 1, $"Market Frame ({frame.Epoch}). {frame}");
                WriteToLine(this._marketFrameOffset + 2, "*****************************");
            }
        }

        /// <summary>
        ///     Paint the last (_tradeLimitToPrint) trades most recent first
        /// </summary>
        public void OutputTradeFrame(Order frame)
        {
            lock (this._lock)
            {
                if (frame == null)
                {
                    WriteToLine(this._tradeFrameOffset, "Trade Frame. Empty frame");
                    return;
                }

                WriteToLine(
                    this._tradeFrameOffset,
                    $"Showing last {this._tradeLimitToPrint} trades of {this._id + 1}.");
                var newStack = new Stack<Order>();
                this._tradeOrders.Push(frame);

                var loopSize = this._tradeOrders.Count;
                for (var x = 1; x <= loopSize; x++)
                {
                    var order = this._tradeOrders.Pop();
                    WriteToLine(this._tradeFrameOffset + x, $"Trade. {order}");

                    if (x < this._tradeLimitToPrint) newStack.Push(order);
                }

                var reversedStack = new Stack<Order>();
                var reverseLoopSize = newStack.Count;
                for (var x = 1; x <= reverseLoopSize; x++) reversedStack.Push(newStack.Pop());

                this._tradeOrders = reversedStack;

                this._id++;
            }
        }

        public void Write(string message)
        {
            this.WriteToUserFeedbackLine(message);
        }

        public void WriteToUserFeedbackLine(string feedback)
        {
            WriteToLine(1, feedback);
        }

        private void InitialConfiguration()
        {
            try
            {
                System.Console.Title = "Red Deer Surveillance | Test Harness";
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.BackgroundColor = ConsoleColor.Black;

                var height = Math.Min(30, System.Console.LargestWindowHeight);
                height = Math.Max(height, 1);
                var width = Math.Max(1, System.Console.LargestWindowWidth);

                System.Console.SetWindowSize(width, height);

                WriteToLine(2, "RED DEER SURVEILLANCE");
                WriteToLine(4, "TEST HARNESS");
            }
            catch
            {
            }
        }
    }
}