using System;

namespace Domain.Streams.Interfaces
{
    public interface IOrderAllocationStream<T>
    {
        void Add(T order);
        IDisposable Subscribe(IObserver<T> observer);
    }
}
