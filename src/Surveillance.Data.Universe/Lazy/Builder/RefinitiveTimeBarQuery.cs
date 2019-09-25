namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;

    using Domain.Core.Financial.Assets;

    /// <summary>
    /// The time bar query.
    /// </summary>
    public class RefinitiveTimeBarQuery
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
        /// Gets the start universal central time.
        /// </summary>
        public DateTime StartUtc { get; }

        /// <summary>
        /// Gets the end universal central time.
        /// </summary>
        public DateTime EndUtc { get; }

        /// <summary>
        /// Gets the identifiers.
        /// </summary>
        public InstrumentIdentifiers Identifiers { get; }
    }
}
