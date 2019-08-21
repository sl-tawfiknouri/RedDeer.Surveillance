namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IUniverseOrderFilter
    {
        IUniverseEvent Filter(IUniverseEvent universeEvent);
    }
}