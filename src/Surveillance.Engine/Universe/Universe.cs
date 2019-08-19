namespace Surveillance.Engine.Rules.Universe
{
    using System.Collections.Generic;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class Universe : IUniverse
    {
        public Universe(IEnumerable<IUniverseEvent> universeEvents)
        {
            this.UniverseEvents = universeEvents ?? new List<IUniverseEvent>();
        }

        public IEnumerable<IUniverseEvent> UniverseEvents { get; }
    }
}