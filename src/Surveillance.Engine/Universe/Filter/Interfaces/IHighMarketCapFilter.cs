namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using Surveillance.Data.Universe.Interfaces;

    public interface IHighMarketCapFilter
    {
        bool Filter(IUniverseEvent universeEvent);
    }
}