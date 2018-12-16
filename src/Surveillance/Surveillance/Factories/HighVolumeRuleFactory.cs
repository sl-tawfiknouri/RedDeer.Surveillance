using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class HighVolumeRuleFactory : IHighVolumeRuleFactory
    {
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger<IHighVolumeRule> _logger;

        public HighVolumeRuleFactory(IUniverseMarketCacheFactory factory, ILogger<IHighVolumeRule> logger)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighVolumeRule Build(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream)
        {
            return new HighVolumeRule(parameters, opCtx, alertStream, _factory, _logger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
