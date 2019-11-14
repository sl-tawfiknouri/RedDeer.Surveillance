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
        /// The refinitive intra day merger.
        /// </summary>
        private readonly TimeLineMerger<RefinitivIntraDayTimeBarQuery> RefinitivIntraDayMerger;

        /// <summary>
        /// The refinitive inter day merger.
        /// </summary>
        private readonly TimeLineMerger<RefinitivInterDayTimeBarQuery> RefinitivInterDayMerger;

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
            this.RefinitivIntraDayMerger = new TimeLineMerger<RefinitivIntraDayTimeBarQuery>();
            this.RefinitivInterDayMerger = new TimeLineMerger<RefinitivInterDayTimeBarQuery>();
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
        /// The <see cref="RefinitivIntraDayTimeBarQuery"/>.
        /// </returns>
        public IReadOnlyCollection<RefinitivIntraDayTimeBarQuery> Merge(IReadOnlyCollection<RefinitivIntraDayTimeBarQuery> queries)
        {
            if (queries == null || !queries.Any())
            {
                return new RefinitivIntraDayTimeBarQuery[0];
            }

            return this.RefinitivIntraDayMerger.Merge(queries);
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="RefinitivInterDayTimeBarQuery"/>.
        /// </returns>
        public IReadOnlyCollection<RefinitivInterDayTimeBarQuery> Merge(IReadOnlyCollection<RefinitivInterDayTimeBarQuery> queries)
        {
            if (queries == null || !queries.Any())
            {
                return new RefinitivInterDayTimeBarQuery[0];
            }

            return this.RefinitivInterDayMerger.Merge(queries);
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
