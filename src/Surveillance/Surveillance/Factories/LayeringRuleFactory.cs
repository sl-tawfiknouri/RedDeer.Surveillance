using Surveillance.Rules;
using System;
using Surveillance.Rules.Layering;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters.Interfaces;

namespace Surveillance.Factories
{
    public class LayeringRuleFactory : ILayeringRuleFactory
    {
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger<LayeringRuleFactory> _logger;

        public LayeringRuleFactory(
            IMarketTradingHoursManager tradingHoursManager,
            IUniverseMarketCacheFactory factory,
            ILogger<LayeringRuleFactory> logger)
        {
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ILayeringRule Build(
            ILayeringRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream)
        {
            return new LayeringRule(parameters, alertStream, _logger, _factory, _tradingHoursManager, ruleCtx);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
