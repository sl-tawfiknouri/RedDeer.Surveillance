namespace TestHarness.Display.Subscribers
{
    using System;

    using Domain.Core.Trading.Orders;

    using TestHarness.Display.Interfaces;

    public class TradeOrderFrameDisplaySubscriber : IObserver<Order>
    {
        private readonly IConsole _display;

        public TradeOrderFrameDisplaySubscriber(IConsole display)
        {
            this._display = display ?? throw new ArgumentNullException(nameof(display));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            this._display.OutputException(error);
        }

        public void OnNext(Order value)
        {
            this._display.OutputTradeFrame(value);
        }
    }
}