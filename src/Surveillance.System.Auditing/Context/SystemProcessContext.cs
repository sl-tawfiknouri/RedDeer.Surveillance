using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessContext : ISystemProcessContext
    {
        private ISystemProcess _systemProcess;
        
        public void StartEvent(ISystemProcess systemProcess)
        {
            _systemProcess = systemProcess;
        }

        public ISystemProcessOperationContext CreateOperationContext()
        {
            return new SystemProcessOperationContext(this);
        }
    }
}
