namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;

    /// <summary>
    /// The data manifest.
    /// </summary>
    public class DataManifest : IDataManifest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataManifest"/> class.
        /// Expects stacks to be ordered with oldest queries at the top
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
        /// <param name="refinitivIntraDayTimeBar">
        /// The thomson reuters intra day time bar query
        /// </param>
        /// <param name="refinitivInterDayTimeBar">
        /// The thomson reuters inter day time bar query
        /// </param>
        public DataManifest(
            ScheduledExecution execution,
            Stack<UnfilteredOrdersQuery> unfilteredOrders,
            Stack<BmllTimeBarQuery> bmllTimeBar,
            Stack<FactSetTimeBarQuery> factsetTimeBar,
            Stack<RefinitivIntraDayTimeBarQuery> refinitivIntraDayTimeBar,
            Stack<RefinitivInterDayTimeBarQuery> refinitivInterDayTimeBar)
        {
            this.Execution = execution ?? throw new ArgumentNullException(nameof(execution));
            this.UnfilteredOrders = unfilteredOrders ?? throw new ArgumentNullException(nameof(unfilteredOrders));
            this.BmllTimeBar = bmllTimeBar ?? throw new ArgumentNullException(nameof(bmllTimeBar));
            this.FactsetTimeBar = factsetTimeBar ?? throw new ArgumentNullException(nameof(factsetTimeBar));
            this.RefinitivIntraDayTimeBar = refinitivIntraDayTimeBar ?? throw new ArgumentNullException(nameof(refinitivIntraDayTimeBar));
            this.RefinitivInterDayTimeBar = refinitivInterDayTimeBar ?? throw new ArgumentNullException(nameof(refinitivInterDayTimeBar));
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
        /// Gets the thomson reuters intra day time bar.
        /// </summary>
        public Stack<RefinitivIntraDayTimeBarQuery> RefinitivIntraDayTimeBar { get; }

        /// <summary>
        /// Gets the thomson reuters inter day time bar.
        /// </summary>
        public Stack<RefinitivInterDayTimeBarQuery> RefinitivInterDayTimeBar { get; }
    }
}
