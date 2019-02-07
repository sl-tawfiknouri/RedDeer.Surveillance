using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Systems.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationDistributeRuleContextFactory
    {
        ISystemProcessOperationDistributeRuleContext Build(ISystemProcessOperationContext operationContext);
    }
}