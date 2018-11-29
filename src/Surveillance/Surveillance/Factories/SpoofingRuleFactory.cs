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
        private readonly ILogger<SpoofingRule> _logger;

        public SpoofingRuleFactory(ILogger<SpoofingRule> logger)
        {
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
                _logger);
        }

        public static string Version => Versioner.Version(2, 0);
    }
}
