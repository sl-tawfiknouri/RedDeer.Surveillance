using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Filter.Interfaces
{
    public interface IUniverseOrderFilter
    {
        IUniverseEvent Filter(IUniverseEvent universeEvent);
    }
}