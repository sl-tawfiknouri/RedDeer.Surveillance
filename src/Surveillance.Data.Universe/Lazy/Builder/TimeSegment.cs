namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;

    using Domain.Core.Financial.Assets;

    /// <summary>
    /// The time segment.
    /// </summary>
    public abstract class TimeSegment
    {
        /// <summary>
        /// Gets the start universal central time.
        /// </summary>
        public DateTime StartUtc { get; protected set; }

        /// <summary>
        /// Gets the end universal central time.
        /// </summary>
        public DateTime EndUtc { get; protected set; }

        /// <summary>
        /// Gets the identifiers.
        /// </summary>
        public InstrumentIdentifiers Identifiers { get; protected set; }
    }
}
