using Surveillance.Rules;
using System;
using Surveillance.Rules.Layering;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;

namespace Surveillance.Factories
{
    public class LayeringRuleFactory : ILayeringRuleFactory
    {
        private readonly ILogger<LayeringRuleFactory> _logger;

        public LayeringRuleFactory(ILogger<LayeringRuleFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ILayeringRule Build(
            ILayeringRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream)
        {
            return new LayeringRule(parameters, alertStream, _logger, ruleCtx);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
