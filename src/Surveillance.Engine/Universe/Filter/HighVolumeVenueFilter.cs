using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class HighVolumeVenueFilter : BaseUniverseRule
    {
        public readonly Dictionary<DateTime, HashSet<IUniverseEvent>> EventDictionary;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly ILogger<HighVolumeVenueFilter> _logger;

        public HighVolumeVenueFilter(
            TimeWindows timeWindows,
            IUniverseOrderFilter universeOrderFilter,
            ISystemProcessOperationRunRuleContext runRuleContext,
            IUniverseMarketCacheFactory universeMarketCacheFactory,
            RuleRunMode ruleRunMode,
            ILogger baseLogger,
            ILogger<TradingHistoryStack> stackLogger,
            ILogger<HighVolumeVenueFilter> logger) 
            : base(
                timeWindows.BackwardWindowSize,
                timeWindows.FutureWindowSize,
                Domain.Surveillance.Scheduling.Rules.UniverseFilter,
                Versioner.Version(1,0),
                nameof(HighVolumeVenueFilter),
                runRuleContext,
                universeMarketCacheFactory,
                ruleRunMode,
                baseLogger,
                stackLogger)
        {
            EventDictionary = new Dictionary<DateTime, HashSet<IUniverseEvent>>();
            _orderFilter = universeOrderFilter ?? throw new ArgumentNullException(nameof(universeOrderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            if (history == null)
            {
                _logger.LogInformation($"null history received by run post order event");
                return;
            }

            var activeHistory = history.ActiveTradeHistory();

            if (activeHistory == null
                || !activeHistory.Any())
            {
                return;
            }

            var volumeTraded = activeHistory.Sum(_ => _.OrderFilledVolume ?? 0);


        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation("Market Close occurred");

            // will call run post order event via proxy
            RunRuleForAllTradingHistoriesInMarket(exchange);
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occurred");
        }
    }
}
