using System.Collections.Generic;
using DomainV2.Equity.TimeBars;
using DomainV2.Trading;

namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    public interface IUniverse
    {
        IReadOnlyCollection<Order> Trades { get; }
        IReadOnlyCollection<IUniverseEvent> UniverseEvents { get; }
        IReadOnlyCollection<EquityInterDayTimeBarCollection> InterDayEquityData { get; }
    }
}