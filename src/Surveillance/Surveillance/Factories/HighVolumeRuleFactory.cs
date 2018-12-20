using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
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
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly ILogger<IHighVolumeRule> _logger;

        public HighVolumeRuleFactory(
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursManager tradingHoursManager,
            ILogger<IHighVolumeRule> logger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(IUniverseOrderFilter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighVolumeRule Build(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream)
        {
            return new HighVolumeRule(parameters, opCtx, alertStream, _orderFilter, _factory, _tradingHoursManager, _logger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
