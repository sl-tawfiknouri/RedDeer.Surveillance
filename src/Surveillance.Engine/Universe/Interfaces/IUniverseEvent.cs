using System;

namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    public interface IUniverseEvent
    {
        UniverseStateEvent StateChange { get; }
        DateTime EventTime { get; }
        object UnderlyingEvent { get; }
    }
}