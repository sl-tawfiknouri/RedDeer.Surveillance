using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.MarketEvents.Interfaces
{
    public interface IMarketOpenCloseEventManager
    {
        Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end);
    }
}