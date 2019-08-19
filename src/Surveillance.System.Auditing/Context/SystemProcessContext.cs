namespace Surveillance.Auditing.Context
{
    using System;
    using System.Diagnostics;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;
    using Surveillance.Auditing.Factories.Interfaces;
    using Surveillance.Auditing.Logging.Interfaces;

    public class SystemProcessContext : ISystemProcessContext
    {
        private readonly ISystemProcessOperationContextFactory _factory;

        private readonly IOperationLogging _operationLogging;

        private readonly ISystemProcessRepository _systemProcessRepository;

        private ISystemProcess _systemProcess;

        public SystemProcessContext(
            ISystemProcessRepository systemProcessRepository,
            ISystemProcessOperationContextFactory factory,
            IOperationLogging operationLogging)
        {
            this._systemProcessRepository = systemProcessRepository
                                            ?? throw new ArgumentNullException(nameof(systemProcessRepository));
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this._operationLogging = operationLogging ?? throw new ArgumentNullException(nameof(operationLogging));

            var systemProcess = new SystemProcess
                                    {
                                        Heartbeat = DateTime.UtcNow,
                                        InstanceInitiated = DateTime.UtcNow,
                                        MachineId = Environment.MachineName,
                                        ProcessId = Process.GetCurrentProcess()?.Id.ToString(),
                                        SystemProcessType = ProcessType
                                    };
            systemProcess.Id = systemProcess.GenerateInstanceId();

            this.StartEvent(systemProcess);
        }

        /// <summary>
        ///     Property injection
        /// </summary>
        public static SystemProcessType ProcessType { get; set; }

        public ISystemProcessOperationContext CreateAndStartOperationContext()
        {
            var op = new SystemProcessOperation
                         {
                             SystemProcessId = this._systemProcess.Id,
                             OperationStart = DateTime.UtcNow,
                             OperationState = OperationState.InProcess
                         };

            var ctx = this._factory.Build(this);
            ctx.StartEvent(op);

            return ctx;
        }

        public ISystemProcessOperationContext CreateOperationContext()
        {
            return this._factory.Build(this);
        }

        public void EventException(Exception e)
        {
            this._operationLogging.Log(e, this._systemProcess);
        }

        public void StartEvent(ISystemProcess systemProcess)
        {
            this._systemProcess = systemProcess;
            this._systemProcessRepository.Create(systemProcess);
        }

        public ISystemProcess SystemProcess()
        {
            return this._systemProcess;
        }

        public void UpdateHeartbeat()
        {
            if (this._systemProcess == null) return;

            this._systemProcess.Heartbeat = DateTime.UtcNow;
            this._systemProcessRepository.Update(this._systemProcess);
        }
    }
}