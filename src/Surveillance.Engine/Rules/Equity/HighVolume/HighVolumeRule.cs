namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
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
        private readonly IUniverseAlertStream _alertStream;

        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly IHighVolumeRuleEquitiesParameters _equitiesParameters;

        private readonly ILogger _logger;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private bool _hadMissingData;

        public HighVolumeRule(
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketTradingHoursService tradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<IHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows.BackwardWindowSize ?? TimeSpan.FromDays(1),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.HighVolume,
                EquityRuleHighVolumeFactory.Version,
                "High Volume Rule",
                opCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this._equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this._ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._dataRequestSubscriber =
                dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (HighVolumeRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (HighVolumeRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation("Eschaton occured");

            if (this._hadMissingData && this.RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(Rules.HighVolume, null, this._ruleCtx, false, true);
                this._alertStream.Add(alert);

                this._dataRequestSubscriber.SubmitRequest();
                this._ruleCtx.EndEvent();
            }
            else
            {
                this._ruleCtx?.EndEvent();
            }
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation("Genesis occurred");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred {exchange?.MarketClose}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null || !tradeWindow.Any()) return;

            var tradedSecurities =
                tradeWindow
                    .Where(tr =>
                        tr.OrderFilledVolume.GetValueOrDefault() > 0)
                    .ToList();

            var tradedVolume = tradedSecurities.Sum(tr => tr.OrderFilledVolume.GetValueOrDefault(0));

            var tradePosition = new TradePosition(tradedSecurities.ToList());
            var mostRecentTrade = tradeWindow.Pop();

            var dailyBreach = this.CheckDailyVolume(mostRecentTrade, tradedVolume);
            var windowBreach = this.CheckWindowVolume(mostRecentTrade, tradedVolume);
            var marketCapBreach = this.CheckMarketCap(mostRecentTrade, tradedSecurities);

            if (this.HasNoBreach(dailyBreach, windowBreach, marketCapBreach)) return;

            // wrong should use a judgement
            var breach = new HighVolumeRuleBreach(
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

            this._logger.LogInformation(
                $"RunRule had a breach for {mostRecentTrade?.Instrument?.Identifiers}. Daily Breach {dailyBreach?.HasBreach} | Window Breach {windowBreach?.HasBreach} | Market Cap Breach {marketCapBreach?.HasBreach}. Passing to alert stream.");
            var message = new UniverseAlertEvent(Rules.HighVolume, breach, this._ruleCtx);
            this._alertStream.Add(message);
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private HighVolumeRuleBreach.BreachDetails CheckDailyVolume(Order mostRecentTrade, decimal tradedVolume)
        {
            var dailyBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (this._equitiesParameters.HighVolumePercentageDaily.HasValue)
                dailyBreach = this.DailyVolumeCheck(mostRecentTrade, tradedVolume);

            return dailyBreach;
        }

        private HighVolumeRuleBreach.BreachDetails CheckMarketCap(Order mostRecentTrade, List<Order> tradedSecurities)
        {
            var marketCapBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (this._equitiesParameters.HighVolumePercentageMarketCap.HasValue)
                marketCapBreach = this.MarketCapCheck(mostRecentTrade, tradedSecurities);

            return marketCapBreach;
        }

        private HighVolumeRuleBreach.BreachDetails CheckWindowVolume(Order mostRecentTrade, decimal tradedVolume)
        {
            var windowBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (this._equitiesParameters.HighVolumePercentageWindow.HasValue)
                windowBreach = this.WindowVolumeCheck(mostRecentTrade, tradedVolume);

            return windowBreach;
        }

        private HighVolumeRuleBreach.BreachDetails DailyVolumeCheck(Order mostRecentTrade, decimal tradedVolume)
        {
            if (mostRecentTrade == null) return HighVolumeRuleBreach.BreachDetails.None();

            var tradingHours =
                this._tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                this._ruleCtx?.Id(),
                DataSource.AllInterday);

            var securityResult = this.UniverseEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                this._hadMissingData = true;
                this._logger.LogWarning($"Missing data for {marketDataRequest}.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            var threshold = (long)Math.Ceiling(
                this._equitiesParameters.HighVolumePercentageDaily.GetValueOrDefault(0)
                * security.DailySummaryTimeBar.DailyVolume.Traded);

            if (threshold <= 0)
            {
                this._hadMissingData = true;
                this._logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var breachPercentage = security.DailySummaryTimeBar.DailyVolume.Traded != 0 && tradedVolume != 0
                                       ? tradedVolume / security.DailySummaryTimeBar.DailyVolume.Traded
                                       : 0;

            if (tradedVolume >= threshold)
            {
                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, threshold, mostRecentTrade.Market);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
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

        private HighVolumeRuleBreach.BreachDetails MarketCapCheck(Order mostRecentTrade, List<Order> trades)
        {
            if (trades == null || !trades.Any()) return HighVolumeRuleBreach.BreachDetails.None();

            var tradingHours =
                this._tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
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
            var thresholdValue = (double)Math.Ceiling(
                this._equitiesParameters.HighVolumePercentageMarketCap.GetValueOrDefault(0)
                * security.DailySummaryTimeBar.MarketCap.GetValueOrDefault(0));

            if (thresholdValue <= 0)
            {
                this._hadMissingData = true;
                this._logger.LogInformation($"Market cap threshold of {thresholdValue} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradedValue = (double)trades.Select(
                    tr => tr.OrderFilledVolume.GetValueOrDefault(0)
                          * tr.OrderAverageFillPrice.GetValueOrDefault().Value)
                .Sum();

            var breachPercentage = security.DailySummaryTimeBar.MarketCap.GetValueOrDefault(0) != 0 && tradedValue != 0
                                       ? (decimal)tradedValue
                                         / security.DailySummaryTimeBar.MarketCap.GetValueOrDefault(0)
                                       : 0;

            if (tradedValue >= thresholdValue)
            {
                var thresholdMoney = new Money((decimal)thresholdValue, mostRecentTrade.OrderCurrency);
                var tradedMoney = new Money((decimal)tradedValue, mostRecentTrade.OrderCurrency);

                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, thresholdMoney, tradedMoney, mostRecentTrade.Market);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        private HighVolumeRuleBreach.BreachDetails WindowVolumeCheck(Order mostRecentTrade, decimal tradedVolume)
        {
            var tradingHours =
                this._tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");

            var tradingDates = this._tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                mostRecentTrade.Market?.MarketIdentifierCode);

            var marketRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                this._ruleCtx?.Id(),
                DataSource.AllIntraday);

            var marketResult = this.UniverseEquityIntradayCache.GetMarketsForRange(
                marketRequest,
                tradingDates,
                this.RunMode);

            if (marketResult.HadMissingData)
            {
                this._logger.LogTrace(
                    $"Unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {this.UniverseDateTime}.");

                this._hadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var securityDataTicks = marketResult.Response;
            var windowVolume = securityDataTicks.Sum(sdt => sdt.SpreadTimeBar.Volume.Traded);
            var threshold = (long)Math.Ceiling(
                this._equitiesParameters.HighVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

            var breachPercentage = windowVolume != 0 && tradedVolume != 0 ? tradedVolume / windowVolume : 0;

            if (threshold <= 0)
            {
                this._hadMissingData = true;
                this._logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            if (tradedVolume >= threshold)
                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, threshold, mostRecentTrade.Market);

            return HighVolumeRuleBreach.BreachDetails.None();
        }
    }
}