using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using System;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class HighMarketCapFilter: IHighMarketCapFilter
    {
        protected readonly IUniverseEquityInterDayCache _universeEquityInterdayCache;
        protected readonly DecimalRangeRuleFilter _marketCapFilter;
        protected readonly IMarketTradingHoursService _tradingHoursService;
        protected readonly ISystemProcessOperationRunRuleContext _operationRunRuleContext;
        protected readonly ILogger _logger;
        protected readonly string _name;

        public HighMarketCapFilter(
            IUniverseMarketCacheFactory factory,
            RuleRunMode ruleRunMode,
            DecimalRangeRuleFilter marketCap,
            IMarketTradingHoursService tradingHoursService,
            ISystemProcessOperationRunRuleContext operationRunRuleContext,
            string ruleName,
            ILogger<HighMarketCapFilter> logger
        )
        {
            _universeEquityInterdayCache =
                factory?.BuildInterday(ruleRunMode)
                ?? throw new ArgumentNullException(nameof(factory));

            _marketCapFilter = marketCap ?? DecimalRangeRuleFilter.None();

            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _operationRunRuleContext = operationRunRuleContext ?? throw new ArgumentNullException(nameof(operationRunRuleContext));
            _name = ruleName;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Filter(IUniverseEvent universeEvent)
        {
            if (universeEvent == null)
            {
                return false;
            }

            if (universeEvent.StateChange == UniverseStateEvent.EquityInterDayTick)
            {
                EquityInterDay(universeEvent);
            }

            if (universeEvent.StateChange != UniverseStateEvent.Order && universeEvent.StateChange != UniverseStateEvent.OrderPlaced)
            {
                return false;
            }

            if (!(universeEvent.UnderlyingEvent is Order mostRecentTrade))
            {
                return false;
            }

            if (_marketCapFilter?.Type != RuleFilterType.Include)
            {
                return false;
            }

            var tradingHours = _tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
                return true;
            }

            var universeDateTime = universeEvent.EventTime;
            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(universeDateTime),
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(universeDateTime),
                _operationRunRuleContext?.Id(),
                DataSource.AllInterday);

            var securityResult = _universeEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                _logger.LogInformation($"Missing data for {marketDataRequest}.");
                return true;
            }

            var security = securityResult.Response;
            var marketCap = security.DailySummaryTimeBar.MarketCap.GetValueOrDefault(0);

            var min = _marketCapFilter.Min ?? marketCap;
            var max = _marketCapFilter.Max ?? marketCap;

            return !(marketCap >= min && marketCap <= max);

        }

        private void EquityInterDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityInterDayTimeBarCollection value))
            {
                return;
            }

            _logger?.LogInformation($"Equity inter day event in HighMarketCapFilter occuring for {_name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            // TODO: Check if everything will be added when I
            _universeEquityInterdayCache.Add(value);
        }
    }
}
