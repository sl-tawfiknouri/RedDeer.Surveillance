namespace Surveillance.Engine.Rules.Data.Subscribers
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;

    public class UniverseDataRequestsSubscriberFactory : IUniverseDataRequestsSubscriberFactory
    {
        private readonly ILogger<UniverseDataRequestsSubscriber> _logger;

        private readonly IQueueDataSynchroniserRequestPublisher _publisher;

        public UniverseDataRequestsSubscriberFactory(
            IQueueDataSynchroniserRequestPublisher publisher,
            ILogger<UniverseDataRequestsSubscriber> logger)
        {
            this._publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseDataRequestsSubscriber Build(ISystemProcessOperationContext context)
        {
            return new UniverseDataRequestsSubscriber(context, this._publisher, this._logger);
        }
    }
}