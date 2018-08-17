namespace TestHarness.Display
{
    public class Console : BaseDisplay, IConsole
    {
        private object _lock = new object();
        private volatile bool initiated;

        public void Initiate()
        {
            lock (_lock)
            {
                if (initiated)
                {
                    _Terminate();
                }

                MarketFrameEvent += OutputMarketFrameEvent;
                TradeFrameEvent += OutputTradeFrameEvent;
                initiated = true;
            }
        }

        public void Terminate()
        {
            lock (_lock)
            {
                _Terminate();
            }
        }

        private void _Terminate()
        {
            MarketFrameEvent -= OutputMarketFrameEvent;
            TradeFrameEvent -= OutputTradeFrameEvent;
            initiated = false;
        }


        private void OutputMarketFrameEvent(object sender, MarketFrameEventArgs e)
        {
            if (e == null
                || e.Frame == null)
            {
                System.Console.WriteLine("Market. Empty frame");
                return;
            }

            System.Console.WriteLine($"Market. {e.Frame.ToString()}");
        }

        private void OutputTradeFrameEvent(object sender, TradeFrameEventArgs e)
        {
            if (e == null
                || e.Frame == null)
            {
                System.Console.WriteLine("Trade. Empty frame");
                return;
            }

            System.Console.WriteLine($"Trade. {e.Frame.ToString()}");
        }
    }
}
