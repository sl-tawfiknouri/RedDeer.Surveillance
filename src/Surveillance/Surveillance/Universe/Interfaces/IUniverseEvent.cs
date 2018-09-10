namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseEvent
    {
        UniverseStateEvent StateChange { get; }
        object UnderlyingEvent { get; }
    }
}