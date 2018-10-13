using Surveillance.Rules;
using System;
using Surveillance.Rules.Layering;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;

namespace Surveillance.Factories
{
    public class LayeringRuleRuleFactory : ILayeringRuleFactory
    {
        private readonly ILogger<LayeringRuleRuleFactory> _logger;

        public LayeringRuleRuleFactory(ILogger<LayeringRuleRuleFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ILayeringRule Build(ILayeringRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx)
        {
            return new LayeringRule(parameters, _logger, ruleCtx);
        }

        public string RuleVersion => Versioner.Version(1, 0);
    }
}
