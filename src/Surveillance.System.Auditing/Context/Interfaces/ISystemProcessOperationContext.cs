using System;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationContext
    {
        ISystemProcessOperationDistributeRuleContext CreateDistributeRuleContext();

        ISystemProcessOperationUploadFileContext CreateUploadFileContext();

        ISystemProcessOperationUploadFileContext CreateAndStartUploadFileContext(SystemProcessOperationUploadFileType type, string filePath);


        ISystemProcessOperationDistributeRuleContext CreateAndStartDistributeRuleContext(
            DateTime? initialStart,
            DateTime? initialEnd,
            string rules);

        ISystemProcessOperationRunRuleContext CreateRuleRunContext();

        ISystemProcessOperationRunRuleContext CreateAndStartRuleRunContext(
            string ruleDescription,
            string ruleVersion,
            string ruleParameterId,
            int ruleTypeId,
            bool isBackTest,
            DateTime ruleScheduleBegin,
            DateTime ruleScheduleEnd,
            string correlationId,
            bool ruleRunMode);

        ISystemProcessOperationThirdPartyDataRequestContext CreateAndStartDataRequestContext(
            string queueMessageId,
            string ruleId);

        ISystemProcessContext EndEvent();
        ISystemProcessContext EndEventWithError(string message);
        ISystemProcessContext EndEventWithMissingDataError();
        void StartEvent(ISystemProcessOperation processOperation);
        ISystemProcessOperationContext UpdateEventState(OperationState state);
        void EventError(string message);
        void EventError(Exception e);
        int Id { get; }
    }
}