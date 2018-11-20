using Surveillance.Analytics.Subscriber.Interfaces;

namespace Surveillance.Analytics.Subscriber.Factory.Interfaces
{
    public interface IUniverseAlertStreamSubscriberFactory
    {
        IUniverseAlertSubscriber Build();
    }
}