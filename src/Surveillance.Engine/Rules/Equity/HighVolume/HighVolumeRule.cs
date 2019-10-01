namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class HighVolumeRule : BaseUniverseRule, IHighVolumeRule
    {
        private readonly IHighVolumeRuleEquitiesParameters _equitiesParameters;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private readonly ICurrencyConverterService currencyConverterService;
        private readonly ILogger _logger;

        private bool _hadMissingData = false;

        public HighVolumeRule(
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext opContext,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketTradingHoursService tradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ICurrencyConverterService currencyConverterService,
            RuleRunMode runMode,
            ILogger<IHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger) 
            : base(
                equitiesParameters?.Windows.BackwardWindowSize ?? TimeSpan.FromDays(1),
                equitiesParameters?.Windows.BackwardWindowSize ?? TimeSpan.FromDays(1),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.HighVolume,
                EquityRuleHighVolumeFactory.Version,
                "High Volume Rule",
                opContext,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this._equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this._ruleCtx = opContext ?? throw new ArgumentNullException(nameof(opContext));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this.currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            var tradedSecurities =
                tradeWindow
                    .Where(tr =>
                        tr.OrderFilledVolume.GetValueOrDefault() > 0)
                    .ToList();

            var tradedVolume = tradedSecurities.Sum(tr => tr.OrderFilledVolume.GetValueOrDefault(0));

            var tradePosition = new TradePosition(tradedSecurities.ToList());
            var mostRecentTrade = tradeWindow.Peek();

            var dailyBreach = this.CheckDailyVolume(mostRecentTrade, tradedVolume);
            var windowBreach = this.CheckWindowVolume(mostRecentTrade, tradedVolume);
            var marketCapBreach = this.CheckMarketCap(mostRecentTrade, tradedSecurities);

            if (this.HasNoBreach(dailyBreach, windowBreach, marketCapBreach))
            {
                return;
            }

            // wrong should use a judgement
            var breach =
                new HighVolumeRuleBreach(
                    this.OrganisationFactorValue,
                    this._ruleCtx.SystemProcessOperationContext(),
                    this._ruleCtx.CorrelationId(),
                    this._equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(1),
                    tradePosition,
                    mostRecentTrade?.Instrument,
                    this._equitiesParameters,
                    dailyBreach,
                    windowBreach,
                    marketCapBreach,
                    tradedVolume,
                    null,
                    null,
                    this.UniverseDateTime);

            this._logger.LogInformation($"RunRule had a breach for {mostRecentTrade?.Instrument?.Identifiers}. Daily Breach {dailyBreach?.HasBreach} | Window Breach {windowBreach?.HasBreach} | Market Cap Breach {marketCapBreach?.HasBreach}. Passing to alert stream.");
            var message = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.HighVolume, breach, this._ruleCtx);
            this._alertStream.Add(message);
        }

        private HighVolumeRuleBreach.BreachDetails CheckDailyVolume(Order mostRecentTrade, decimal tradedVolume)
        {
            var dailyBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_equitiesParameters.HighVolumePercentageDaily.HasValue)
            {
                dailyBreach = DailyVolumeCheck(mostRecentTrade, tradedVolume);
            }

            return dailyBreach;
        }

        private HighVolumeRuleBreach.BreachDetails CheckWindowVolume(Order mostRecentTrade, decimal tradedVolume)
        {
            var windowBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (_equitiesParameters.HighVolumePercentageWindow.HasValue)
            {
                windowBreach = WindowVolumeCheck(mostRecentTrade, tradedVolume);
            }

            return windowBreach;
        }

        private HighVolumeRuleBreach.BreachDetails CheckMarketCap(Order mostRecentTrade, List<Order> tradedSecurities)
        {
            var marketCapBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (this._equitiesParameters.HighVolumePercentageMarketCap.HasValue)
            {
                marketCapBreach = this.MarketCapCheck(mostRecentTrade, tradedSecurities);
            }

            return marketCapBreach;
        }

        private bool HasNoBreach(
            HighVolumeRuleBreach.BreachDetails dailyBreach,
            HighVolumeRuleBreach.BreachDetails windowBreach,
            HighVolumeRuleBreach.BreachDetails marketCapBreach)
        {
            return (!dailyBreach?.HasBreach ?? true)
                   && (!windowBreach?.HasBreach ?? true)
                   && (!marketCapBreach?.HasBreach ?? true);
        }

        private HighVolumeRuleBreach.BreachDetails DailyVolumeCheck(Order mostRecentTrade, decimal tradedVolume)
        {
            if (mostRecentTrade == null)
            {
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradingHours = _tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                _ruleCtx?.Id(),
                DataSource.AllInterday);

            var securityResult = UniverseEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogWarning($"Missing data for {marketDataRequest}.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            var threshold = (long)Math.Ceiling(
                _equitiesParameters.HighVolumePercentageDaily.GetValueOrDefault(0) * security.DailySummaryTimeBar.DailyVolume.Traded);

            if (threshold <= 0)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var breachPercentage =
                security.DailySummaryTimeBar.DailyVolume.Traded != 0 && tradedVolume != 0
                    ? (decimal)tradedVolume / (decimal)security.DailySummaryTimeBar.DailyVolume.Traded
                    : 0;

            if (tradedVolume >= threshold)
            {
                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, threshold, mostRecentTrade.Market);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        private HighVolumeRuleBreach.BreachDetails WindowVolumeCheck(Order mostRecentTrade, decimal tradedVolume)
        {
            var tradingHours = _tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var tradingDates = _tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                mostRecentTrade.Market?.MarketIdentifierCode);

            var marketRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market?.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                    tradingHours.ClosingInUtcForDay(UniverseDateTime),
                    _ruleCtx?.Id(),
                    DataSource.AllIntraday);

            var marketResult = UniverseEquityIntradayCache.GetMarketsForRange(marketRequest, tradingDates, RunMode);

            if (marketResult.HadMissingData)
            {
                _logger.LogTrace($"Unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var securityDataTicks = marketResult.Response;           
            var windowVolume = securityDataTicks.Sum(sdt => sdt.SpreadTimeBar.Volume.Traded);
            var threshold = (long)Math.Ceiling(_equitiesParameters.HighVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

            var breachPercentage =
                windowVolume != 0 && tradedVolume != 0
                    ? (decimal)tradedVolume / (decimal)windowVolume
                    : 0;

            if (threshold <= 0)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Window volume threshold of {threshold} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            if (tradedVolume >= threshold)
            {
                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, threshold, mostRecentTrade.Market);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        private HighVolumeRuleBreach.BreachDetails MarketCapCheck(Order mostRecentTrade, List<Order> trades)
        {
            if (trades == null
                || !trades.Any())
            {
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradingHours = this._tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this._logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(this.UniverseDateTime),
                this._ruleCtx?.Id(),
                DataSource.AllInterday);

            var securityResult = this.UniverseEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                this._hadMissingData = true;
                this._logger.LogInformation($"Missing data for {marketDataRequest}.");

                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            var marketCapMoney = security.DailySummaryTimeBar.MarketCap;

            if (marketCapMoney == null)
            {
                this._logger.LogInformation($"Missing data for {marketDataRequest}.");

                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var convertedMarketCap =
                this.currencyConverterService.Convert(
                    new[] { marketCapMoney.Value },
                    mostRecentTrade.OrderCurrency,
                    this.UniverseDateTime,
                    this._ruleCtx).Result;

            if (convertedMarketCap == null)
            {
                this._hadMissingData = true;
                this._logger.LogInformation($"Missing data for exchange rates between USD and {mostRecentTrade.OrderCurrency} on {this.UniverseDateTime}");

                return HighVolumeRuleBreach.BreachDetails.None();
            }

            double thresholdValue =
                (double)Math.Ceiling(this._equitiesParameters.HighVolumePercentageMarketCap.GetValueOrDefault(0)
                * convertedMarketCap.Value.Value);

            if (thresholdValue <= 0)
            {
                this._hadMissingData = true;
                this._logger.LogInformation($"Market cap threshold of {thresholdValue} was recorded.");

                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradedValue =
                (double)trades
                    .Select(tr => tr.OrderFilledVolume.GetValueOrDefault(0) * (tr.OrderAverageFillPrice.GetValueOrDefault().Value))
                    .Sum();

            var breachPercentage =
                convertedMarketCap.Value.Value != 0 && tradedValue != 0
                    ? (decimal)tradedValue / convertedMarketCap.Value.Value
                    : 0;

            if (tradedValue >= thresholdValue)
            {
                var thresholdMoney = new Money((decimal)thresholdValue, mostRecentTrade.OrderCurrency);
                var tradedMoney = new Money((decimal)tradedValue, mostRecentTrade.OrderCurrency);

                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, thresholdMoney, tradedMoney, mostRecentTrade.Market);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        { }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        { }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
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

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured");

            if (_hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.HighVolume, null, _ruleCtx, false, true);
                _alertStream.Add(alert);

                _dataRequestSubscriber.SubmitRequest();
                _ruleCtx.EndEvent();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (HighVolumeRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (HighVolumeRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
