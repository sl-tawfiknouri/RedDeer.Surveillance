using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
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
        private bool _hasEnded = false;

        public SystemProcessOperationContext(
            ISystemProcessContext systemProcessContext,
            ISystemProcessOperationRepository systemProcessOperationRepository,
            ISystemProcessOperationRunRuleContextFactory runRuleContextFactory,
            ISystemProcessOperationDistributeRuleContextFactory distributeRuleContextFactory)
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

        public ISystemProcessOperationRunRuleContext CreateAndStartRuleRunContext(
            string ruleDescription,
            string ruleVersion,
            DateTime ruleScheduleBegin,
            DateTime ruleScheduleEnd)
        {
            var ctx = _runRuleContextFactory.Build(this);
            var startEvent = new SystemProcessOperationRuleRun
            {
                SystemProcessOperationId = _systemProcessOperation.Id,
                RuleDescription = ruleDescription,
                RuleVersion = ruleVersion,
                ScheduleRuleStart = ruleScheduleBegin,
                ScheduleRuleEnd = ruleScheduleEnd
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

        public ISystemProcessContext EndEventWithError()
        {
            if (_hasEnded)
            {
                return _systemProcessContext;
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
    }
}
