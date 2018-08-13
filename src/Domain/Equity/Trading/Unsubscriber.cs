using System;
using System.Collections.Concurrent;

namespace Domain.Equity.Trading
{
    public class Unsubscriber : IDisposable
    {
        /// <summary>
        /// Shared by reference
        /// </summary>
        private readonly ConcurrentBag<IObserver<ExchangeTick>> _observers;

        private IObserver<ExchangeTick> _observer;

        public Unsubscriber(ConcurrentBag<IObserver<ExchangeTick>> observers, IObserver<ExchangeTick> observer)
        {
            _observers = observers ?? new ConcurrentBag<IObserver<ExchangeTick>>();
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers != null)
            {
                _observers.TryTake(out _observer);
            }
        }
    }
}
