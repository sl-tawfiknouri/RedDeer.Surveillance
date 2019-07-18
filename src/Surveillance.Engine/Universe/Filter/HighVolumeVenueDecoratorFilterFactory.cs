using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class HighVolumeVenueDecoratorFilterFactory : IHighVolumeVenueDecoratorFilterFactory
    {
        private readonly IUniverseEquityOrderFilterService _equityOrderFilterService;
        private readonly IUniverseMarketCacheFactory _universeMarketCacheFactory;
        private readonly IMarketTradingHoursService _marketTradingHoursService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;
        private readonly ILogger<HighVolumeVenueFilter> _venueLogger;

        public HighVolumeVenueDecoratorFilterFactory(
            IUniverseEquityOrderFilterService equityOrderFilterService,
            IUniverseMarketCacheFactory universeMarketCacheFactory,
            IMarketTradingHoursService marketTradingHoursService,
            ILogger<TradingHistoryStack> tradingHistoryLogger,
            ILogger<HighVolumeVenueFilter> venueLogger)
        {
            _equityOrderFilterService = equityOrderFilterService ?? throw new ArgumentNullException(nameof(equityOrderFilterService));
            _universeMarketCacheFactory = universeMarketCacheFactory ?? throw new ArgumentNullException(nameof(universeMarketCacheFactory));
            _marketTradingHoursService = marketTradingHoursService ?? throw new ArgumentNullException(nameof(marketTradingHoursService));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
            _venueLogger = venueLogger ?? throw new ArgumentNullException(nameof(venueLogger));
        }

        public IHighVolumeVenueDecoratorFilter Build(
            TimeWindows timeWindows,
            IUniverseFilterService baseService, 
            DecimalRangeRuleFilter venueVolumeFilterSetting,
            ISystemProcessOperationRunRuleContext ruleRunContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode ruleRunMode)
        {
            var filterRule =
                new HighVolumeVenueFilter(
                    timeWindows,
                    venueVolumeFilterSetting,
                    _equityOrderFilterService,
                    ruleRunContext,
                    _universeMarketCacheFactory,
                    ruleRunMode,
                    _marketTradingHoursService,
                    dataRequestSubscriber,
                    _tradingHistoryLogger,
                    _venueLogger);

            var filter =
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    baseService,
                    filterRule);

            return filter;
        }
    }
}
