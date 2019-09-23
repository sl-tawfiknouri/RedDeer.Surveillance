namespace Domain.Surveillance.Streams
{
    using System;

    public interface IPublishingStream<T> : IObservable<T>
    {
        void Add(T streamData);
    }
}