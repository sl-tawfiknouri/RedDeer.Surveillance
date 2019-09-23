namespace Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces
{
    using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;

    public interface IUniverseAnalyticsSubscriberFactory
    {
        IUniverseAnalyticsSubscriber Build(int operationContextId);
    }
}