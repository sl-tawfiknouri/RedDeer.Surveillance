using System;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationContext
    {
        ISystemProcessOperationDistributeRuleContext CreateDistributeRuleContext();

        ISystemProcessOperationDistributeRuleContext CreateAndStartDistributeRuleContext(
            DateTime? initialStart,
            DateTime? initialEnd,
            string rules);

        ISystemProcessOperationRunRuleContext CreateRuleRunContext();

        ISystemProcessOperationRunRuleContext CreateAndStartRuleRunContext(
            string ruleDescription,
            string ruleVersion,
            DateTime ruleScheduleBegin,
            DateTime ruleScheduleEnd);

        ISystemProcessContext EndEvent();
        ISystemProcessContext EndEventWithError();
        void StartEvent(ISystemProcessOperation processOperation);
        ISystemProcessOperationContext UpdateEventState(OperationState state);
    }
}