namespace Surveillance.Auditing.Context
{
    using System;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Factories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessOperationContext : ISystemProcessOperationContext
    {
        private readonly ISystemProcessOperationDataRequestContextFactory _dataRequestFactory;

        private readonly ISystemProcessOperationDistributeRuleContextFactory _distributeRuleContextFactory;

        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessOperationRunRuleContextFactory _runRuleContextFactory;

        private readonly ISystemProcessContext _systemProcessContext;

        private readonly ISystemProcessOperationRepository _systemProcessOperationRepository;

        private readonly ISystemProcessOperationFileUploadContextFactory _uploadFileFactory;

        private bool _hasEnded;

        private ISystemProcessOperation _systemProcessOperation;

        public SystemProcessOperationContext(
            ISystemProcessContext systemProcessContext,
            ISystemProcessOperationRepository systemProcessOperationRepository,
            ISystemProcessOperationRunRuleContextFactory runRuleContextFactory,
            ISystemProcessOperationDistributeRuleContextFactory distributeRuleContextFactory,
            ISystemProcessOperationFileUploadContextFactory uploadFileFactory,
            ISystemProcessOperationDataRequestContextFactory dataRequestFactory,
            IOperationLogging operationLogging)
        {
            this._systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));

            this._systemProcessOperationRepository = systemProcessOperationRepository
                                                     ?? throw new ArgumentNullException(
                                                         nameof(systemProcessOperationRepository));

            this._runRuleContextFactory =
                runRuleContextFactory ?? throw new ArgumentNullException(nameof(runRuleContextFactory));

            this._distributeRuleContextFactory = distributeRuleContextFactory
                                                 ?? throw new ArgumentNullException(
                                                     nameof(distributeRuleContextFactory));

            this._uploadFileFactory = uploadFileFactory ?? throw new ArgumentNullException(nameof(uploadFileFactory));

            this._dataRequestFactory =
                dataRequestFactory ?? throw new ArgumentNullException(nameof(dataRequestFactory));

            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public int Id => this._systemProcessOperation?.Id ?? 0;

        public ISystemProcessOperationThirdPartyDataRequestContext CreateAndStartDataRequestContext(
            string queueMessageId,
            string ruleId)
        {
            var ctx = this._dataRequestFactory.Build(this);

            var startEvent = new SystemProcessOperationThirdPartyDataRequest
                                 {
                                     SystemProcessId = this._systemProcessOperation.SystemProcessId,
                                     SystemProcessOperationId = this._systemProcessOperation.Id,
                                     QueueMessageId = queueMessageId,
                                     RuleRunId = ruleId
                                 };

            ctx.StartEvent(startEvent);

            return ctx;
        }

        public ISystemProcessOperationDistributeRuleContext CreateAndStartDistributeRuleContext(
            DateTime? initialStart,
            DateTime? initialEnd,
            string rules)
        {
            var op = new SystemProcessOperationDistributeRule
                         {
                             SystemProcessId = this._systemProcessOperation.SystemProcessId,
                             SystemProcessOperationId = this._systemProcessOperation.Id,
                             ScheduleRuleInitialStart = initialStart,
                             ScheduleRuleInitialEnd = initialEnd,
                             RulesDistributed = rules
                         };

            var ctx = this._distributeRuleContextFactory.Build(this);
            ctx.StartEvent(op);

            return ctx;
        }

        public ISystemProcessOperationRunRuleContext CreateAndStartRuleRunContext(
            string ruleDescription,
            string ruleVersion,
            string ruleParameterId,
            int ruleTypeId,
            bool isBackTest,
            DateTime ruleScheduleBegin,
            DateTime ruleScheduleEnd,
            string correlationId,
            bool ruleRunMode)
        {
            var ctx = this._runRuleContextFactory.Build(this);
            var startEvent = new SystemProcessOperationRuleRun
                                 {
                                     SystemProcessId = this._systemProcessOperation.SystemProcessId,
                                     SystemProcessOperationId = this._systemProcessOperation.Id,
                                     RuleDescription = ruleDescription,
                                     RuleVersion = ruleVersion,
                                     RuleParameterId = ruleParameterId,
                                     ScheduleRuleStart = ruleScheduleBegin,
                                     ScheduleRuleEnd = ruleScheduleEnd,
                                     CorrelationId = correlationId,
                                     IsBackTest = isBackTest,
                                     RuleTypeId = ruleTypeId,
                                     IsForceRun = ruleRunMode
                                 };

            ctx.StartEvent(startEvent);

            return ctx;
        }

        public ISystemProcessOperationUploadFileContext CreateAndStartUploadFileContext(
            SystemProcessOperationUploadFileType type,
            string filePath)
        {
            var upload = this._uploadFileFactory.Build(this);
            upload.StartEvent(
                new SystemProcessOperationUploadFile
                    {
                        FilePath = filePath,
                        FileType = (int)type,
                        SystemProcessId = this._systemProcessOperation.SystemProcessId,
                        SystemProcessOperationId = this._systemProcessOperation.Id
                    });

            return upload;
        }

        public ISystemProcessOperationDistributeRuleContext CreateDistributeRuleContext()
        {
            return this._distributeRuleContextFactory.Build(this);
        }

        public ISystemProcessOperationRunRuleContext CreateRuleRunContext()
        {
            return this._runRuleContextFactory.Build(this);
        }

        public ISystemProcessOperationUploadFileContext CreateUploadFileContext()
        {
            return this._uploadFileFactory.Build(this);
        }

        public ISystemProcessContext EndEvent()
        {
            if (this._hasEnded) return this._systemProcessContext;

            this._hasEnded = true;
            this._systemProcessOperation.OperationEnd = DateTime.UtcNow;
            this._systemProcessOperation.OperationState = OperationState.Completed;
            this._systemProcessOperationRepository.Update(this._systemProcessOperation);
            return this._systemProcessContext;
        }

        public ISystemProcessContext EndEventWithError(string message)
        {
            if (this._hasEnded) return this._systemProcessContext;

            if (!string.IsNullOrWhiteSpace(message))
                this._operationLogging.Log(new Exception(message), this._systemProcessOperation);

            this._hasEnded = true;
            this._systemProcessOperation.OperationEnd = DateTime.UtcNow;
            this._systemProcessOperation.OperationState = OperationState.CompletedWithErrors;
            this._systemProcessOperationRepository.Update(this._systemProcessOperation);
            return this._systemProcessContext;
        }

        public ISystemProcessContext EndEventWithMissingDataError()
        {
            if (this._hasEnded) return this._systemProcessContext;

            this._hasEnded = true;
            this._systemProcessOperation.OperationEnd = DateTime.UtcNow;
            this._systemProcessOperation.OperationState = OperationState.IncompleteMissingData;
            this._systemProcessOperationRepository.Update(this._systemProcessOperation);
            return this._systemProcessContext;
        }

        public void EventError(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            this._operationLogging.Log(new Exception(message), this._systemProcessOperation);
        }

        public void EventError(Exception e)
        {
            this._operationLogging.Log(e, this._systemProcessOperation);
        }

        public void StartEvent(ISystemProcessOperation processOperation)
        {
            this._systemProcessOperation = processOperation;
            this._systemProcessOperationRepository.Create(processOperation);
        }

        public ISystemProcessOperationContext UpdateEventState(OperationState state)
        {
            this._systemProcessOperation.OperationState = state;
            this._systemProcessOperationRepository.Update(this._systemProcessOperation);

            return this;
        }
    }
}