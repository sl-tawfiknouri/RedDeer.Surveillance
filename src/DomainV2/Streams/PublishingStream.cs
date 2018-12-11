using System;

namespace DomainV2.Streams
{
    public interface IPublishingStream<T> : IObservable<T> 
    {
        void Add(T streamData);
    }
}
