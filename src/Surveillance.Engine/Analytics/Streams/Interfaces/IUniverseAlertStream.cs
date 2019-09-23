namespace Surveillance.Engine.Rules.Analytics.Streams.Interfaces
{
    using Domain.Surveillance.Streams;

    public interface IUniverseAlertStream : IPublishingStream<IUniverseAlertEvent>
    {
    }
}