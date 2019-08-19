namespace Domain.Surveillance.Streams
{
    using System;
    using System.Collections.Concurrent;

    public class Unsubscriber<T> : IDisposable
    {
        /// <summary>
        ///     Shared by reference
        /// </summary>
        private readonly ConcurrentDictionary<IObserver<T>, IObserver<T>> _observers;

        private IObserver<T> _observer;

        public Unsubscriber(ConcurrentDictionary<IObserver<T>, IObserver<T>> observers, IObserver<T> observer)
        {
            this._observers = observers ?? new ConcurrentDictionary<IObserver<T>, IObserver<T>>();
            this._observer = observer ?? throw new ArgumentNullException(nameof(observer));
        }

        public void Dispose()
        {
            if (this._observers != null && this._observers.ContainsKey(this._observer))
                this._observers.TryRemove(this._observer, out this._observer);
        }
    }
}