namespace Surveillance.Data.Universe.MarketEvents.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The MarketOpenCloseEventService interface.
    /// </summary>
    public interface IMarketOpenCloseEventService
    {
        /// <summary>
        /// The all open close events.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end);
    }
}