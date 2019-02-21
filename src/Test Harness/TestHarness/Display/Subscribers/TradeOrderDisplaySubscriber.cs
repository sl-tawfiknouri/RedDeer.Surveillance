using System;
using Domain.Trading;
using TestHarness.Display.Interfaces;

namespace TestHarness.Display.Subscribers
{
    public class TradeOrderFrameDisplaySubscriber : IObserver<Order>
    {
        private readonly IConsole _display;

        public TradeOrderFrameDisplaySubscriber(IConsole display)
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

        public void OnNext(Order value)
        {
            _display.OutputTradeFrame(value);
        }
    }
}
