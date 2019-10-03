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
        /// The <see cref="RefinitiveTimeBarQuery"/>.
        /// </returns>
        IReadOnlyCollection<RefinitiveTimeBarQuery> Merge(IReadOnlyCollection<RefinitiveTimeBarQuery> queries);

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
