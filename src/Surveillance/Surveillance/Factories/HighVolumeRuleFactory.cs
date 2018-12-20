using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Filter.Interfaces;

namespace Surveillance.Factories
{
    public class HighVolumeRuleFactory : IHighVolumeRuleFactory
    {
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly ILogger<IHighVolumeRule> _logger;

        public HighVolumeRuleFactory(
            IUniverseOrderFilter orderFilter,
            ILogger<IHighVolumeRule> logger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(IUniverseOrderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighVolumeRule Build(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream)
        {
            return new HighVolumeRule(parameters, opCtx, alertStream, _orderFilter, _logger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
