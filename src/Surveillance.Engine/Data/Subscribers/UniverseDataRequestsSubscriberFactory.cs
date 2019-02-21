using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules.Data.Subscribers
{
    public class UniverseDataRequestsSubscriberFactory : IUniverseDataRequestsSubscriberFactory
    {
        private readonly IQueueDataSynchroniserRequestPublisher _publisher;
        private readonly ILogger<UniverseDataRequestsSubscriber> _logger;

        public UniverseDataRequestsSubscriberFactory(
            IQueueDataSynchroniserRequestPublisher publisher,
            ILogger<UniverseDataRequestsSubscriber> logger)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseDataRequestsSubscriber Build(ISystemProcessOperationContext context)
        {
            return new UniverseDataRequestsSubscriber(context, _publisher, _logger);
        }
    }
}
