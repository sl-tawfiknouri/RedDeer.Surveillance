using System;

namespace Domain.Streams
{
    public interface IPublishingStream<T> : IObservable<T> 
    {
        void Add(T streamData);
    }
}
