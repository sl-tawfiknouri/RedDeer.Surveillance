using System;

namespace DomainV2.Streams.Interfaces
{
    public interface IOrderStream<T>
    {
        void Add(T order);
        IDisposable Subscribe(IObserver<T> observer);
    }
}