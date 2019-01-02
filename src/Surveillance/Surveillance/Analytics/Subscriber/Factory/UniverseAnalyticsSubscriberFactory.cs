using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Analytics.Subscriber.Interfaces;

namespace Surveillance.Analytics.Subscriber.Factory
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
