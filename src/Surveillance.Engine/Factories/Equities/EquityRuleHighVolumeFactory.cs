using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities
{
    public class EquityRuleHighVolumeFactory : IEquityRuleHighVolumeFactory
    {
        private readonly IUniverseEquityOrderFilterService _orderFilterService;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly ILogger<IHighVolumeRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleHighVolumeFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursService tradingHoursService,
            ILogger<IHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public IHighVolumeRule Build(
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode)
        {

            return new HighVolumeRule(equitiesParameters, opCtx, alertStream, _orderFilterService, _factory, _tradingHoursService, dataRequestSubscriber, runMode, _logger, _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
