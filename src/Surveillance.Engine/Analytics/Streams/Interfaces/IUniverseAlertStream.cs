using DomainV2.Streams;

namespace Surveillance.Engine.Rules.Analytics.Streams.Interfaces
{
    public interface IUniverseAlertStream : IPublishingStream<IUniverseAlertEvent>
    {
    }
}
