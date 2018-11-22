using Surveillance.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Analytics.Subscriber.Interfaces;

namespace Surveillance.Analytics.Subscriber.Factory
{
    public class UniverseAnalyticsSubscriberFactory : IUniverseAnalyticsSubscriberFactory
    {
        public IUniverseAnalyticsSubscriber Build(int operationContextId)
        {
            return new UniverseAnalyticsSubscriber(operationContextId);
        }
    }
}
