using System;
using System.Collections.Concurrent;
using Domain.Surveillance.Streams.Interfaces;

namespace Domain.Surveillance.Streams
{
    public class OrderStream<T> : IOrderStream<T>
    {
        private readonly ConcurrentDictionary<IObserver<T>, IObserver<T>> _observers;
        private readonly IUnsubscriberFactory<T> _unsubscriberFactory;

        public OrderStream(IUnsubscriberFactory<T> unsubscriberFactory)
        {
            _observers = new ConcurrentDictionary<IObserver<T>, IObserver<T>>();
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (!_observers.ContainsKey(observer))
            {
                _observers.TryAdd(observer, observer);
            }

            return _unsubscriberFactory.Create(_observers, observer);
        }

        public void Add(T order)
        {
            if (order == null)
            {
                return;
            }

            if (_observers == null)
            {
                return;
            }

            foreach (var obs in _observers)
                obs.Value?.OnNext(order);
        }
    }
}
