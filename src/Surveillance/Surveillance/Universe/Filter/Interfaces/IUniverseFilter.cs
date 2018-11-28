using System;
using Surveillance.Rules.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Filter.Interfaces
{
    public interface IUniverseFilter : IObservable<IUniverseEvent>, IUniverseCloneableRule
    { }
}
