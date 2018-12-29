using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.System.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationDataRequestContextFactory
    {
        ISystemProcessOperationThirdPartyDataRequestContext Build(ISystemProcessOperationContext operationContext);
    }
}