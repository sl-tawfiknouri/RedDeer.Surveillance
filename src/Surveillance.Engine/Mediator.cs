﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules
{
    public class Mediator : IRulesEngineMediator
    {
        private readonly IQueueRuleSubscriber _ruleSubscriber;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IQueueRuleSubscriber ruleSubscriber,
            ILogger<Mediator> logger)
        {
            _ruleSubscriber = ruleSubscriber ?? throw new ArgumentNullException(nameof(ruleSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger?.LogInformation("initiating");
            _ruleSubscriber.Initiate();
            _logger?.LogInformation("initiation complete");
        }

        public void Terminate()
        {
            _logger?.LogInformation("terminating");
            _ruleSubscriber.Terminate();
            _logger?.LogInformation("termination complete");
        }
    }
}