using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Multiverse.Interfaces
{
    public interface IMarketCloseMultiverseTransformer 
        : IObserver<IUniverseEvent>, IObservable<IUniverseEvent>
    { }
}
