namespace Surveillance.Data.Universe.Lazy.Builder.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The DataManifest interface.
    /// </summary>
    public interface IDataManifest
    {
        /// <summary>
        /// Gets the execution.
        /// </summary>
        ScheduledExecution Execution { get; }

        /// <summary>
        /// Gets the unfiltered orders.
        /// </summary>
        Stack<UnfilteredOrdersQuery> UnfilteredOrders { get; }

        /// <summary>
        /// Gets the time bar.
        /// </summary>
        Stack<BmllTimeBarQuery> BmllTimeBar { get; }

        /// <summary>
        /// Gets the fact set time bar.
        /// </summary>
        Stack<FactSetTimeBarQuery> FactsetTimeBar { get; }

        /// <summary>
        /// Gets the thomson reuters time bar.
        /// </summary>
        Stack<RefinitiveTimeBarQuery> RefinitiveTimeBar { get; }
    }
}
