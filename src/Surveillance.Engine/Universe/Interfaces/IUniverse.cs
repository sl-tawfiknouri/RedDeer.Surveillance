using System.Collections.Generic;
using Domain.Equity.TimeBars;
using Domain.Trading;

namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    public interface IUniverse
    {
        IReadOnlyCollection<Order> Trades { get; }
        IReadOnlyCollection<IUniverseEvent> UniverseEvents { get; }
        IReadOnlyCollection<EquityInterDayTimeBarCollection> InterDayEquityData { get; }
    }
}