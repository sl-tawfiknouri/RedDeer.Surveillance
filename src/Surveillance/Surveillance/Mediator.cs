using Surveillance.Services.Interfaces;
using System;
using System.Diagnostics;
using Surveillance.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Surveillance.System.Auditing.Context;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;

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
        private readonly IReddeerSmartRuleScheduler _smartRuleScheduler;
        private ISystemProcessContext _systemProcessContext;

        public Mediator(
            IReddeerTradeService reddeerTradeService,
            IReddeerRuleScheduler ruleScheduler,
            IReddeerSmartRuleScheduler smartRuleScheduler)
        {
            _reddeerTradeService =
                reddeerTradeService 
                ?? throw new ArgumentNullException(nameof(reddeerTradeService));
            _ruleScheduler =
                ruleScheduler
                ?? throw new ArgumentNullException(nameof(ruleScheduler));
            _smartRuleScheduler =
                smartRuleScheduler
                ?? throw new ArgumentNullException(nameof(smartRuleScheduler));
        }

        public void Initiate()
        {
            var systemProcess = new SystemProcess
            {
                MachineId = Environment.MachineName,
                ProcessId = Process.GetCurrentProcess()?.Id.ToString(),
                InstanceInitiated = DateTime.UtcNow,
                Heartbeat = DateTime.UtcNow
            };
            systemProcess.InstanceId = systemProcess.GenerateInstanceId();

            _systemProcessContext = new SystemProcessContext();
            _systemProcessContext.StartEvent(systemProcess);

            _smartRuleScheduler.Initiate();
            _ruleScheduler.Initiate();
            _reddeerTradeService.Initialise();
        }

        public void Terminate()
        {
            _reddeerTradeService.Dispose();
            _ruleScheduler.Terminate();
            _smartRuleScheduler.Terminate();
        }
    }
}
