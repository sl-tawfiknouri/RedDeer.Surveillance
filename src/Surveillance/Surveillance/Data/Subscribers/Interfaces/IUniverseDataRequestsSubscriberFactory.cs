using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Data.Subscribers.Interfaces
{
    public interface IUniverseDataRequestsSubscriberFactory
    {
        IUniverseDataRequestsSubscriber Build(ISystemProcessOperationContext context);
    }
}