namespace Surveillance.Data.Universe.Interfaces
{
    using System;

    /// <summary>
    /// The UniverseEvent interface.
    /// </summary>
    public interface IUniverseEvent
    {
        /// <summary>
        /// Gets the event time.
        /// </summary>
        DateTime EventTime { get; }

        /// <summary>
        /// Gets the state change.
        /// </summary>
        UniverseStateEvent StateChange { get; }

        /// <summary>
        /// Gets the underlying event.
        /// </summary>
        object UnderlyingEvent { get; }
    }
}