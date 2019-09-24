namespace Surveillance.Data.Universe.Interfaces
{
    using System.Collections.Generic;

    public interface IUniverse
    {
        IEnumerable<IUniverseEvent> UniverseEvents { get; }
    }
}