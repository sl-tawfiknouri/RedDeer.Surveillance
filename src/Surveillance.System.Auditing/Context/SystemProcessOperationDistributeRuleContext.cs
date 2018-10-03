using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessOperationDistributeRuleContext : ISystemProcessOperationDistributeRuleContext
    {
        private readonly ISystemProcessOperationContext _processOperationContext;
        
        public SystemProcessOperationDistributeRuleContext(ISystemProcessOperationContext processOperationContext)
        {
            _processOperationContext =
                processOperationContext
                ?? throw new ArgumentNullException(nameof(processOperationContext));
        }

        public void StartEvent(ISystemProcessOperationDistributeRule distributeRule)
        {
            // save distribute rule
        }

        public ISystemProcessOperationContext EndEvent()
        {
            return _processOperationContext;
        }
    }
}
