namespace Surveillance.Data.Universe.Lazy.Builder.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// The TimeLineContinuum interface.
    /// </summary>
    public interface ITimeLineContinuum
    {
        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="BmllTimeBarQuery"/>.
        /// </returns>
        IReadOnlyCollection<BmllTimeBarQuery> Merge(IReadOnlyCollection<BmllTimeBarQuery> queries);

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="FactSetTimeBarQuery"/>.
        /// </returns>
        IReadOnlyCollection<FactSetTimeBarQuery> Merge(IReadOnlyCollection<FactSetTimeBarQuery> queries);

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="RefinitivIntraDayTimeBarQuery"/>.
        /// </returns>
        IReadOnlyCollection<RefinitivIntraDayTimeBarQuery> Merge(IReadOnlyCollection<RefinitivIntraDayTimeBarQuery> queries);

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="RefinitivInterDayTimeBarQuery"/>.
        /// </returns>
        IReadOnlyCollection<RefinitivInterDayTimeBarQuery> Merge(IReadOnlyCollection<RefinitivInterDayTimeBarQuery> queries);

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="UnfilteredOrdersQuery"/>.
        /// </returns>
        IReadOnlyCollection<UnfilteredOrdersQuery> Merge(IReadOnlyCollection<UnfilteredOrdersQuery> queries);
    }
}
