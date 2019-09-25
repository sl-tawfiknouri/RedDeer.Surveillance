namespace Surveillance.Data.Universe.Interfaces
{
    using System;
    using System.Threading.Tasks;

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
    }
}