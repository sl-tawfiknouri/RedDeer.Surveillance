namespace Surveillance.Engine.Rules.Data.Subscribers.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;

    public interface IUniverseDataRequestsSubscriberFactory
    {
        IUniverseDataRequestsSubscriber Build(ISystemProcessOperationContext context);
    }
}