using System;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    public class UniverseEvent : IUniverseEvent
    {
        public UniverseEvent(UniverseStateEvent eventType, object underlyingEvent)
        {
            StateChange = eventType;
            UnderlyingEvent = underlyingEvent ?? throw new ArgumentNullException(nameof(underlyingEvent));
        }


        /// <summary>
        /// The event that caused a change in the state of the universe
        /// </summary>
        public UniverseStateEvent StateChange { get; }

        /// <summary>
        /// The underlying event of the universe
        /// </summary>
        public object UnderlyingEvent { get; }
    }
}