namespace Domain.Surveillance.Streams.Interfaces
{
    using System;
    using System.Collections.Concurrent;

    public interface IUnsubscriberFactory<T>
    {
        Unsubscriber<T> Create(ConcurrentDictionary<IObserver<T>, IObserver<T>> observers, IObserver<T> observer);
    }
}