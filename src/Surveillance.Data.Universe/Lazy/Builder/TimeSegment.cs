namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;

    using Domain.Core.Financial.Assets;

    /// <summary>
    /// The time segment.
    /// </summary>
    /// <typeparam name="T">
    /// Derived class
    /// </typeparam>
    public abstract class TimeSegment<T>
    {
        /// <summary>
        /// Gets or sets the start universal central time.
        /// </summary>
        public DateTime StartUtc { get; protected set; }

        /// <summary>
        /// Gets or sets the end universal central time.
        /// </summary>
        public DateTime EndUtc { get; protected set; }

        /// <summary>
        /// Gets or sets the identifiers.
        /// </summary>
        public InstrumentIdentifiers Identifiers { get; protected set; }

        /// <summary>
        /// The combine.
        /// </summary>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public abstract T Combine(T right);
    }
}
