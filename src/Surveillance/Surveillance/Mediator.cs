using Surveillance.Services.Interfaces;
using System;
using Surveillance.Interfaces;
using Surveillance.Scheduler.Interfaces;

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
        private readonly IApplicationHeartbeatService _heartbeatService;
        private readonly IDeadLetterQueueService _deadLetterQueueService;

        public Mediator(
            IReddeerTradeService reddeerTradeService,
            IReddeerRuleScheduler ruleScheduler,
            IReddeerDistributedRuleScheduler distributedRuleScheduler,
            IApplicationHeartbeatService heartbeatService,
            IDeadLetterQueueService deadLetterQueueService)
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
            _heartbeatService =
                heartbeatService
                ?? throw new ArgumentNullException(nameof(heartbeatService));
            _deadLetterQueueService =
                deadLetterQueueService
                ?? throw new ArgumentNullException(nameof(deadLetterQueueService));
        }

        public void Initiate()
        {
            _distributedRuleScheduler.Initiate();
            _ruleScheduler.Initiate();
            _reddeerTradeService.Initialise();
            _heartbeatService.Initialise();
            _deadLetterQueueService.Initialise();
        }

        public void Terminate()
        {
            _reddeerTradeService.Dispose();
            _ruleScheduler.Terminate();
            _distributedRuleScheduler.Terminate();
            _deadLetterQueueService.Terminate();
            // we don't terminate the heart beat service as it will stop when the entire app has stopped
        }
    }
}
