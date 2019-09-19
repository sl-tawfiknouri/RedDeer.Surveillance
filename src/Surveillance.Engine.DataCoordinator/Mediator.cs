namespace Surveillance.Engine.DataCoordinator
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.DataCoordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Queues.Interfaces;
    using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

    /// <summary>
    /// The mediator.
    /// </summary>
    public class Mediator : ICoordinatorMediator
    {
        /// <summary>
        /// The data scheduler.
        /// </summary>
        private readonly IDataCoordinatorScheduler dataScheduler;

        /// <summary>
        /// The queue subscriber.
        /// </summary>
        private readonly IQueueSubscriber queueSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<Mediator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="dataScheduler">
        /// The data scheduler.
        /// </param>
        /// <param name="queueSubscriber">
        /// The queue subscriber.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public Mediator(
            IDataCoordinatorScheduler dataScheduler,
            IQueueSubscriber queueSubscriber,
            ILogger<Mediator> logger)
        {
            this.dataScheduler = dataScheduler ?? throw new ArgumentNullException(nameof(dataScheduler));
            this.queueSubscriber = queueSubscriber ?? throw new ArgumentNullException(nameof(queueSubscriber));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The initiate.
        /// </summary>
        public void Initiate()
        {
            this.logger?.LogInformation("Initiating");
            this.dataScheduler.Initialise();
            this.queueSubscriber.Initiate();
            this.logger?.LogInformation("Initiation Completed");
        }

        /// <summary>
        /// The terminate.
        /// </summary>
        public void Terminate()
        {
            this.logger?.LogInformation("Terminating");
            this.dataScheduler.Terminate();
            this.queueSubscriber.Terminate();
            this.logger?.LogInformation("Termination Completed");
        }
    }
}