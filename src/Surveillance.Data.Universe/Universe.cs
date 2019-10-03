namespace Surveillance.Data.Universe
{
    using System.Collections.Generic;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The universe.
    /// </summary>
    public class Universe : IUniverse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Universe"/> class.
        /// </summary>
        /// <param name="universeEvents">
        /// The universe events.
        /// </param>
        public Universe(IEnumerable<IUniverseEvent> universeEvents)
        {
            this.UniverseEvents = universeEvents ?? new List<IUniverseEvent>();
        }

        /// <summary>
        /// Gets the universe events.
        /// </summary>
        public IEnumerable<IUniverseEvent> UniverseEvents { get; }
    }
}