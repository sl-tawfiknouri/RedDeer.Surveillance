using System;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessContext : ISystemProcessContext
    {
        private readonly ISystemProcessRepository _systemProcessRepository;
        private ISystemProcess _systemProcess;

        public SystemProcessContext(ISystemProcessRepository systemProcessRepository)
        {
            _systemProcessRepository =
                systemProcessRepository
                ?? throw new ArgumentNullException(nameof(systemProcessRepository));
        }

        public void StartEvent(ISystemProcess systemProcess)
        {
            _systemProcess = systemProcess;
            _systemProcessRepository.Create(systemProcess);
        }

        public ISystemProcessOperationContext CreateOperationContext()
        {
            return new SystemProcessOperationContext(this);
        }
    }
}
