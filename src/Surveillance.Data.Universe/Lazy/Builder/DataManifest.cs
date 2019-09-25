namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The data manifest.
    /// </summary>
    public class DataManifest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataManifest"/> class.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="unfilteredOrders">
        /// The unfiltered orders
        /// </param>
        /// <param name="bmllTimeBar">
        /// The time bar query
        /// </param>
        /// <param name="factsetTimeBar">
        /// The fact set time bar query
        /// </param>
        /// <param name="refinitiveTimeBar">
        /// The thomson reuters time bar query
        /// </param>
        public DataManifest(
            ScheduledExecution execution,
            Stack<UnfilteredOrdersQuery> unfilteredOrders,
            Stack<BmllTimeBarQuery> bmllTimeBar,
            Stack<FactSetTimeBarQuery> factsetTimeBar,
            Stack<RefinitiveTimeBarQuery> refinitiveTimeBar)
        {
            this.Execution = execution ?? throw new ArgumentNullException(nameof(execution));
            this.UnfilteredOrders = unfilteredOrders ?? throw new ArgumentNullException(nameof(unfilteredOrders));
            this.BmllTimeBar = bmllTimeBar ?? throw new ArgumentNullException(nameof(bmllTimeBar));
            this.FactsetTimeBar = factsetTimeBar ?? throw new ArgumentNullException(nameof(factsetTimeBar));
            this.RefinitiveTimeBar = refinitiveTimeBar ?? throw new ArgumentNullException(nameof(refinitiveTimeBar));
        }

        /// <summary>
        /// Gets the execution.
        /// </summary>
        public ScheduledExecution Execution { get; }

        /// <summary>
        /// Gets the unfiltered orders.
        /// </summary>
        public Stack<UnfilteredOrdersQuery> UnfilteredOrders { get; }

        /// <summary>
        /// Gets the time bar.
        /// </summary>
        public Stack<BmllTimeBarQuery> BmllTimeBar { get; }

        /// <summary>
        /// Gets the fact set time bar.
        /// </summary>
        public Stack<FactSetTimeBarQuery> FactsetTimeBar { get; }

        /// <summary>
        /// Gets the thomson reuters time bar.
        /// </summary>
        public Stack<RefinitiveTimeBarQuery> RefinitiveTimeBar { get; }
    }
}
