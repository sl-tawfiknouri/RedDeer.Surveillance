namespace Surveillance.Engine.Rules.Analytics.Subscriber.Factory
{
    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;

    public class UniverseAnalyticsSubscriberFactory : IUniverseAnalyticsSubscriberFactory
    {
        private readonly ILogger<UniverseAnalyticsSubscriber> _logger;

        public UniverseAnalyticsSubscriberFactory(ILogger<UniverseAnalyticsSubscriber> logger)
        {
            this._logger = logger;
        }

        public IUniverseAnalyticsSubscriber Build(int operationContextId)
        {
            return new UniverseAnalyticsSubscriber(operationContextId, this._logger);
        }
    }
}