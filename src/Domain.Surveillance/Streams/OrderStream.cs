namespace Domain.Surveillance.Streams
{
    using System;
    using System.Collections.Concurrent;

    using Domain.Surveillance.Streams.Interfaces;

    public class OrderStream<T> : IOrderStream<T>
    {
        private readonly ConcurrentDictionary<IObserver<T>, IObserver<T>> _observers;

        private readonly IUnsubscriberFactory<T> _unsubscriberFactory;

        public OrderStream(IUnsubscriberFactory<T> unsubscriberFactory)
        {
            this._observers = new ConcurrentDictionary<IObserver<T>, IObserver<T>>();
            this._unsubscriberFactory =
                unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public void Add(T order)
        {
            if (order == null) return;

            if (this._observers == null) return;

            foreach (var obs in this._observers)
                obs.Value?.OnNext(order);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            if (!this._observers.ContainsKey(observer)) this._observers.TryAdd(observer, observer);

            return this._unsubscriberFactory.Create(this._observers, observer);
        }
    }
}