using System;

namespace DomainV2.Streams.Interfaces
{
    public interface IOrderAllocationStream<T>
    {
        void Add(T order);
        IDisposable Subscribe(IObserver<T> observer);
    }
}
