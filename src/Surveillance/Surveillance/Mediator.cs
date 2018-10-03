using Surveillance.Services.Interfaces;
using System;
using System.Diagnostics;
using Surveillance.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance
{
    /// <summary>
    /// The mediator orchestrates program components; factory; services; display
    /// This represents the root entry into the 'real' surveillance object graph
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly IReddeerTradeService _reddeerTradeService;
        private readonly IReddeerRuleScheduler _ruleScheduler;
        private readonly IReddeerDistributedRuleScheduler _distributedRuleScheduler;
        private readonly ISystemProcessRepository _processRepository;
        private ISystemProcessContext _systemProcessContext;

        public Mediator(
            IReddeerTradeService reddeerTradeService,
            IReddeerRuleScheduler ruleScheduler,
            IReddeerDistributedRuleScheduler distributedRuleScheduler,
            ISystemProcessRepository processRepository)
        {
            _reddeerTradeService =
                reddeerTradeService 
                ?? throw new ArgumentNullException(nameof(reddeerTradeService));
            _ruleScheduler =
                ruleScheduler
                ?? throw new ArgumentNullException(nameof(ruleScheduler));
            _distributedRuleScheduler =
                distributedRuleScheduler
                ?? throw new ArgumentNullException(nameof(distributedRuleScheduler));
            _processRepository =
                processRepository
                ?? throw new ArgumentNullException(nameof(processRepository));
        }

        public void Initiate()
        {
            var systemProcess = new SystemProcess
            {
                Heartbeat = DateTime.UtcNow,
                InstanceInitiated = DateTime.UtcNow,
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess()?.Id.ToString(),
            };
            systemProcess.InstanceId = systemProcess.GenerateInstanceId();
            _systemProcessContext = new SystemProcessContext(_processRepository);
            _systemProcessContext.StartEvent(systemProcess);

            _distributedRuleScheduler.Initiate();
            _ruleScheduler.Initiate();
            _reddeerTradeService.Initialise();
        }

        public void Terminate()
        {
            _reddeerTradeService.Dispose();
            _ruleScheduler.Terminate();
            _distributedRuleScheduler.Terminate();
        }
    }
}
