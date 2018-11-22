using Surveillance.Analytics.Streams.Interfaces;

namespace Surveillance.Analytics.Streams.Factory.Interfaces
{
    public interface IUniverseAlertStreamFactory
    {
        IUniverseAlertStream Build();
    }
}