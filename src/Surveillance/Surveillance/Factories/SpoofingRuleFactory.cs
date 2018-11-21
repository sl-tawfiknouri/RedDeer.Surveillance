using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.Spoofing;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class SpoofingRuleFactory : ISpoofingRuleFactory
    {
        private readonly ISpoofingRuleMessageSender _ruleMessageSender;
        private readonly ILogger<SpoofingRule> _logger;

        public SpoofingRuleFactory(
            ILogger<SpoofingRule> logger,
            ISpoofingRuleMessageSender ruleMessageSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ruleMessageSender = ruleMessageSender ?? throw new ArgumentNullException(nameof(ruleMessageSender));
        }

        public ISpoofingRule Build(ISpoofingRuleParameters spoofingParameters, ISystemProcessOperationRunRuleContext ruleCtx)
        {
            return new SpoofingRule(
                spoofingParameters,
                _ruleMessageSender,
                ruleCtx,
                _logger);
        }

        public static string Version => Versioner.Version(2, 0);
    }
}
