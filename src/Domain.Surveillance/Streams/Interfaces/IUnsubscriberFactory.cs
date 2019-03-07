using System;
using System.Collections.Concurrent;

namespace Domain.Surveillance.Streams.Interfaces
{
    public interface IUnsubscriberFactory<T>
    {
        Unsubscriber<T> Create(ConcurrentDictionary<IObserver<T>, IObserver<T>> observers, IObserver<T> observer);
    }
}