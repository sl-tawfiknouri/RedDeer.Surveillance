namespace Domain.Surveillance.Streams.Interfaces
{
    using System;

    public interface IOrderAllocationStream<T>
    {
        void Add(T order);

        IDisposable Subscribe(IObserver<T> observer);
    }
}