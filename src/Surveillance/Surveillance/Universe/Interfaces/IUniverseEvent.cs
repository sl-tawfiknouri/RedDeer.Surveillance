using System;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseEvent
    {
        UniverseStateEvent StateChange { get; }
        DateTime EventTime { get; }
        object UnderlyingEvent { get; }
    }
}