using System;

namespace Relay.Processors.Interfaces
{
    public interface IProcessor<T> : IObserver<T>, IObservable<T>
    {
    }
}
