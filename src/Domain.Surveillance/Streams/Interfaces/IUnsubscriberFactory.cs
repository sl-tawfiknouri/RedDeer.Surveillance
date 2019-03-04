using System;
using System.Collections.Concurrent;
using Domain.Streams;

namespace Domain.Equity.Streams.Interfaces
{
    public interface IUnsubscriberFactory<T>
    {
        Unsubscriber<T> Create(ConcurrentDictionary<IObserver<T>, IObserver<T>> observers, IObserver<T> observer);
    }
}