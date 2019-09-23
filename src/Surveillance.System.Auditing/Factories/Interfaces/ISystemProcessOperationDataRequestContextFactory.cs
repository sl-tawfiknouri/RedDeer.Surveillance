namespace Surveillance.Auditing.Factories.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;

    public interface ISystemProcessOperationDataRequestContextFactory
    {
        ISystemProcessOperationThirdPartyDataRequestContext Build(ISystemProcessOperationContext operationContext);
    }
}