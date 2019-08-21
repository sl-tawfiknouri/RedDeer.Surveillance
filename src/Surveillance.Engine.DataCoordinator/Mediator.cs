namespace Surveillance.Engine.DataCoordinator
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.DataCoordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Queues.Interfaces;
    using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

    public class Mediator : ICoordinatorMediator
    {
        private readonly IDataCoordinatorScheduler _dataScheduler;

        private readonly ILogger<Mediator> _logger;

        private readonly IQueueSubscriber _queueSubscriber;

        public Mediator(
            IDataCoordinatorScheduler dataScheduler,
            IQueueSubscriber queueSubscriber,
            ILogger<Mediator> logger)
        {
            this._dataScheduler = dataScheduler ?? throw new ArgumentNullException(nameof(dataScheduler));
            this._queueSubscriber = queueSubscriber ?? throw new ArgumentNullException(nameof(queueSubscriber));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            this._logger?.LogInformation("Initiating");
            this._dataScheduler.Initialise();
            this._queueSubscriber.Initiate();
            this._logger?.LogInformation("Initiation Completed");
        }

        public void Terminate()
        {
            this._logger?.LogInformation("Terminating");
            this._dataScheduler.Terminate();
            this._queueSubscriber.Terminate();
            this._logger?.LogInformation("Termination Completed");
        }
    }
}