using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.MessageBusIO.Interfaces;

namespace Surveillance.Engine.Rules.Data.Subscribers
{
    public class UniverseDataRequestsSubscriberFactory : IUniverseDataRequestsSubscriberFactory
    {
        private readonly IDataRequestMessageSender _messageSender;
        private readonly ILogger<UniverseDataRequestsSubscriber> _logger;

        public UniverseDataRequestsSubscriberFactory(
            IDataRequestMessageSender messageSender,
            ILogger<UniverseDataRequestsSubscriber> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseDataRequestsSubscriber Build(ISystemProcessOperationContext context)
        {
            return new UniverseDataRequestsSubscriber(context, _messageSender, _logger);
        }
    }
}
