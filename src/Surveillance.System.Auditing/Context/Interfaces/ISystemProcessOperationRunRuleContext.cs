using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationRunRuleContext
    {
        void StartEvent(ISystemProcessOperationRuleRun ruleRun);
        ISystemProcessOperationRunRuleContext UpdateAlertEvent(int alerts);
        ISystemProcessOperationContext EndEvent();
    }
}