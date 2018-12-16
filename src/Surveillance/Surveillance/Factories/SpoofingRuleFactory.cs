using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.Spoofing;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class SpoofingRuleFactory : ISpoofingRuleFactory
    {
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger<SpoofingRule> _logger;

        public SpoofingRuleFactory(IUniverseMarketCacheFactory factory, ILogger<SpoofingRule> logger)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ISpoofingRule Build(
            ISpoofingRuleParameters spoofingParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream)
        {
            return new SpoofingRule(
                spoofingParameters,
                ruleCtx,
                alertStream,
                _factory,
                _logger);
        }

        public static string Version => Versioner.Version(2, 0);
    }
}
