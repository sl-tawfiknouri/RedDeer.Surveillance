namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The time line merger.
    /// </summary>
    /// <typeparam name="T">
    /// Some form of time segment
    /// </typeparam>
    public class TimeLineMerger<T> where T : TimeSegment
    {
        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="ts">
        /// The t.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public IReadOnlyCollection<T> Merge(IReadOnlyCollection<T> ts)
        {
            if (ts == null || !ts.Any())
            {
                return new T[0];
            }

            var workingSet = new List<T>(ts);
            workingSet = workingSet.Distinct().ToList();

            if (workingSet.Count == 1)
            {
                return workingSet;
            }

            return workingSet;
        }
    }
}
