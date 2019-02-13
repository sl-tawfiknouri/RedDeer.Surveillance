using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces
{
    public interface IUniverseAlertStreamSubscriberFactory
    {
        IUniverseAlertSubscriber Build(int opCtxId, bool isBackTest);
    }
}