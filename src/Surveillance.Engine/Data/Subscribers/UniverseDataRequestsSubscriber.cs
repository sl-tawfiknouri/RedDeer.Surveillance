using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Data.Subscribers
{
    public class UniverseDataRequestsSubscriber : IUniverseDataRequestsSubscriber
    {
        private readonly ISystemProcessOperationContext _operationContext;
        private readonly IQueueDataSynchroniserRequestPublisher _queueDataSynchroniserRequestPublisher;
        private readonly ILogger<UniverseDataRequestsSubscriber> _logger;

        public bool SubmitRequests { get; private set; }

        public UniverseDataRequestsSubscriber(
            ISystemProcessOperationContext operationContext,
            IQueueDataSynchroniserRequestPublisher queueDataSynchroniserRequestPublisher,
            ILogger<UniverseDataRequestsSubscriber> logger)
        {
            _operationContext = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            _queueDataSynchroniserRequestPublisher = queueDataSynchroniserRequestPublisher ?? throw new ArgumentNullException(nameof(queueDataSynchroniserRequestPublisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
            _logger?.LogInformation($"UniverseDataRequestsSubscriber reached OnCompleted() in its stream");
        }

        public void OnError(Exception error)
        {
            _logger?.LogError($"UniverseDataRequestsSubscriber reached OnError in its universe subscription {error.Message}", error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value.StateChange != UniverseStateEvent.Eschaton)
            {
                return;
            }

            _logger?.LogInformation($"UniverseDataRequestsSubscriber reached eschaton in its OnNext stream subscription and has a submit requests value of {SubmitRequests}");
            
            if (SubmitRequests)
            {
                var task = _queueDataSynchroniserRequestPublisher.Send(_operationContext.Id.ToString());
                task.Wait();
            }

            _logger?.LogInformation($"UniverseDataRequestsSubscriber completed eschaton in its OnNext stream subscription");
        }

        public void SubmitRequest()
        {
            _logger?.LogInformation($"UniverseDataRequestsSubscriber received a submit request indication for operation context {_operationContext.Id}.");

            SubmitRequests = true;
        }
    }
}
