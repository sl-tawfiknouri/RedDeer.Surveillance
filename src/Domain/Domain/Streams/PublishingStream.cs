using System;

namespace Domain.Streams
{
    public interface PublishingStream<T> : IObservable<T> 
    {
        void Add(T streamData);
    }
}
