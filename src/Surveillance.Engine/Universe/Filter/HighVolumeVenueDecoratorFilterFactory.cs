namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class HighVolumeVenueDecoratorFilterFactory : IHighVolumeVenueDecoratorFilterFactory
    {
        private readonly IUniverseEquityOrderFilterService _equityOrderFilterService;

        private readonly IMarketTradingHoursService _marketTradingHoursService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private readonly IUniverseMarketCacheFactory _universeMarketCacheFactory;

        private readonly ILogger<HighVolumeVenueFilter> _venueLogger;

        public HighVolumeVenueDecoratorFilterFactory(
            IUniverseEquityOrderFilterService equityOrderFilterService,
            IUniverseMarketCacheFactory universeMarketCacheFactory,
            IMarketTradingHoursService marketTradingHoursService,
            ILogger<TradingHistoryStack> tradingHistoryLogger,
            ILogger<HighVolumeVenueFilter> venueLogger)
        {
            this._equityOrderFilterService = equityOrderFilterService
                                             ?? throw new ArgumentNullException(nameof(equityOrderFilterService));
            this._universeMarketCacheFactory = universeMarketCacheFactory
                                               ?? throw new ArgumentNullException(nameof(universeMarketCacheFactory));
            this._marketTradingHoursService = marketTradingHoursService
                                              ?? throw new ArgumentNullException(nameof(marketTradingHoursService));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
            this._venueLogger = venueLogger ?? throw new ArgumentNullException(nameof(venueLogger));
        }

        public IHighVolumeVenueDecoratorFilter Build(
            TimeWindows timeWindows,
            IUniverseFilterService baseService,
            DecimalRangeRuleFilter venueVolumeFilterSetting,
            ISystemProcessOperationRunRuleContext ruleRunContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            DataSource dataSource,
            RuleRunMode ruleRunMode)
        {
            var filterRule = new HighVolumeVenueFilter(
                timeWindows,
                venueVolumeFilterSetting,
                this._equityOrderFilterService,
                ruleRunContext,
                this._universeMarketCacheFactory,
                ruleRunMode,
                this._marketTradingHoursService,
                dataRequestSubscriber,
                dataSource,
                this._tradingHistoryLogger,
                this._venueLogger);

            var filter = new HighVolumeVenueDecoratorFilter(timeWindows, baseService, filterRule);

            return filter;
        }
    }
}