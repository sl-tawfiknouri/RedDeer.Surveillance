using System;

namespace Domain.Surveillance.Streams
{
    public interface IPublishingStream<T> : IObservable<T> 
    {
        void Add(T streamData);
    }
}
