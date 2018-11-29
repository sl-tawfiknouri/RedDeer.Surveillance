using System;
using Surveillance.Rules.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Multiverse.Interfaces
{
    public interface IMarketCloseMultiverseTransformer
        : IObserver<IUniverseEvent>, IObservable<IUniverseEvent>, ICloneable
    {
        IDisposable Subscribe(IUniverseCloneableRule rule);
    }
}
