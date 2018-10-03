using System;
using Surveillance.System.Auditing.Context.Interfaces;
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

        public ISystemProcessOperationDistributeRuleContext 

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
