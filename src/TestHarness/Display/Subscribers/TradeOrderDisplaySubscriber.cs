using Domain.Equity.Trading.Orders;
using System;

namespace TestHarness.Display.Subscribers
{
    public class TradeOrderDisplaySubscriber : IObserver<TradeOrderFrame>
    {
        private readonly IConsole _display;

        public TradeOrderDisplaySubscriber(IConsole display)
        {
            _display = display ?? throw new ArgumentNullException(nameof(display));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            _display.OutputException(error);
        }

        public void OnNext(TradeOrderFrame value)
        {
            _display.OutputTradeFrame(value);
        }
    }
}
