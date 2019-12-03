namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
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

    /// <summary>
    /// The high volume rule.
    /// </summary>
    public class HighVolumeRule : BaseUniverseRule, IHighVolumeRule
    {
        /// <summary>
        /// The equities parameters.
        /// </summary>
        private readonly IHighVolumeRuleEquitiesParameters EquitiesParameters;

        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream AlertStream;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter OrderFilter;

        /// <summary>
        /// The trading hours service.
        /// </summary>
        private readonly IMarketTradingHoursService TradingHoursService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber DataRequestSubscriber;

        /// <summary>
        /// The currency converter service.
        /// </summary>
        private readonly ICurrencyConverterService CurrencyConverterService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger Logger;

        /// <summary>
        /// The had missing data.
        /// </summary>
        private bool HadMissingData = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighVolumeRule"/> class.
        /// </summary>
        /// <param name="equitiesParameters">
        /// The equities parameters.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="equityMarketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="fixedIncomeMarketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="tradingHoursService">
        /// The trading hours service.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="currencyConverterService">
        /// The currency converter service.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// The trading history logger.
        /// </param>
        public HighVolumeRule(
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
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
                operationContext,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.EquitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this.AlertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.OrderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this.TradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this.DataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this.CurrencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organisation factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public override IRuleDataConstraint DataConstraints()
        {
            if (this.EquitiesParameters == null)
            {
                return RuleDataConstraint.Empty().Case;
            }

            var constraints = new List<RuleDataSubConstraint>();

            if (this.EquitiesParameters.HighVolumePercentageDaily != null)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyInterday,
                    _ => true);

                constraints.Add(constraint);
            }

            if (this.EquitiesParameters.HighVolumePercentageMarketCap != null)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyInterday,
                    _ => !this.OrderFilter.Filter(_));

                constraints.Add(constraint);
            }

            if (this.EquitiesParameters.HighVolumePercentageWindow != null)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyIntraday,
                    _ => !this.OrderFilter.Filter(_));

                constraints.Add(constraint);
            }

            return new RuleDataConstraint(
                this.Rule,
                this.EquitiesParameters.Id,
                constraints);
        }

        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.OrderFilter.Filter(value);
        }

        /// <summary>
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
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
                    this.RuleCtx.SystemProcessOperationContext(),
                    this.RuleCtx.CorrelationId(),
                    this.EquitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(1),
                    tradePosition,
                    mostRecentTrade?.Instrument,
                    this.EquitiesParameters,
                    dailyBreach,
                    windowBreach,
                    marketCapBreach,
                    tradedVolume,
                    null,
                    null,
                    this.UniverseDateTime);

            this.Logger.LogInformation($"RunRule had a breach for {mostRecentTrade?.Instrument?.Identifiers}. Daily Breach {dailyBreach?.HasBreach} | Window Breach {windowBreach?.HasBreach} | Market Cap Breach {marketCapBreach?.HasBreach}. Passing to alert stream.");
            var message = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.HighVolume, breach, this.RuleCtx);
            this.AlertStream.Add(message);
        }

        /// <summary>
        /// The check daily volume.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="BreachDetails"/>.
        /// </returns>
        private HighVolumeRuleBreach.BreachDetails CheckDailyVolume(Order mostRecentTrade, decimal tradedVolume)
        {
            var dailyBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (this.EquitiesParameters.HighVolumePercentageDaily.HasValue)
            {
                dailyBreach = this.DailyVolumeCheck(mostRecentTrade, tradedVolume);
            }

            return dailyBreach;
        }

        /// <summary>
        /// The check window volume.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="BreachDetails"/>.
        /// </returns>
        private HighVolumeRuleBreach.BreachDetails CheckWindowVolume(Order mostRecentTrade, decimal tradedVolume)
        {
            var windowBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (this.EquitiesParameters.HighVolumePercentageWindow.HasValue)
            {
                windowBreach = this.WindowVolumeCheck(mostRecentTrade, tradedVolume);
            }

            return windowBreach;
        }

        /// <summary>
        /// The check market cap.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="tradedSecurities">
        /// The traded securities.
        /// </param>
        /// <returns>
        /// The <see cref="BreachDetails"/>.
        /// </returns>
        private HighVolumeRuleBreach.BreachDetails CheckMarketCap(Order mostRecentTrade, List<Order> tradedSecurities)
        {
            var marketCapBreach = HighVolumeRuleBreach.BreachDetails.None();
            if (this.EquitiesParameters.HighVolumePercentageMarketCap.HasValue)
            {
                marketCapBreach = this.MarketCapCheck(mostRecentTrade, tradedSecurities);
            }

            return marketCapBreach;
        }

        /// <summary>
        /// The has no breach.
        /// </summary>
        /// <param name="dailyBreach">
        /// The daily breach.
        /// </param>
        /// <param name="windowBreach">
        /// The window breach.
        /// </param>
        /// <param name="marketCapBreach">
        /// The market cap breach.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasNoBreach(
            HighVolumeRuleBreach.BreachDetails dailyBreach,
            HighVolumeRuleBreach.BreachDetails windowBreach,
            HighVolumeRuleBreach.BreachDetails marketCapBreach)
        {
            return (!dailyBreach?.HasBreach ?? true)
                   && (!windowBreach?.HasBreach ?? true)
                   && (!marketCapBreach?.HasBreach ?? true);
        }

        /// <summary>
        /// The daily volume check.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="BreachDetails"/>.
        /// </returns>
        private HighVolumeRuleBreach.BreachDetails DailyVolumeCheck(Order mostRecentTrade, decimal tradedVolume)
        {
            if (mostRecentTrade == null)
            {
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradingHours = this.TradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.Logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                this.RuleCtx?.Id(),
                DataSource.AnyInterday);

            var securityResult = UniverseEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                this.HadMissingData = true;
                this.Logger.LogWarning($"Missing data for {marketDataRequest}.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            var threshold = (long)Math.Ceiling(
                this.EquitiesParameters.HighVolumePercentageDaily.GetValueOrDefault(0) * security.DailySummaryTimeBar.DailyVolume.Traded);

            if (threshold <= 0)
            {
                this.HadMissingData = true;
                this.Logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
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

        /// <summary>
        /// The window volume check.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="BreachDetails"/>.
        /// </returns>
        private HighVolumeRuleBreach.BreachDetails WindowVolumeCheck(Order mostRecentTrade, decimal tradedVolume)
        {
            var tradingHours = this.TradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.Logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var tradingDates = this.TradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
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
                    this.RuleCtx?.Id(),
                    DataSource.AnyIntraday);

            var marketResult = this.UniverseEquityIntradayCache.GetMarketsForRange(marketRequest, tradingDates, RunMode);

            if (marketResult.HadMissingData)
            {
                this.Logger.LogTrace($"Unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                this.HadMissingData = true;
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var securityDataTicks = marketResult.Response;           
            var windowVolume = securityDataTicks.Sum(sdt => sdt.SpreadTimeBar.Volume.Traded);
            var threshold = (long)Math.Ceiling(this.EquitiesParameters.HighVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

            var breachPercentage =
                windowVolume != 0 && tradedVolume != 0
                    ? (decimal)tradedVolume / (decimal)windowVolume
                    : 0;

            if (threshold <= 0)
            {
                this.HadMissingData = true;
                this.Logger.LogInformation($"Window volume threshold of {threshold} was recorded.");
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            if (tradedVolume >= threshold)
            {
                return new HighVolumeRuleBreach.BreachDetails(true, breachPercentage, threshold, mostRecentTrade.Market);
            }

            return HighVolumeRuleBreach.BreachDetails.None();
        }

        /// <summary>
        /// The market cap check.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="trades">
        /// The trades.
        /// </param>
        /// <returns>
        /// The <see cref="BreachDetails"/>.
        /// </returns>
        private HighVolumeRuleBreach.BreachDetails MarketCapCheck(Order mostRecentTrade, List<Order> trades)
        {
            if (trades == null
                || !trades.Any())
            {
                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var tradingHours = this.TradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.Logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(this.UniverseDateTime),
                this.RuleCtx?.Id(),
                DataSource.AnyInterday);

            var securityResult = this.UniverseEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                this.HadMissingData = true;
                this.Logger.LogInformation($"Missing data for {marketDataRequest}.");

                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var security = securityResult.Response;
            var marketCapMoney = security.DailySummaryTimeBar.MarketCap;

            if (marketCapMoney == null)
            {
                this.Logger.LogInformation($"Missing data for {marketDataRequest}.");

                return HighVolumeRuleBreach.BreachDetails.None();
            }

            var convertedMarketCap =
                this.CurrencyConverterService.Convert(
                    new[] { marketCapMoney.Value },
                    mostRecentTrade.OrderCurrency,
                    this.UniverseDateTime,
                    this.RuleCtx).Result;

            if (convertedMarketCap == null)
            {
                this.HadMissingData = true;
                this.Logger.LogInformation($"Missing data for exchange rates between USD and {mostRecentTrade.OrderCurrency} on {this.UniverseDateTime}");

                return HighVolumeRuleBreach.BreachDetails.None();
            }

            double thresholdValue =
                (double)Math.Ceiling(this.EquitiesParameters.HighVolumePercentageMarketCap.GetValueOrDefault(0)
                * convertedMarketCap.Value.Value);

            if (thresholdValue <= 0)
            {
                this.HadMissingData = true;
                this.Logger.LogInformation($"Market cap threshold of {thresholdValue} was recorded.");

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

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        { }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        { }

        /// <summary>
        /// The run post order event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run initial submission event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run order filled event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.Logger.LogInformation("Genesis occurred");
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.Logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.Logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred {exchange?.MarketClose}");
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.Logger.LogInformation("Eschaton occured");

            if (this.HadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.HighVolume, null, this.RuleCtx, false, true);
                this.AlertStream.Add(alert);

                this.DataRequestSubscriber.SubmitRequest();
                this.RuleCtx.EndEvent();
            }
            else
            {
                this.RuleCtx?.EndEvent();
            }
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (HighVolumeRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            var clone = (HighVolumeRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
