using System;
using Microsoft.Extensions.Logging;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Data.Subscribers
{
    public class UniverseDataRequestsSubscriber : IUniverseDataRequestsSubscriber
    {
        private readonly ISystemProcessOperationContext _operationContext;
        private readonly IDataRequestMessageSender _dataRequestMessageSender;
        private readonly ILogger<UniverseDataRequestsSubscriber> _logger;

        private bool _submitRequests = false;

        public UniverseDataRequestsSubscriber(
            ISystemProcessOperationContext operationContext,
            IDataRequestMessageSender dataRequestMessageSender,
            ILogger<UniverseDataRequestsSubscriber> logger)
        {
            _operationContext = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            _dataRequestMessageSender = dataRequestMessageSender ?? throw new ArgumentNullException(nameof(dataRequestMessageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
            _logger?.LogInformation($"UniverseDataRequestsSubscriber reached OnCompleted() in its stream");
        }

        public void OnError(Exception error)
        {
            _logger?.LogError($"UniverseDataRequestsSubscriber reached OnError in its universe subscription", error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value.StateChange != UniverseStateEvent.Eschaton)
            {
                return;
            }

            _logger?.LogInformation($"UniverseDataRequestsSubscriber reached eschaton in its OnNext stream subscription and has a submit requests value of {_submitRequests}");
            
            if (_submitRequests)
            {
                var task = _dataRequestMessageSender.Send(_operationContext.Id.ToString());
                task.Wait();
            }

            _logger?.LogInformation($"UniverseDataRequestsSubscriber completed eschaton in its OnNext stream subscription");
        }

        public void SubmitRequest()
        {
            _logger?.LogInformation($"UniverseDataRequestsSubscriber received a submit request indication for operation context {_operationContext.Id}.");

            _submitRequests = true;
        }
    }
}
