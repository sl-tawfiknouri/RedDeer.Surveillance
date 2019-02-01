using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Systems.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationRunRuleContextFactory
    {
        ISystemProcessOperationRunRuleContext Build(ISystemProcessOperationContext context);
    }
}