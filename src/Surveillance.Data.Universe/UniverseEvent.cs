namespace Surveillance.Data.Universe
{
    using System;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The universe event.
    /// </summary>
    public class UniverseEvent : IUniverseEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseEvent"/> class.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="eventTime">
        /// The event time.
        /// </param>
        /// <param name="underlyingEvent">
        /// The underlying event.
        /// </param>
        public UniverseEvent(UniverseStateEvent eventType, DateTime eventTime, object underlyingEvent)
        {
            this.StateChange = eventType;
            this.UnderlyingEvent = underlyingEvent ?? throw new ArgumentNullException(nameof(underlyingEvent));
            this.EventTime = eventTime;
        }

        /// <summary>
        /// Gets the point in time the event happened at in UTC time only
        /// </summary>
        public DateTime EventTime { get; }

        /// <summary>
        /// Gets the event that caused a change in the state of the universe
        /// </summary>
        public UniverseStateEvent StateChange { get; }

        /// <summary>
        /// Gets the underlying event of the universe
        /// </summary>
        public object UnderlyingEvent { get; }
    }
}