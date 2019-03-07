using System;

namespace Domain.Surveillance.Streams.Interfaces
{
    public interface IOrderAllocationStream<T>
    {
        void Add(T order);
        IDisposable Subscribe(IObserver<T> observer);
    }
}
