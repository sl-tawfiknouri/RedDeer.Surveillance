using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.RuleDistributor.Interfaces;
using Surveillance.Engine.Rules.Interfaces;
using Surveillance.Interfaces;
using Surveillance.Systems.Auditing.Utilities.Interfaces;

namespace Surveillance
{
    /// <summary>
    /// The mediator orchestrates program components; factory; services; display
    /// This represents the root entry into the 'real' surveillance object graph
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly Engines.Interfaces.Mediator.IMediator _ruleDistributorMediator;
        private readonly Engines.Interfaces.Mediator.IMediator _ruleAnalysisMediator;
        private readonly IApplicationHeartbeatService _heartbeatService;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IRuleDistributorMediator ruleDistributorMediator,
            IRulesEngineMediator ruleEngineMediator,
            IApplicationHeartbeatService heartbeatService,
            ILogger<Mediator> logger)
        {

            _ruleDistributorMediator = 
                ruleDistributorMediator
                ?? throw new ArgumentNullException(nameof(ruleDistributorMediator));

            _ruleAnalysisMediator =
                ruleEngineMediator
                ?? throw new ArgumentNullException(nameof(ruleEngineMediator));

            _heartbeatService =
                heartbeatService
                ?? throw new ArgumentNullException(nameof(heartbeatService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"Mediator beginning initiate");
            _ruleDistributorMediator.Initiate();
            _ruleAnalysisMediator.Initiate();
            _heartbeatService.Initialise();
            _logger.LogInformation($"Mediator completed initiate");
        }

        public void Terminate()
        {
            _logger.LogInformation($"Mediator beginning terminate");
            _ruleDistributorMediator.Terminate();
            _ruleAnalysisMediator.Terminate();
            _logger.LogInformation($"Mediator completed terminate");
        }
    }
}
