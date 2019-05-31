using System;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    public interface IUniverseFilterService : IObservable<IUniverseEvent>, IUniverseRule
    {
    }
}
