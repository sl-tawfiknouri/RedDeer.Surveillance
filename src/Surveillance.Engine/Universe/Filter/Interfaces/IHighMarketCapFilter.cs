namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public interface IHighMarketCapFilter
    {
        bool Filter(IUniverseEvent universeEvent);
    }
}