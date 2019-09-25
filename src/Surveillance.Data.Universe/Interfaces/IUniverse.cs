namespace Surveillance.Data.Universe.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// The Universe interface.
    /// </summary>
    public interface IUniverse
    {
        /// <summary>
        /// Gets the universe events.
        /// </summary>
        IEnumerable<IUniverseEvent> UniverseEvents { get; }
    }
}