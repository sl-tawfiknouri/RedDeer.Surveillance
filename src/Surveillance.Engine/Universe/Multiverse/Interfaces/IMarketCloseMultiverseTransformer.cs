using System;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Multiverse.Interfaces
{
    public interface IMarketCloseMultiverseTransformer
        : IObserver<IUniverseEvent>, IObservable<IUniverseEvent>, ICloneable
    {
        IDisposable Subscribe(IUniverseCloneableRule rule);
    }
}
