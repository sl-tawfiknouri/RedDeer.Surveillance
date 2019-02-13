using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.MarketEvents.Interfaces
{
    public interface IMarketOpenCloseEventManager
    {
        Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end);
    }
}