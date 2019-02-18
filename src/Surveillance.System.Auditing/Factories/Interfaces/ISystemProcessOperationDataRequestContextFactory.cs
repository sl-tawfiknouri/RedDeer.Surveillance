using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationDataRequestContextFactory
    {
        ISystemProcessOperationThirdPartyDataRequestContext Build(ISystemProcessOperationContext operationContext);
    }
}