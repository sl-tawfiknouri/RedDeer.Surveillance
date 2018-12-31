﻿using System;
using Surveillance.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Services.Interfaces;

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

        public Mediator(
            IReddeerRuleScheduler ruleScheduler,
            IReddeerDistributedRuleScheduler distributedRuleScheduler,
            IApplicationHeartbeatService heartbeatService)
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
        }

        public void Initiate()
        {
            _distributedRuleScheduler.Initiate();
            _ruleScheduler.Initiate();
            _heartbeatService.Initialise();
        }

        public void Terminate()
        {
            _ruleScheduler.Terminate();
            _distributedRuleScheduler.Terminate();
            // we don't terminate the heart beat service as it will stop when the entire app has stopped
        }
    }
}
