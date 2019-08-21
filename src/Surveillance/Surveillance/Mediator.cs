namespace Surveillance
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Utilities.Interfaces;
    using Surveillance.Engine.DataCoordinator.Interfaces;
    using Surveillance.Engine.RuleDistributor.Interfaces;
    using Surveillance.Engine.Rules.Interfaces;
    using Surveillance.Engine.Scheduler.Interfaces;
    using Surveillance.Interfaces;

    /// <summary>
    ///     The mediator orchestrates program components; factory; services; display
    ///     This represents the root entry into the 'real' surveillance object graph
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly Engine.Interfaces.Mediator.IMediator _coordinatorMediator;

        private readonly IApplicationHeartbeatService _heartbeatService;

        private readonly ILogger<Mediator> _logger;

        private readonly Engine.Interfaces.Mediator.IMediator _ruleAnalysisMediator;

        private readonly Engine.Interfaces.Mediator.IMediator _ruleDistributorMediator;

        private readonly Engine.Interfaces.Mediator.IMediator _schedulerMediator;

        public Mediator(
            IRuleDistributorMediator ruleDistributorMediator,
            IRulesEngineMediator ruleEngineMediator,
            ICoordinatorMediator coordinatorMediator,
            IRuleSchedulerMediator schedulerMediator,
            IApplicationHeartbeatService heartbeatService,
            ILogger<Mediator> logger)
        {
            this._ruleDistributorMediator = ruleDistributorMediator
                                            ?? throw new ArgumentNullException(nameof(ruleDistributorMediator));

            this._ruleAnalysisMediator =
                ruleEngineMediator ?? throw new ArgumentNullException(nameof(ruleEngineMediator));

            this._coordinatorMediator =
                coordinatorMediator ?? throw new ArgumentNullException(nameof(coordinatorMediator));

            this._schedulerMediator = schedulerMediator ?? throw new ArgumentNullException(nameof(schedulerMediator));

            this._heartbeatService = heartbeatService ?? throw new ArgumentNullException(nameof(heartbeatService));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            this._logger.LogInformation("Mediator beginning initiate");
            this._ruleDistributorMediator.Initiate();
            this._ruleAnalysisMediator.Initiate();
            this._coordinatorMediator.Initiate();
            this._schedulerMediator.Initiate();
            this._heartbeatService.Initialise();
            this._logger.LogInformation("Mediator completed initiate");
        }

        public void Terminate()
        {
            this._logger.LogInformation("Mediator beginning terminate");
            this._ruleDistributorMediator.Terminate();
            this._ruleAnalysisMediator.Terminate();
            this._coordinatorMediator.Terminate();
            this._schedulerMediator.Terminate();
            this._logger.LogInformation("Mediator completed terminate");
        }
    }
}