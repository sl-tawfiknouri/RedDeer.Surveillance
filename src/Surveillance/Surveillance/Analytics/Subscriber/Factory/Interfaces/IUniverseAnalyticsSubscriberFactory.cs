using Surveillance.Analytics.Subscriber.Interfaces;

namespace Surveillance.Analytics.Subscriber.Factory
{
    public interface IUniverseAnalyticsSubscriberFactory
    {
        IUniverseAnalyticsSubscriber Build();
    }
}