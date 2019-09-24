namespace Surveillance.Data.Universe.Interfaces
{
    using System;

    public interface IUniverseEvent
    {
        DateTime EventTime { get; }

        UniverseStateEvent StateChange { get; }

        object UnderlyingEvent { get; }
    }
}