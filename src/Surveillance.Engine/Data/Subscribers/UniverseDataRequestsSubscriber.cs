namespace Surveillance.Engine.Rules.Data.Subscribers
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;

    public class UniverseDataRequestsSubscriber : IUniverseDataRequestsSubscriber
    {
        private readonly ILogger<UniverseDataRequestsSubscriber> _logger;

        private readonly ISystemProcessOperationContext _operationContext;

        private readonly IQueueDataSynchroniserRequestPublisher _queueDataSynchroniserRequestPublisher;

        public UniverseDataRequestsSubscriber(
            ISystemProcessOperationContext operationContext,
            IQueueDataSynchroniserRequestPublisher queueDataSynchroniserRequestPublisher,
            ILogger<UniverseDataRequestsSubscriber> logger)
        {
            this._operationContext = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            this._queueDataSynchroniserRequestPublisher = queueDataSynchroniserRequestPublisher
                                                          ?? throw new ArgumentNullException(
                                                              nameof(queueDataSynchroniserRequestPublisher));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool SubmitRequests { get; private set; }

        public void DispatchIfSubmitRequest()
        {
            this._logger?.LogInformation(
                $"reached eschaton in its OnNext stream subscription and has a submit requests value of {this.SubmitRequests}");

            if (this.SubmitRequests)
            {
                var task = this._queueDataSynchroniserRequestPublisher.Send(this._operationContext.Id.ToString());
                task.Wait();
            }

            this._logger?.LogInformation("completed eschaton in its OnNext stream subscription");
        }

        public void OnCompleted()
        {
            this._logger?.LogInformation("reached OnCompleted() in its stream");
        }

        public void OnError(Exception error)
        {
            this._logger?.LogError(error, $"reached OnError in its universe subscription {error.Message}");
        }

        public void OnNext(IUniverseEvent value)
        {
        }

        /// <summary>
        ///     Ensure that this can only be set to true and not unset
        /// </summary>
        public void SubmitRequest()
        {
            this._logger?.LogInformation(
                $"received a submit request indication for operation context {this._operationContext.Id}.");

            this.SubmitRequests = true;
        }
    }
}