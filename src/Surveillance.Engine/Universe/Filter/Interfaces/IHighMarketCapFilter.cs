using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    public interface IHighMarketCapFilter
    {
        bool Filter(IUniverseEvent universeEvent);
    }
}
