using System.Collections.Generic;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    public class Universe : IUniverse
    {
        public Universe(IEnumerable<IUniverseEvent> universeEvents)
        {
            UniverseEvents = universeEvents ?? new List<IUniverseEvent>();
        }

        public IEnumerable<IUniverseEvent> UniverseEvents { get; private set; }
    }
}