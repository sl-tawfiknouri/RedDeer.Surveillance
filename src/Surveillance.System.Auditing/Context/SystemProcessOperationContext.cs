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
