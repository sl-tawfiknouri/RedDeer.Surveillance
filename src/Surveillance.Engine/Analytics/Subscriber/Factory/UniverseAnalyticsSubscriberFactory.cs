using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Subscriber.Factory
{
    public class UniverseAnalyticsSubscriberFactory : IUniverseAnalyticsSubscriberFactory
    {
        private readonly ILogger<UniverseAnalyticsSubscriber> _logger;

        public UniverseAnalyticsSubscriberFactory(
            ILogger<UniverseAnalyticsSubscriber> logger)
        {
            _logger = logger;
        }

        public IUniverseAnalyticsSubscriber Build(int operationContextId)
        {
            return new UniverseAnalyticsSubscriber(operationContextId, _logger);
        }
    }
}
