using Surveillance.Rules;
using System;
using Surveillance.Rules.Layering;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Universe.Filter.Interfaces;

namespace Surveillance.Factories
{
    public class LayeringRuleFactory : ILayeringRuleFactory
    {
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly ILogger<LayeringRuleFactory> _logger;

        public LayeringRuleFactory(
            IUniverseOrderFilter orderFilter,
            ILogger<LayeringRuleFactory> logger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ILayeringRule Build(
            ILayeringRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream)
        {
            return new LayeringRule(parameters, alertStream, _orderFilter, _logger, ruleCtx);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
