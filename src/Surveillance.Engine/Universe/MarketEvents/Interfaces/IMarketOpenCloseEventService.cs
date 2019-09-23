namespace Surveillance.Engine.Rules.Universe.MarketEvents.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IMarketOpenCloseEventService
    {
        Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end);
    }
}