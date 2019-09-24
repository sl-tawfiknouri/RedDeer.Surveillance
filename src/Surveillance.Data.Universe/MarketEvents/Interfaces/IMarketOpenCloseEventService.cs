namespace Surveillance.Data.Universe.MarketEvents.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Data.Universe.Interfaces;

    public interface IMarketOpenCloseEventService
    {
        Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end);
    }
}