namespace Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces
{
    using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;

    public interface IUniverseAlertStreamSubscriberFactory
    {
        IUniverseAlertSubscriber Build(int opCtxId, bool isBackTest);
    }
}