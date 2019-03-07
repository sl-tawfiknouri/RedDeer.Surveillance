using System;

namespace Domain.Surveillance.Streams.Interfaces
{
    public interface IOrderStream<T>
    {
        void Add(T order);
        IDisposable Subscribe(IObserver<T> observer);
    }
}