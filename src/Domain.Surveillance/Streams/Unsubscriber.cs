using System;
using System.Collections.Concurrent;

namespace Domain.Streams
{
    public class Unsubscriber<T> : IDisposable
    {
        /// <summary>
        /// Shared by reference
        /// </summary>
        private readonly ConcurrentDictionary<IObserver<T>, IObserver<T>> _observers;

        private IObserver<T> _observer;

        public Unsubscriber(ConcurrentDictionary<IObserver<T>, IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers ?? new ConcurrentDictionary<IObserver<T>, IObserver<T>>();
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
