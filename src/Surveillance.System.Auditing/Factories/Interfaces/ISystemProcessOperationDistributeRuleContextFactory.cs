using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationDistributeRuleContextFactory
    {
        ISystemProcessOperationDistributeRuleContext Build(ISystemProcessOperationContext operationContext);
    }
}