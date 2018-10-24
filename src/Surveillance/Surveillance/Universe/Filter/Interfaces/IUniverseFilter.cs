using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Filter.Interfaces
{
    public interface IUniverseFilter : IObservable<IUniverseEvent>, IObserver<IUniverseEvent>
    { }
}
