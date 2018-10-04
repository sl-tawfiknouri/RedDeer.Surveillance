using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationContext : ISystemProcessOperationContext
    {
        private readonly ISystemProcessContext _systemProcessContext;
        private ISystemProcessOperation _systemProcessOperation;

        public SystemProcessOperationContext(ISystemProcessContext systemProcessContext)
        {
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
        }

        public ISystemProcessOperationFileUploadContext CreateFileUploadContext()
        {
            return new SystemProcessOperationFileUploadContext(this);
        }

        public ISystemProcessOperationDistributeRuleContext CreateDistributeRuleContext()
        {
            return new SystemProcessOperationDistributeRuleContext(this);
        }

        public ISystemProcessOperationDistributeRuleContext CreateAndStartDistributeRuleContext(
            DateTime? initialStart,
            DateTime? initialEnd,
            string rules)
        {
            var op = new SystemProcessOperationDistributeRule
            {
                OperationId = _systemProcessOperation.Id,
                InitialStart = initialStart,
                InitialEnd = initialEnd,
                RulesDistributed = rules
            };

            var ctx = new SystemProcessOperationDistributeRuleContext(this);
            ctx.StartEvent(op);

            return ctx;
        }

        public ISystemProcessOperationRunRuleContext CreateRuleRunContext()
        {
            return new SystemProcessOperationRunRuleContext(this);
        }

        public ISystemProcessOperationRunRuleContext CreateAndStartRuleRunContext(
            string ruleDescription,
            string ruleVersion,
            DateTime ruleScheduleBegin,
            DateTime ruleScheduleEnd)
        {
            var ctx = new SystemProcessOperationRunRuleContext(this);
            var startEvent = new SystemProcessOperationRuleRun
            {
                OperationId = _systemProcessOperation.Id,
                RuleDescription = ruleDescription,
                RuleVersion = ruleVersion,
                RuleScheduleBegin = ruleScheduleBegin,
                RuleScheduleEnd = ruleScheduleEnd
            };

            ctx.StartEvent(startEvent);

            return ctx;
        }

        public void StartEvent(ISystemProcessOperation processOperation)
        {
            _systemProcessOperation = processOperation;
        }

        public ISystemProcessContext EndEvent()
        {
            return _systemProcessContext;
        }
    }
}
