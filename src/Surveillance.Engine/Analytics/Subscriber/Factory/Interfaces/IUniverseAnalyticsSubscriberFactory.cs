using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces
{
    public interface IUniverseAnalyticsSubscriberFactory
    {
        IUniverseAnalyticsSubscriber Build(int operationContextId);
    }
}