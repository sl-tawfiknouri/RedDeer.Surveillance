namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System.Collections.Generic;
    using System.Linq;

    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;

    /// <summary>
    /// The time line continuum.
    /// </summary>
    public class TimeLineContinuum : ITimeLineContinuum
    {
        /// <summary>
        /// The bmll merger.
        /// </summary>
        private readonly TimeLineMerger<BmllTimeBarQuery> BmllMerger;

        /// <summary>
        /// The fact set merger.
        /// </summary>
        private readonly TimeLineMerger<FactSetTimeBarQuery> FactSetMerger;

        /// <summary>
        /// The refinitive merger.
        /// </summary>
        private readonly TimeLineMerger<RefinitiveTimeBarQuery> RefinitiveMerger;

        /// <summary>
        /// The unfiltered orders merger.
        /// </summary>
        private readonly TimeLineMerger<UnfilteredOrdersQuery> UnfilteredOrdersMerger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeLineContinuum"/> class.
        /// </summary>
        public TimeLineContinuum()
        {
            this.BmllMerger = new TimeLineMerger<BmllTimeBarQuery>(); 
            this.FactSetMerger = new TimeLineMerger<FactSetTimeBarQuery>();
            this.RefinitiveMerger = new TimeLineMerger<RefinitiveTimeBarQuery>();
            this.UnfilteredOrdersMerger = new TimeLineMerger<UnfilteredOrdersQuery>();
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="BmllTimeBarQuery"/>.
        /// </returns>
        public IReadOnlyCollection<BmllTimeBarQuery> Merge(IReadOnlyCollection<BmllTimeBarQuery> queries)
        {
            if (queries == null || !queries.Any())
            {
                return new BmllTimeBarQuery[0];
            }

            return this.BmllMerger.Merge(queries);
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="FactSetTimeBarQuery"/>.
        /// </returns>
        public IReadOnlyCollection<FactSetTimeBarQuery> Merge(IReadOnlyCollection<FactSetTimeBarQuery> queries)
        {
            if (queries == null || !queries.Any())
            {
                return new FactSetTimeBarQuery[0];
            }

            return this.FactSetMerger.Merge(queries);
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="RefinitiveTimeBarQuery"/>.
        /// </returns>
        public IReadOnlyCollection<RefinitiveTimeBarQuery> Merge(IReadOnlyCollection<RefinitiveTimeBarQuery> queries)
        {
            if (queries == null || !queries.Any())
            {
                return new RefinitiveTimeBarQuery[0];
            }

            return this.RefinitiveMerger.Merge(queries);
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="UnfilteredOrdersQuery"/>.
        /// </returns>
        public IReadOnlyCollection<UnfilteredOrdersQuery> Merge(IReadOnlyCollection<UnfilteredOrdersQuery> queries)
        {
            if (queries == null || !queries.Any())
            {
                return new UnfilteredOrdersQuery[0];
            }

            return this.UnfilteredOrdersMerger.Merge(queries);
        }
    }
}
