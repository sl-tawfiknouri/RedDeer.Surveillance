using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.DataCoordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

namespace Surveillance.Engine.DataCoordinator
{
    public class Mediator : ICoordinatorMediator
    {
        private readonly IQueueSubscriber _queueSubscriber;
        private readonly ILogger<Mediator> _logger;

        public Mediator(
            IQueueSubscriber queueSubscriber,
            ILogger<Mediator> logger)
        {
            _queueSubscriber = queueSubscriber ?? throw new ArgumentNullException(nameof(queueSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger?.LogInformation("DataCoordinator Mediator Initiating");
            _queueSubscriber.Initiate();
            _logger?.LogInformation("DataCoordinator Mediator Initiation Completed");
        }

        public void Terminate()
        {
            _logger?.LogInformation("DataCoordinator Mediator Terminating");
            _queueSubscriber.Terminate();
            _logger?.LogInformation("DataCoordinator Mediator Termination Completed");
        }
    }
}
