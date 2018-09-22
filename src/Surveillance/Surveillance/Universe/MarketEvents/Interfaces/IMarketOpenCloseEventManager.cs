using System;
using System.Collections.Generic;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.MarketEvents.Interfaces
{
    public interface IMarketOpenCloseEventManager
    {
        IReadOnlyCollection<IUniverseEvent> AllOpenCloseEvents(DateTime start, DateTime end);
        IReadOnlyCollection<IUniverseEvent> OpenCloseEvents(IReadOnlyCollection<string> marketIds, DateTime start, DateTime end);
    }
}