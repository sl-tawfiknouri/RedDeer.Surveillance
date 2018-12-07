using System;

namespace DataImport.Processors.Interfaces
{
    public interface IProcessor<T> : IObserver<T>, IObservable<T>
    {
    }
}
