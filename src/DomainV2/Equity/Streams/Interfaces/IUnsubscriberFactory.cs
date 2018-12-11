using System;
using System.Collections.Concurrent;
using DomainV2.Streams;

namespace DomainV2.Equity.Streams.Interfaces
{
    public interface IUnsubscriberFactory<T>
    {
        Unsubscriber<T> Create(ConcurrentDictionary<IObserver<T>, IObserver<T>> observers, IObserver<T> observer);
    }
}