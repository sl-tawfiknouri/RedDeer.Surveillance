using DomainV2.Streams;

namespace Surveillance.Analytics.Streams.Interfaces
{
    public interface IUniverseAlertStream : IPublishingStream<IUniverseAlertEvent>
    {
    }
}
