using System;
using System.Diagnostics;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
using Surveillance.Auditing.Factories.Interfaces;
using Surveillance.Auditing.Logging.Interfaces;

namespace Surveillance.Auditing.Context
{
    public class SystemProcessContext : ISystemProcessContext
    {
        private readonly IOperationLogging _operationLogging;
        private readonly ISystemProcessRepository _systemProcessRepository;
        private readonly ISystemProcessOperationContextFactory _factory;
        private ISystemProcess _systemProcess;

        public SystemProcessContext(
            ISystemProcessRepository systemProcessRepository,
            ISystemProcessOperationContextFactory factory,
            IOperationLogging operationLogging)
        {
            _systemProcessRepository =
                systemProcessRepository
                ?? throw new ArgumentNullException(nameof(systemProcessRepository));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));

            var systemProcess = new SystemProcess
            {
                Heartbeat = DateTime.UtcNow,
                InstanceInitiated = DateTime.UtcNow,
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess()?.Id.ToString(),
                SystemProcessType = ProcessType
            };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            StartEvent(systemProcess);
        }

        /// <summary>
        /// Property injection
        /// </summary>
        public static SystemProcessType ProcessType { get; set; }

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

        public void EventException(Exception e)
        {
            _operationLogging.Log(e, _systemProcess);
        }

        public void UpdateHeartbeat()
        {
            if (_systemProcess == null)
            {
                return;
            }

            _systemProcess.Heartbeat = DateTime.UtcNow;
            _systemProcessRepository.Update(_systemProcess);
        }

        public ISystemProcess SystemProcess()
        {
            return _systemProcess;
        }
    }
}