using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.DataCoordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;
using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

namespace Surveillance.Engine.DataCoordinator
{
    public class Mediator : ICoordinatorMediator
    {
        private readonly IDataCoordinatorScheduler _dataScheduler;
        private readonly IQueueSubscriber _queueSubscriber;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IDataCoordinatorScheduler dataScheduler,
            IQueueSubscriber queueSubscriber,
            ILogger<Mediator> logger)
        {
            _dataScheduler = dataScheduler ?? throw new ArgumentNullException(nameof(dataScheduler));
            _queueSubscriber = queueSubscriber ?? throw new ArgumentNullException(nameof(queueSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger?.LogInformation("DataCoordinator Mediator Initiating");
            _dataScheduler.Initialise();
            _queueSubscriber.Initiate();
            _logger?.LogInformation("DataCoordinator Mediator Initiation Completed");
        }

        public void Terminate()
        {
            _logger?.LogInformation("DataCoordinator Mediator Terminating");
            _dataScheduler.Terminate();
            _queueSubscriber.Terminate();
            _logger?.LogInformation("DataCoordinator Mediator Termination Completed");
        }
    }
}
