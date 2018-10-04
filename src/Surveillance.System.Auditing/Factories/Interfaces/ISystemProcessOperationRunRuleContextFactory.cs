using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.System.Auditing.Factories.Interfaces
{
    public interface ISystemProcessOperationRunRuleContextFactory
    {
        ISystemProcessOperationRunRuleContext Build(ISystemProcessOperationContext context);
    }
}