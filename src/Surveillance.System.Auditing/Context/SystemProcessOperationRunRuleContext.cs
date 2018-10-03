using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationRunRuleContext : ISystemProcessOperationRunRuleContext
    {
        private readonly ISystemProcessOperationContext _processOperationContext;
        private ISystemProcessOperationRuleRun _ruleRun;

        public SystemProcessOperationRunRuleContext(ISystemProcessOperationContext processOperationContext)
        {
            _processOperationContext =
                processOperationContext
                ?? throw new ArgumentNullException(nameof(processOperationContext));
        }

        public void StartEvent(ISystemProcessOperationRuleRun ruleRun)
        {
            _ruleRun = ruleRun;
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }
    }
}
