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
    public class TimeLineMerger<T> where T : TimeSegment<T>
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

            workingSet = workingSet.OrderBy(_ => _.StartUtc).ToList();

            var segmentStack = new Stack<T>();
            foreach (var item in workingSet)
            {
                if (!segmentStack.Any())
                {
                    segmentStack.Push(item);
                    continue;
                }

                var topItem = segmentStack.Peek();
                var hasIntersection = this.TimeSegmentIntersect(topItem, item);

                if (!hasIntersection)
                {
                    segmentStack.Push(item);
                    continue;
                }

                var mergeItem = segmentStack.Pop();
                var newTimeSegment = mergeItem.Combine(item);
                segmentStack.Push(newTimeSegment);
            }

            var sortedList = segmentStack.ToList().OrderBy(_ => _.StartUtc).ToList();

            return sortedList;
        }

        /// <summary>
        /// The time segment intersect.
        /// </summary>
        /// <param name="segmentLeft">
        /// The segment left.
        /// </param>
        /// <param name="segmentRight">
        /// The segment right.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TimeSegmentIntersect(T segmentLeft, T segmentRight)
        {
            if (segmentLeft.StartUtc == segmentRight.StartUtc)
            {
                return true;
            }

            if (segmentLeft.EndUtc == segmentRight.EndUtc)
            {
                return true;
            }

            var leadingEdge = segmentLeft.StartUtc < segmentRight.StartUtc ? segmentLeft : segmentRight;
            var trailingEdge = segmentLeft.StartUtc < segmentRight.StartUtc ? segmentRight : segmentLeft;

            return leadingEdge.EndUtc >= trailingEdge.StartUtc;
        }
    }
}
