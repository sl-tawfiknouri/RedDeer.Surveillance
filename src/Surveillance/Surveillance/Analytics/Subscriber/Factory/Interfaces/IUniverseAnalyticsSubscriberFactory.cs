using Surveillance.Analytics.Subscriber.Interfaces;

namespace Surveillance.Analytics.Subscriber.Factory.Interfaces
{
    public interface IUniverseAnalyticsSubscriberFactory
    {
        IUniverseAnalyticsSubscriber Build(int operationContextId);
    }
}