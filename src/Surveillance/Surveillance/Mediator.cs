using System;
using Microsoft.Extensions.Logging;
using Surveillance.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Systems.Auditing.Utilities.Interfaces;

namespace Surveillance
{
    /// <summary>
    /// The mediator orchestrates program components; factory; services; display
    /// This represents the root entry into the 'real' surveillance object graph
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly IReddeerRuleScheduler _ruleScheduler;
        private readonly IReddeerDistributedRuleScheduler _distributedRuleScheduler;
        private readonly IApplicationHeartbeatService _heartbeatService;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IReddeerRuleScheduler ruleScheduler,
            IReddeerDistributedRuleScheduler distributedRuleScheduler,
            IApplicationHeartbeatService heartbeatService,
            ILogger<Mediator> logger)
        {
            _ruleScheduler =
                ruleScheduler
                ?? throw new ArgumentNullException(nameof(ruleScheduler));
            _distributedRuleScheduler =
                distributedRuleScheduler
                ?? throw new ArgumentNullException(nameof(distributedRuleScheduler));
            _heartbeatService =
                heartbeatService
                ?? throw new ArgumentNullException(nameof(heartbeatService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"Mediator beginning initiate");
            _distributedRuleScheduler.Initiate();
            _ruleScheduler.Initiate();
            _heartbeatService.Initialise();
            _logger.LogInformation($"Mediator completed initiate");
        }

        public void Terminate()
        {
            _logger.LogInformation($"Mediator beginning terminate");
            _ruleScheduler.Terminate();
            _distributedRuleScheduler.Terminate();
            _logger.LogInformation($"Mediator completed terminate");
            // we don't terminate the heart beat service as it will stop when the entire app has stopped
        }
    }
}
