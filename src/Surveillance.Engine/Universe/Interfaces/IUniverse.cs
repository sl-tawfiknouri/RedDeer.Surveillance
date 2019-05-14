using System.Collections.Generic;

namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    public interface IUniverse
    {
        IEnumerable<IUniverseEvent> UniverseEvents { get; }
    }
}