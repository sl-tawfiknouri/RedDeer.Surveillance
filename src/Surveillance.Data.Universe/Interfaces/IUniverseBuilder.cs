namespace Surveillance.Data.Universe.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;

    /// <summary>
    /// The UniverseBuilder interface.
    /// </summary>
    public interface IUniverseBuilder
    {
        /// <summary>
        /// The summon.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext operationContext);

        /// <summary>
        /// The summon.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="includeGenesis">
        /// The include genesis.
        /// </param>
        /// <param name="includeEschaton">
        /// The include eschaton.
        /// </param>
        /// <param name="realUniverseEpoch">
        /// The real universe epoch.
        /// </param>
        /// <param name="futureUniverseEpoch">
        /// The future universe epoch.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IUniverse> Summon(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            bool includeGenesis,
            bool includeEschaton,
            DateTimeOffset? realUniverseEpoch,
            DateTimeOffset? futureUniverseEpoch);

        /// <summary>
        /// The universe events.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="trades">
        /// The trades.
        /// </param>
        /// <param name="equityIntradayUpdates">
        /// The equity intraday updates.
        /// </param>
        /// <param name="equityInterDayUpdates">
        /// The equity inter day updates.
        /// </param>
        /// <param name="marketEvents">
        /// The market events open/close.
        /// </param>
        /// <param name="includeGenesis">
        /// The include genesis.
        /// </param>
        /// <param name="includeEschaton">
        /// The include eschaton.
        /// </param>
        /// <param name="realUniverseEpoch">
        /// The real universe epoch.
        /// </param>
        /// <param name="futureUniverseEpoch">
        /// The future universe epoch.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        IUniverse PackageUniverse(
            ScheduledExecution execution,
            IReadOnlyCollection<Order> trades,
            IReadOnlyCollection<EquityIntraDayTimeBarCollection> equityIntradayUpdates,
            IReadOnlyCollection<EquityInterDayTimeBarCollection> equityInterDayUpdates,
            IReadOnlyCollection<IUniverseEvent> marketEvents,
            bool includeGenesis,
            bool includeEschaton,
            DateTimeOffset? realUniverseEpoch,
            DateTimeOffset? futureUniverseEpoch);
    }
}