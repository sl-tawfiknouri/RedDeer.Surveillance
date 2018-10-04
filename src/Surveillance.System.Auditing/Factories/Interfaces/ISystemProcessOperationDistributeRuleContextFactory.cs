using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.System.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationDistributeRuleContextFactory
    {
        ISystemProcessOperationDistributeRuleContext Build(ISystemProcessOperationContext operationContext);
    }
}