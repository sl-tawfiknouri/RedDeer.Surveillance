using System;
using System.Diagnostics;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
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

            var systemProcess = new SystemProcess
            {
                Heartbeat = DateTime.UtcNow,
                InstanceInitiated = DateTime.UtcNow,
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess()?.Id.ToString(),
            };
            systemProcess.InstanceId = systemProcess.GenerateInstanceId();

            StartEvent(systemProcess);
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

        public ISystemProcessOperationContext CreateAndStartOperationContext()
        {
            var op = new SystemProcessOperation
            {
                InstanceId = _systemProcess.InstanceId,
                OperationStart = DateTime.UtcNow,
                OperationEnd = DateTime.UtcNow,
                OperationState = OperationState.InProcess
            };

            var ctx = new SystemProcessOperationContext(this);
            ctx.StartEvent(op);

            return ctx;
        }
    }
}