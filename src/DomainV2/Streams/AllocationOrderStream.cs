using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Streams.Interfaces;
using System;
using System.Collections.Concurrent;

namespace DomainV2.Streams
{
    public class OrderAllocationStream<T> : IOrderAllocationStream<T>
    {
        private readonly ConcurrentDictionary<IObserver<T>, IObserver<T>> _observers;
        private readonly IUnsubscriberFactory<T> _unsubscriberFactory;

        public OrderAllocationStream(IUnsubscriberFactory<T> unsubscriberFactory)
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
