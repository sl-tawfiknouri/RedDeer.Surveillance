using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Utilities.Interfaces;
using Surveillance.Engine.DataCoordinator.Interfaces;
using Surveillance.Engine.RuleDistributor.Interfaces;
using Surveillance.Engine.Rules.Interfaces;
using Surveillance.Engine.Scheduler.Interfaces;
using Surveillance.Interfaces;

namespace Surveillance
{
    /// <summary>
    /// The mediator orchestrates program components; factory; services; display
    /// This represents the root entry into the 'real' surveillance object graph
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly Engine.Interfaces.Mediator.IMediator _ruleDistributorMediator;
        private readonly Engine.Interfaces.Mediator.IMediator _ruleAnalysisMediator;
        private readonly Engine.Interfaces.Mediator.IMediator _coordinatorMediator;
        private readonly Engine.Interfaces.Mediator.IMediator _schedulerMediator;
        private readonly IApplicationHeartbeatService _heartbeatService;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IRuleDistributorMediator ruleDistributorMediator,
            IRulesEngineMediator ruleEngineMediator,
            ICoordinatorMediator coordinatorMediator,
            IRuleSchedulerMediator schedulerMediator,
            IApplicationHeartbeatService heartbeatService,
            ILogger<Mediator> logger)
        {

            _ruleDistributorMediator = 
                ruleDistributorMediator
                ?? throw new ArgumentNullException(nameof(ruleDistributorMediator));

            _ruleAnalysisMediator =
                ruleEngineMediator
                ?? throw new ArgumentNullException(nameof(ruleEngineMediator));

            _coordinatorMediator =
                coordinatorMediator
                ?? throw new ArgumentNullException(nameof(coordinatorMediator));

            _schedulerMediator =
                schedulerMediator
                ?? throw new ArgumentNullException(nameof(schedulerMediator));

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
            _coordinatorMediator.Initiate();
            _schedulerMediator.Initiate();
            _heartbeatService.Initialise();
            _logger.LogInformation($"Mediator completed initiate");
        }

        public void Terminate()
        {
            _logger.LogInformation($"Mediator beginning terminate");
            _ruleDistributorMediator.Terminate();
            _ruleAnalysisMediator.Terminate();
            _coordinatorMediator.Terminate();
            _schedulerMediator.Terminate();
            _logger.LogInformation($"Mediator completed terminate");
        }
    }
}
