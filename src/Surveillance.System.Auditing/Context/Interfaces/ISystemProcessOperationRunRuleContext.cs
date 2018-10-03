using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationRunRuleContext
    {
        ISystemProcessOperationContext EndEvent();
        void StartEvent(ISystemProcessOperationRuleRun ruleRun);
    }
}