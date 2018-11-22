using Domain.Streams;

namespace Surveillance.Analytics.Streams.Interfaces
{
    public interface IUniverseAlertStream : IPublishingStream<IUniverseAlertEvent>
    {
    }
}
