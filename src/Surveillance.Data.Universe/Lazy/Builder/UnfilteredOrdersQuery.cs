namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;

    /// <summary>
    /// The unfiltered orders query.
    /// </summary>
    public class UnfilteredOrdersQuery : TimeSegment<UnfilteredOrdersQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnfilteredOrdersQuery"/> class.
        /// </summary>
        /// <param name="startUtc">
        /// The start universe central time.
        /// </param>
        /// <param name="endUtc">
        /// The end in universal central time.
        /// </param>
        public UnfilteredOrdersQuery(DateTime startUtc, DateTime endUtc)
        {
            this.StartUtc = startUtc;
            this.EndUtc = endUtc;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return 
                (this.StartUtc.GetHashCode() * 13) 
                + (this.EndUtc.GetHashCode() * 19);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var query = obj as UnfilteredOrdersQuery;
            if (query == null)
            {
                return false;
            }

            return this.StartUtc == query.StartUtc 
                   && this.EndUtc == query.EndUtc;
        }

        /// <summary>
        /// The combine.
        /// </summary>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// The <see cref="UnfilteredOrdersQuery"/>.
        /// </returns>
        public override UnfilteredOrdersQuery Combine(UnfilteredOrdersQuery right)
        {
            if (right == null)
            {
                return this;
            }

            var newStartDate = this.StartUtc < right.StartUtc ? this.StartUtc : right.StartUtc;
            var newEndDate = this.EndUtc > right.EndUtc ? this.EndUtc : right.EndUtc;

            return new UnfilteredOrdersQuery(newStartDate, newEndDate);
        }
    }
}
