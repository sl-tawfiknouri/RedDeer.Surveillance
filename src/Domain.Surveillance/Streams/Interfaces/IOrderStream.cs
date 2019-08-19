namespace Domain.Surveillance.Streams.Interfaces
{
    using System;

    public interface IOrderStream<T>
    {
        void Add(T order);

        IDisposable Subscribe(IObserver<T> observer);
    }
}