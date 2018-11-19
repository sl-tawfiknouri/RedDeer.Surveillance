using Surveillance.Analytics.Subscriber.Interfaces;

namespace Surveillance.Analytics.Subscriber.Factory
{
    public class UniverseAnalyticsSubscriberFactory : IUniverseAnalyticsSubscriberFactory
    {
        public IUniverseAnalyticsSubscriber Build()
        {
            return new UniverseAnalyticsSubscriber();
        }
    }
}
