using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Systems.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationDataRequestContextFactory
    {
        ISystemProcessOperationThirdPartyDataRequestContext Build(ISystemProcessOperationContext operationContext);
    }
}