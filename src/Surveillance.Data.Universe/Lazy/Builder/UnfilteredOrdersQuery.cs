namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;

    /// <summary>
    /// The unfiltered orders query.
    /// </summary>
    public class UnfilteredOrdersQuery
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
        /// Gets the start universal central time.
        /// </summary>
        public DateTime StartUtc { get;  }

        /// <summary>
        /// Gets the end universal central time.
        /// </summary>
        public DateTime EndUtc { get; }

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
    }
}
