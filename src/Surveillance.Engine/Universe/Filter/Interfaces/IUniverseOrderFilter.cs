using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    public interface IUniverseOrderFilter
    {
        IUniverseEvent Filter(IUniverseEvent universeEvent);
    }
}