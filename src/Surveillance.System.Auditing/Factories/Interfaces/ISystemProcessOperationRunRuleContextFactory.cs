using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationRunRuleContextFactory
    {
        ISystemProcessOperationRunRuleContext Build(ISystemProcessOperationContext context);
    }
}