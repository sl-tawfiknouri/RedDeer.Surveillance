using System.Collections.Generic;
using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;

namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    public interface IUniverse
    {
        IReadOnlyCollection<Order> Trades { get; }
        IReadOnlyCollection<IUniverseEvent> UniverseEvents { get; }
        IReadOnlyCollection<EquityInterDayTimeBarCollection> InterDayEquityData { get; }
    }
}