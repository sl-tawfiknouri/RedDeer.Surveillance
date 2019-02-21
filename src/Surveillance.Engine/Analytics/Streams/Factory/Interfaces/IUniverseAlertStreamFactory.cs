using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Streams.Factory.Interfaces
{
    public interface IUniverseAlertStreamFactory
    {
        IUniverseAlertStream Build();
    }
}