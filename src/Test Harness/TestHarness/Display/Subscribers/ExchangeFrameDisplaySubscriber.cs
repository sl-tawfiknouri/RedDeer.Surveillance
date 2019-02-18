using System;
using Domain.Equity.TimeBars;
using TestHarness.Display.Interfaces;

namespace TestHarness.Display.Subscribers
{
    public class ExchangeFrameDisplaySubscriber : IObserver<EquityIntraDayTimeBarCollection>
    {
        private readonly IConsole _console;

        public ExchangeFrameDisplaySubscriber(IConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            _console.OutputException(error);
        }

        public void OnNext(EquityIntraDayTimeBarCollection value)
        {
            _console.OutputMarketFrame(value);
        }
    }
}
