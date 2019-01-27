using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Data.Subscribers.Interfaces
{
    public interface IUniverseDataRequestsSubscriberFactory
    {
        IUniverseDataRequestsSubscriber Build(ISystemProcessOperationContext context);
    }
}