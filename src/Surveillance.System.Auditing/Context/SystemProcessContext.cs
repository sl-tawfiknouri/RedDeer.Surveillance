using System;
using System.Diagnostics;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.Auditing.Factories.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Processes.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.Auditing.Context
{
    public class SystemProcessContext : ISystemProcessContext
    {
        private readonly ISystemProcessRepository _systemProcessRepository;
        private readonly ISystemProcessOperationContextFactory _factory;
        private ISystemProcess _systemProcess;

        public SystemProcessContext(
            ISystemProcessRepository systemProcessRepository,
            ISystemProcessOperationContextFactory factory)
        {
            _systemProcessRepository =
                systemProcessRepository
                ?? throw new ArgumentNullException(nameof(systemProcessRepository));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            var systemProcess = new SystemProcess
            {
                Heartbeat = DateTime.UtcNow,
                InstanceInitiated = DateTime.UtcNow,
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess()?.Id.ToString(),
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            StartEvent(systemProcess);
        }

        public void StartEvent(ISystemProcess systemProcess)
        {
            _systemProcess = systemProcess;
            _systemProcessRepository.Create(systemProcess);
        }

        public ISystemProcessOperationContext CreateOperationContext()
        {
            return _factory.Build(this);
        }

        public ISystemProcessOperationContext CreateAndStartOperationContext()
        {
            var op = new SystemProcessOperation
            {
                SystemProcessId = _systemProcess.Id,
                OperationStart = DateTime.UtcNow,
                OperationState = OperationState.InProcess
            };

            var ctx = _factory.Build(this);
            ctx.StartEvent(op);

            return ctx;
        }
    }
}