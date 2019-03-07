using Domain.Surveillance.Streams;

namespace Surveillance.Engine.Rules.Analytics.Streams.Interfaces
{
    public interface IUniverseAlertStream : IPublishingStream<IUniverseAlertEvent>
    {
    }
}
