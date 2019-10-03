namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;

    using Domain.Core.Financial.Assets;

    /// <summary>
    /// The time bar query.
    /// </summary>
    public class BmllTimeBarQuery : TimeSegment<BmllTimeBarQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BmllTimeBarQuery"/> class.
        /// </summary>
        /// <param name="startUtc">
        /// The start universal central time.
        /// </param>
        /// <param name="endUtc">
        /// The end universal central time.
        /// </param>
        /// <param name="identifiers">
        /// The identifiers.
        /// </param>
        public BmllTimeBarQuery(
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
            return 
                (this.StartUtc.GetHashCode() * 13)
                + (this.EndUtc.GetHashCode() * 19)
                + (this.Identifiers.GetHashCode() * 3);
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

            var query = obj as BmllTimeBarQuery;
            if (query == null)
            {
                return false;
            }

            return 
                query.StartUtc == this.StartUtc
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
        /// The <see cref="BmllTimeBarQuery"/>.
        /// </returns>
        public override BmllTimeBarQuery Combine(BmllTimeBarQuery right)
        {
            if (right == null)
            {
                return this;
            }

            var newStartDate = this.StartUtc < right.StartUtc ? this.StartUtc : right.StartUtc;
            var newEndDate = this.EndUtc > right.EndUtc ? this.EndUtc : right.EndUtc;

            return new BmllTimeBarQuery(newStartDate, newEndDate, this.Identifiers);
        }
    }
}
