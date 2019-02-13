using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Data.Subscribers.Interfaces
{
    public interface IUniverseDataRequestsSubscriberFactory
    {
        IUniverseDataRequestsSubscriber Build(ISystemProcessOperationContext context);
    }
}