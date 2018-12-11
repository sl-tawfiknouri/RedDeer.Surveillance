using System.Collections.Generic;
using DomainV2.Trading;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverse
    {
        IReadOnlyCollection<Order> Trades { get; }
        IReadOnlyCollection<IUniverseEvent> UniverseEvents { get; }
    }
}