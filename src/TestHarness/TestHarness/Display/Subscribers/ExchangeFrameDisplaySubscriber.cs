namespace TestHarness.Display.Subscribers
{
    using System;

    using Domain.Core.Markets.Collections;

    using TestHarness.Display.Interfaces;

    public class ExchangeFrameDisplaySubscriber : IObserver<EquityIntraDayTimeBarCollection>
    {
        private readonly IConsole _console;

        public ExchangeFrameDisplaySubscriber(IConsole console)
        {
            this._console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            this._console.OutputException(error);
        }

        public void OnNext(EquityIntraDayTimeBarCollection value)
        {
            this._console.OutputMarketFrame(value);
        }
    }
}