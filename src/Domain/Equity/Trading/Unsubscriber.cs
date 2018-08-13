using System;
using System.Collections.Concurrent;

namespace Domain.Equity.Trading
{
    public class Unsubscriber : IDisposable
    {
        /// <summary>
        /// Shared by reference
        /// </summary>
        private readonly ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>> _observers;

        private IObserver<ExchangeTick> _observer;

        public Unsubscriber(ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>> observers, IObserver<ExchangeTick> observer)
        {
            _observers = observers ?? new ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>>();
            _observer = observer ?? throw new ArgumentNullException(nameof(observer));
        }

        public void Dispose()
        {
            if (_observers != null 
                && _observers.ContainsKey(_observer))
            {
                _observers.TryRemove(_observer, out _observer);
            }
        }
    }
}
