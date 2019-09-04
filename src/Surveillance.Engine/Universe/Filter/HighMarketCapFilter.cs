namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;

    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class HighMarketCapFilter: IHighMarketCapFilter
    {
        private readonly IUniverseEquityInterDayCache _universeEquityInterdayCache;
        private readonly RuleRunMode _ruleRunMode;
        private readonly DecimalRangeRuleFilter _marketCapFilter;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly ISystemProcessOperationRunRuleContext _operationRunRuleContext;
        private readonly IUniverseDataRequestsSubscriber _universeDataRequestsSubscriber;
        private readonly ICurrencyConverterService currencyConverterService;

        private readonly ILogger _logger;
        private readonly string _name;

        private bool _requestData = false;

        public HighMarketCapFilter(
            IUniverseMarketCacheFactory factory,
            RuleRunMode ruleRunMode,
            DecimalRangeRuleFilter marketCap,
            IMarketTradingHoursService tradingHoursService,
            ISystemProcessOperationRunRuleContext operationRunRuleContext,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ICurrencyConverterService currencyConverterService,
            string ruleName,
            ILogger<HighMarketCapFilter> logger
        )
        {
            _universeEquityInterdayCache =
                factory?.BuildInterday(ruleRunMode)
                ?? throw new ArgumentNullException(nameof(factory));

            _ruleRunMode = ruleRunMode;
            _marketCapFilter = marketCap ?? DecimalRangeRuleFilter.None();

            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _operationRunRuleContext = operationRunRuleContext ?? throw new ArgumentNullException(nameof(operationRunRuleContext));
            _universeDataRequestsSubscriber = universeDataRequestsSubscriber;
            this.currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
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

            if (universeEvent.StateChange == UniverseStateEvent.Eschaton
                && _requestData
                && _ruleRunMode == RuleRunMode.ValidationRun)
            {
                _universeDataRequestsSubscriber.SubmitRequest();
            }

            if (universeEvent.StateChange != UniverseStateEvent.Order
                && universeEvent.StateChange != UniverseStateEvent.OrderPlaced)
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

            if (securityResult.HadMissingData && _ruleRunMode == RuleRunMode.ValidationRun)
            {
                _requestData = true;
            }

            if (securityResult.HadMissingData)
            {
                _logger.LogInformation($"Missing data for {marketDataRequest}.");
                return true;
            }

            var security = securityResult.Response;
            var marketCap = security.DailySummaryTimeBar.MarketCap;

            if (marketCap == null)
            {
                this._logger.LogInformation($"Missing data for market cap from daily summary time bar {marketDataRequest}.");
                return true;
            }

            var marketCapInUsd = this.currencyConverterService.Convert(
                new[] { marketCap.Value },
                new Currency("USD"), 
                universeDateTime,
                this._operationRunRuleContext).Result;

            if (marketCapInUsd == null)
            {
                this._logger.LogInformation($"Missing data for market cap currency conversion into USD at {universeEvent} from daily summary time bar {marketDataRequest}.");
                return true;
            }

            var min = this._marketCapFilter.Min ?? marketCap.Value.Value;
            var max = this._marketCapFilter.Max ?? marketCap.Value.Value;

            return !(marketCap.Value.Value >= min && marketCap.Value.Value <= max);

        }

        private void EquityInterDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityInterDayTimeBarCollection value))
            {
                return;
            }

            this._logger?.LogInformation($"Equity inter day event in HighMarketCapFilter occuring for {_name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this._universeEquityInterdayCache.Add(value);
        }
    }
}
