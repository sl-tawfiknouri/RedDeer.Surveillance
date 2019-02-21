using Domain.Streams;

namespace Surveillance.Engine.Rules.Analytics.Streams.Interfaces
{
    public interface IUniverseAlertStream : IPublishingStream<IUniverseAlertEvent>
    {
    }
}
