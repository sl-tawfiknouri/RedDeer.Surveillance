using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using System;

namespace TestHarness.Display
{
    public class Console : IConsole
    {
        public void OutputMarketFrame(ExchangeFrame frame)
        {
            if (frame == null)
            {
                System.Console.WriteLine("Market. Empty frame");
                return;
            }

            System.Console.WriteLine();
            System.Console.WriteLine($"****************************");
            System.Console.WriteLine($"Market Frame. {frame.ToString()}");
            System.Console.WriteLine($"****************************");
            System.Console.WriteLine();
        }

        public void OutputTradeFrame(TradeOrderFrame frame)
        {
            if (frame == null)
            {
                System.Console.WriteLine("Trade. Empty frame");
                return;
            }

            System.Console.WriteLine($"Trade. {frame.ToString()}");
        }

        public void OutputException(Exception e)
        {
            if (e == null)
            {
                System.Console.WriteLine("Exception. Empty exception");
                return;
            }

            System.Console.WriteLine($"Exception. {e.ToString()}");
            System.Console.WriteLine($"Inner.Exception. {e.InnerException.ToString()}");
        }
    }
}