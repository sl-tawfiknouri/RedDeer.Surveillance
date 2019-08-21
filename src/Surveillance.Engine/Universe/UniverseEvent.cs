namespace Surveillance.Engine.Rules.Universe
{
    using System;

    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class UniverseEvent : IUniverseEvent
    {
        public UniverseEvent(UniverseStateEvent eventType, DateTime eventTime, object underlyingEvent)
        {
            this.StateChange = eventType;
            this.UnderlyingEvent = underlyingEvent ?? throw new ArgumentNullException(nameof(underlyingEvent));
            this.EventTime = eventTime;
        }

        /// <summary>
        ///     The point in time the event happened at in UTC time only
        /// </summary>
        public DateTime EventTime { get; }

        /// <summary>
        ///     The event that caused a change in the state of the universe
        /// </summary>
        public UniverseStateEvent StateChange { get; }

        /// <summary>
        ///     The underlying event of the universe
        /// </summary>
        public object UnderlyingEvent { get; }
    }
}