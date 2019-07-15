using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    /// <summary>
    /// High volume as a (%) of venue trading
    /// </summary>
    public interface IHighVolumeVenueFilter
    {
        bool Filter(IUniverseEvent universeEvent);
    }
}
