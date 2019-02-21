using System;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    public class UniverseEvent : IUniverseEvent
    {
        public UniverseEvent(UniverseStateEvent eventType, DateTime eventTime, object underlyingEvent)
        {
            StateChange = eventType;
            UnderlyingEvent = underlyingEvent ?? throw new ArgumentNullException(nameof(underlyingEvent));
            EventTime = eventTime;
        }

        /// <summary>
        /// The event that caused a change in the state of the universe
        /// </summary>
        public UniverseStateEvent StateChange { get; }

        /// <summary>
        /// The point in time the event happened at in UTC time only
        /// </summary>
        public DateTime EventTime { get; }

        /// <summary>
        /// The underlying event of the universe
        /// </summary>
        public object UnderlyingEvent { get; }
    }
}