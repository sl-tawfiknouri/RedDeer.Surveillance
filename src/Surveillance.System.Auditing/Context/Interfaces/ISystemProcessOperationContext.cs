namespace Surveillance.Auditing.Context.Interfaces
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationContext
    {
        int Id { get; }

        ISystemProcessOperationThirdPartyDataRequestContext CreateAndStartDataRequestContext(
            string queueMessageId,
            string ruleId);

        ISystemProcessOperationDistributeRuleContext CreateAndStartDistributeRuleContext(
            DateTime? initialStart,
            DateTime? initialEnd,
            string rules);

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

        ISystemProcessOperationUploadFileContext CreateAndStartUploadFileContext(
            SystemProcessOperationUploadFileType type,
            string filePath);

        ISystemProcessOperationDistributeRuleContext CreateDistributeRuleContext();

        ISystemProcessOperationRunRuleContext CreateRuleRunContext();

        ISystemProcessOperationUploadFileContext CreateUploadFileContext();

        ISystemProcessContext EndEvent();

        ISystemProcessContext EndEventWithError(string message);

        ISystemProcessContext EndEventWithMissingDataError();

        void EventError(string message);

        void EventError(Exception e);

        void StartEvent(ISystemProcessOperation processOperation);

        ISystemProcessOperationContext UpdateEventState(OperationState state);
    }
}