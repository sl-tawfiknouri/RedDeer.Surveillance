using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.Auditing.Logging.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationContext : ISystemProcessOperationContext
    {
        private ISystemProcessOperation _systemProcessOperation;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ISystemProcessOperationRepository _systemProcessOperationRepository;
        private readonly ISystemProcessOperationRunRuleContextFactory _runRuleContextFactory;
        private readonly ISystemProcessOperationDistributeRuleContextFactory _distributeRuleContextFactory;
        private readonly ISystemProcessOperationFileUploadContextFactory _uploadFileFactory;
        private readonly ISystemProcessOperationDataRequestContextFactory _dataRequestFactory;
        private readonly IOperationLogging _operationLogging;
        private bool _hasEnded = false;

        public SystemProcessOperationContext(
            ISystemProcessContext systemProcessContext,
            ISystemProcessOperationRepository systemProcessOperationRepository,
            ISystemProcessOperationRunRuleContextFactory runRuleContextFactory,
            ISystemProcessOperationDistributeRuleContextFactory distributeRuleContextFactory,
            ISystemProcessOperationFileUploadContextFactory uploadFileFactory,
            ISystemProcessOperationDataRequestContextFactory dataRequestFactory,
            IOperationLogging operationLogging)
        {
            _systemProcessContext =
                systemProcessContext
                ?? throw new ArgumentNullException(nameof(systemProcessContext));

            _systemProcessOperationRepository =
                systemProcessOperationRepository
                ?? throw new ArgumentNullException(nameof(systemProcessOperationRepository));

            _runRuleContextFactory =
                runRuleContextFactory
                ?? throw new ArgumentNullException(nameof(runRuleContextFactory));

            _distributeRuleContextFactory =
                distributeRuleContextFactory
                ?? throw new ArgumentNullException(nameof(distributeRuleContextFactory));

            _uploadFileFactory =
                uploadFileFactory
                ?? throw new ArgumentNullException(nameof(uploadFileFactory));

            _dataRequestFactory =
                dataRequestFactory
                ?? throw new ArgumentNullException(nameof(dataRequestFactory));

            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));
        }

        public ISystemProcessOperationDistributeRuleContext CreateDistributeRuleContext()
        {
            return _distributeRuleContextFactory.Build(this);
        }

        public ISystemProcessOperationDistributeRuleContext CreateAndStartDistributeRuleContext(
            DateTime? initialStart,
            DateTime? initialEnd,
            string rules)
        {
            var op = new SystemProcessOperationDistributeRule
            {
                SystemProcessId = _systemProcessOperation.SystemProcessId,
                SystemProcessOperationId = _systemProcessOperation.Id,
                ScheduleRuleInitialStart = initialStart,
                ScheduleRuleInitialEnd = initialEnd,
                RulesDistributed = rules
            };

            var ctx = _distributeRuleContextFactory.Build(this);
            ctx.StartEvent(op);

            return ctx;
        }

        public ISystemProcessOperationRunRuleContext CreateRuleRunContext()
        {
            return _runRuleContextFactory.Build(this);
        }

        public ISystemProcessOperationUploadFileContext CreateUploadFileContext()
        {
            return _uploadFileFactory.Build(this);
        }

        public ISystemProcessOperationUploadFileContext CreateAndStartUploadFileContext(
            SystemProcessOperationUploadFileType type,
            string filePath)
        {
            var upload = _uploadFileFactory.Build(this);
            upload.StartEvent(new SystemProcessOperationUploadFile
            {
                FilePath = filePath,
                FileType = (int) type,
                SystemProcessId = _systemProcessOperation.SystemProcessId,
                SystemProcessOperationId = _systemProcessOperation.Id
            });

            return upload;
        }

        public ISystemProcessOperationRunRuleContext CreateAndStartRuleRunContext(
            string ruleDescription,
            string ruleVersion,
            DateTime ruleScheduleBegin,
            DateTime ruleScheduleEnd,
            string correlationId)
        {
            var ctx = _runRuleContextFactory.Build(this);
            var startEvent = new SystemProcessOperationRuleRun
            {
                SystemProcessId = _systemProcessOperation.SystemProcessId,
                SystemProcessOperationId = _systemProcessOperation.Id,
                RuleDescription = ruleDescription,
                RuleVersion = ruleVersion,
                ScheduleRuleStart = ruleScheduleBegin,
                ScheduleRuleEnd = ruleScheduleEnd,
                CorrelationId = correlationId
            };

            ctx.StartEvent(startEvent);

            return ctx;
        }

        public ISystemProcessOperationThirdPartyDataRequestContext CreateAndStartDataRequestContext(
            string queueMessageId,
            string ruleId)
        {
            var ctx = _dataRequestFactory.Build(this);

            var startEvent = new SystemProcessOperationThirdPartyDataRequest
            {
                SystemProcessId = _systemProcessOperation.SystemProcessId,
                SystemProcessOperationId = _systemProcessOperation.Id,
                QueueMessageId = queueMessageId,
                RuleId = ruleId
            };

            ctx.StartEvent(startEvent);

            return ctx;
        }

        public void StartEvent(ISystemProcessOperation processOperation)
        {
            _systemProcessOperation = processOperation;
            _systemProcessOperationRepository.Create(processOperation);
        }

        public ISystemProcessOperationContext UpdateEventState(OperationState state)
        {
            _systemProcessOperation.OperationState = state;
            _systemProcessOperationRepository.Update(_systemProcessOperation);

            return this;
        }

        public ISystemProcessContext EndEvent()
        {
            if (_hasEnded)
            {
                return _systemProcessContext;
            }

            _hasEnded = true;
            _systemProcessOperation.OperationEnd = DateTime.UtcNow;
            _systemProcessOperation.OperationState = OperationState.Completed;
            _systemProcessOperationRepository.Update(_systemProcessOperation);
            return _systemProcessContext;
        }

        public void EventError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _operationLogging.Log(new Exception(message), _systemProcessOperation);
        }

        public void EventError(Exception e)
        {
            _operationLogging.Log(e, _systemProcessOperation);
        }

        public ISystemProcessContext EndEventWithError(string message)
        {
            if (_hasEnded)
            {
                return _systemProcessContext;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                _operationLogging.Log(new Exception(message), _systemProcessOperation);
            }

            _hasEnded = true;
            _systemProcessOperation.OperationEnd = DateTime.UtcNow;
            _systemProcessOperation.OperationState = OperationState.CompletedWithErrors;
            _systemProcessOperationRepository.Update(_systemProcessOperation);
            return _systemProcessContext;
        }

        public ISystemProcessContext EndEventWithMissingDataError()
        {
            if (_hasEnded)
            {
                return _systemProcessContext;
            }

            _hasEnded = true;
            _systemProcessOperation.OperationEnd = DateTime.UtcNow;
            _systemProcessOperation.OperationState = OperationState.IncompleteMissingData;
            _systemProcessOperationRepository.Update(_systemProcessOperation);
            return _systemProcessContext;
        }

        public int Id => _systemProcessOperation?.Id ?? 0;
    }
}
