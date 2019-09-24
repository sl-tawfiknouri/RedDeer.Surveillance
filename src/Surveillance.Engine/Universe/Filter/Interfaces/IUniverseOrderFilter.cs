namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using Surveillance.Data.Universe.Interfaces;

    public interface IUniverseOrderFilter
    {
        IUniverseEvent Filter(IUniverseEvent universeEvent);
    }
}