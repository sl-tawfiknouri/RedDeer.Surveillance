namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;

    using Domain.Core.Financial.Assets;

    /// <summary>
    /// The time bar query.
    /// </summary>
    public class RefinitiveTimeBarQuery : TimeSegment<RefinitiveTimeBarQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefinitiveTimeBarQuery"/> class.
        /// </summary>
        /// <param name="startUtc">
        /// The start universe central time.
        /// </param>
        /// <param name="endUtc">
        /// The end universe central time.
        /// </param>
        /// <param name="identifiers">
        /// The identifiers.
        /// </param>
        public RefinitiveTimeBarQuery(
            DateTime startUtc,
            DateTime endUtc,
            InstrumentIdentifiers identifiers)
        {
            this.StartUtc = startUtc;
            this.EndUtc = endUtc;
            this.Identifiers = identifiers;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return (this.StartUtc.GetHashCode() * 13)
                   + (this.EndUtc.GetHashCode() * 17)
                   + (this.Identifiers.GetHashCode() * 19);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The object.
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

            var query = obj as RefinitiveTimeBarQuery;
            if (query == null)
            {
                return false;
            }

            return query.StartUtc == this.StartUtc 
                   && query.EndUtc == this.EndUtc
                   && query.Identifiers.Equals(this.Identifiers);
        }

        /// <summary>
        /// The combine.
        /// </summary>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// The <see cref="RefinitiveTimeBarQuery"/>.
        /// </returns>
        public override RefinitiveTimeBarQuery Combine(RefinitiveTimeBarQuery right)
        {
            if (right == null)
            {
                return this;
            }

            var newStartDate = this.StartUtc < right.StartUtc ? this.StartUtc : right.StartUtc;
            var newEndDate = this.EndUtc > right.EndUtc ? this.EndUtc : right.EndUtc;

            return new RefinitiveTimeBarQuery(newStartDate, newEndDate, this.Identifiers);
        }
    }
}
