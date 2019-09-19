namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Dates;
    using Domain.Core.Markets;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Judgement.FixedIncome;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    /// <summary>
    /// The fixed income high volume issuance rule.
    /// </summary>
    public class FixedIncomeHighVolumeRule : BaseUniverseRule, IFixedIncomeHighVolumeRule
    {
        /// <summary>
        /// The trading hours service.
        /// </summary>
        private readonly IMarketTradingHoursService tradingHoursService;

        /// <summary>
        /// The order filter service for CFI codes.
        /// </summary>
        private readonly IUniverseFixedIncomeOrderFilterService orderFilterService;

        /// <summary>
        /// The rule run parameters.
        /// </summary>
        private readonly IHighVolumeIssuanceRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The judgement service.
        /// </summary>
        private readonly IFixedIncomeHighVolumeJudgementService judgementService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighVolumeRule> logger;

        /// <summary>
        /// The had missing market data.
        /// </summary>
        private bool hadMissingMarketData;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeRule"/> class.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="orderFilterService">
        /// The order filter service.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber
        /// </param>
        /// <param name="tradingHoursService">
        /// The trading hours service
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingStackLogger">
        /// The trading stack logger.
        /// </param>
        public FixedIncomeHighVolumeRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilterService orderFilterService,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseMarketCacheFactory factory,
            IFixedIncomeHighVolumeJudgementService judgementService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IMarketTradingHoursService tradingHoursService,
            RuleRunMode runMode,
            ILogger<FixedIncomeHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)

            : base(
                parameters.Windows.BackwardWindowSize,
                parameters.Windows.BackwardWindowSize,
                parameters.Windows.FutureWindowSize,
                Rules.FixedIncomeHighVolumeIssuance,
                FixedIncomeHighVolumeFactory.Version,
                $"{nameof(FixedIncomeHighVolumeRule)}",
                ruleContext,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this.judgementService = judgementService ?? throw new ArgumentNullException(nameof(judgementService));
            this.dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this.tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// The clone with factor value for supporting factor value brokering.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeHighVolumeRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        /// <summary>
        /// The clone object method with shallow clone implementation.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} Clone called at {this.UniverseDateTime}");

            var clone = (FixedIncomeHighVolumeRule)this.MemberwiseClone();
            clone.BaseClone();

            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} Clone completed for {this.UniverseDateTime}");

            return clone;
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} RunOrderFilledEvent called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} RunOrderFilledEvent completed for {this.UniverseDateTime}");
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
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} EndOfUniverse called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} EndOfUniverse completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The filter universe event method used for filtering via CFI code.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.orderFilterService.Filter(value);
        }

        /// <summary>
        /// The genesis method on first universe event.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} Genesis called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} Genesis completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The market close any/all.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} MarketClose called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} MarketClose completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The market open any/all.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} MarketOpen called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} MarketOpen completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} RunInitialSubmissionRule called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} RunInitialSubmissionRule completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The run initial submission event delayed for future window.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} RunRule called at {this.UniverseDateTime}");

            var tradeWindow = history?.ActiveTradeHistory() ?? new Stack<Order>();

            if (this.HasEmptyTradeWindow(tradeWindow))
            {
                this.logger.LogInformation($"RunPostOrderEvent had an empty trade window");

                return;
            }

            var tradedSecurities = tradeWindow.Where(_ => _.OrderFilledVolume.GetValueOrDefault() > 0).ToList();
            tradedSecurities = this.FilterOutOtc(tradedSecurities);
            var tradedVolume = tradedSecurities.Sum(_ => _.OrderFilledVolume.GetValueOrDefault(0));

            var tradePosition = new TradePosition(tradedSecurities.ToList());
            var mostRecentTrade = tradeWindow.Peek();

            var dailyBreach = this.CheckDailyVolume(mostRecentTrade, tradedVolume);
            var windowBreach = this.CheckWindowVolume(mostRecentTrade, tradedVolume);

            if (this.HasNoBreach(dailyBreach, windowBreach))
            {
                this.logger.LogInformation($"RunPostOrderEvent passing judgement with no daily or window breach for {mostRecentTrade.Instrument.Identifiers}");
                this.PassJudgementForNoBreachAsync(mostRecentTrade, tradePosition).Wait();
            }

            if (windowBreach.HasBreach)
            {
                this.logger.LogInformation($"RunPostOrderEvent passing judgement with window breach for {mostRecentTrade.Instrument.Identifiers}");
                this.PassJudgementForWindowBreachAsync(mostRecentTrade, windowBreach, tradePosition).Wait();
            }

            if (dailyBreach.HasBreach)
            {
                this.logger.LogInformation($"RunPostOrderEvent passing judgement with no daily breach for {mostRecentTrade.Instrument.Identifiers}");
                this.PassJudgementForDailyBreachAsync(mostRecentTrade, dailyBreach, tradePosition).Wait();
            }
        }

        /// <summary>
        /// The run post order event delayed for future window.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The pass judgement for no breach async.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="tradePosition">
        /// The traded position
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PassJudgementForNoBreachAsync(Order mostRecentTrade, TradePosition tradePosition)
        {
            var serialisedParameters = JsonConvert.SerializeObject(this.parameters);

            var judgement =
                new FixedIncomeHighVolumeJudgement(
                    this.RuleCtx.RuleParameterId(),
                    this.RuleCtx.CorrelationId(),
                    mostRecentTrade?.ReddeerOrderId?.ToString(),
                    mostRecentTrade?.OrderId?.ToString(),
                    serialisedParameters,
                    this.hadMissingMarketData,
                    false,
                    null,
                    null,
                    tradePosition);

            var fixedIncomeHighVolumeContext = new FixedIncomeHighVolumeJudgementContext(judgement, false);

            await this.judgementService.Judgement(fixedIncomeHighVolumeContext);
        }

        /// <summary>
        /// The pass judgement for window breach async.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="breachDetails">
        /// The breach details.
        /// </param>
        /// <param name="tradePosition">
        /// The trade position.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PassJudgementForWindowBreachAsync(
            Order mostRecentTrade,
            FixedIncomeHighVolumeJudgement.BreachDetails breachDetails,
            TradePosition tradePosition)
        {
            var serialisedParameters = JsonConvert.SerializeObject(this.parameters);
            
            var judgement =
                new FixedIncomeHighVolumeJudgement(
                    this.RuleCtx.RuleParameterId(),
                    this.RuleCtx.CorrelationId(),
                    mostRecentTrade?.ReddeerOrderId?.ToString(),
                    mostRecentTrade?.OrderId?.ToString(),
                    serialisedParameters,
                    this.hadMissingMarketData,
                    false,
                    breachDetails,
                    null,
                    tradePosition);

            var fixedIncomeHighVolumeContext = new FixedIncomeHighVolumeJudgementContext(judgement, true);

            await this.judgementService.Judgement(fixedIncomeHighVolumeContext);
        }

        /// <summary>
        /// The pass judgement for daily breach async.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="breachDetails">
        /// The breach details.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PassJudgementForDailyBreachAsync(
            Order mostRecentTrade,
            FixedIncomeHighVolumeJudgement.BreachDetails breachDetails,
            TradePosition tradePosition)
        {
            var serialisedParameters = JsonConvert.SerializeObject(this.parameters);

            var judgement =
                new FixedIncomeHighVolumeJudgement(
                    this.RuleCtx.RuleParameterId(),
                    this.RuleCtx.CorrelationId(),
                    mostRecentTrade?.ReddeerOrderId?.ToString(),
                    mostRecentTrade?.OrderId?.ToString(),
                    serialisedParameters,
                    this.hadMissingMarketData,
                    false,
                    null,
                    breachDetails,
                    tradePosition);

            var fixedIncomeHighVolumeContext = new FixedIncomeHighVolumeJudgementContext(judgement, true);

            await this.judgementService.Judgement(fixedIncomeHighVolumeContext);
        }

        /// <summary>
        /// The check daily volume.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private FixedIncomeHighVolumeJudgement.BreachDetails CheckDailyVolume(Order order, decimal tradedVolume)
        {
            if (this.parameters.FixedIncomeHighVolumePercentageDaily == null)
            {
                this.logger.LogDebug(
                    $"Check Daily Volume called for {order?.Instrument?.Identifiers} at {this.UniverseDateTime} but has null daily percentage parameter");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            if (order == null)
            {
                this.logger.LogDebug($"Check Daily Volume called at {this.UniverseDateTime} but had a null order");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            if (tradedVolume <= 0)
            {
                this.logger.LogDebug($"Check Daily Volume called at {this.UniverseDateTime} but had a traded volume of zero");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            return this.DailyVolumeAnalysis(order, tradedVolume);
        }

        /// <summary>
        /// The daily volume analysis.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="FixedIncomeHighVolumeJudgement.BreachDetails"/>.
        /// </returns>
        private FixedIncomeHighVolumeJudgement.BreachDetails DailyVolumeAnalysis(Order order, decimal tradedVolume)
        {
            var tradingHours = this.tradingHoursService.GetTradingHoursForMic(order.Market?.MarketIdentifierCode);

            if (!tradingHours.IsValid)
            {
                this.logger.LogError($"Request for trading hours was invalid. MIC - {order.Market?.MarketIdentifierCode}");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            var marketDataRequest = this.ConstructMarketDataRequest(order, tradingHours);
            var securityResult = this.UniverseEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                this.hadMissingMarketData = true;
                this.logger.LogWarning($"Missing data for {marketDataRequest}.");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            var securityDailyData = securityResult.Response;
            var threshold = this.CalculateDailyVolumeThresholdPercentage(securityDailyData);

            if (threshold <= 0)
            {
                this.hadMissingMarketData = true;
                this.logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            var breachPercentage = this.CalculateDailyVolumePercentage(securityDailyData, tradedVolume);

            if (tradedVolume >= threshold)
            {
                return new FixedIncomeHighVolumeJudgement.BreachDetails(true, breachPercentage, threshold, order.Market);
            }

            // replace with daily volume implementation
            return FixedIncomeHighVolumeJudgement.BreachDetails.None();
        }

        /// <summary>
        /// The construct market data request.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradingHours">
        /// The trading hours.
        /// </param>
        /// <returns>
        /// The <see cref="MarketDataRequest"/>.
        /// </returns>
        private MarketDataRequest ConstructMarketDataRequest(Order order, ITradingHours tradingHours)
        {
            var opening = tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.TradeBackwardWindowSize));
            var closing = tradingHours.ClosingInUtcForDay(this.UniverseDateTime);

            return new MarketDataRequest(
                order.Market?.MarketIdentifierCode,
                order.Instrument.Cfi,
                order.Instrument.Identifiers,
                opening,
                closing,
                this.RuleCtx?.Id(),
                DataSource.AllInterday);
        }

        /// <summary>
        /// The calculate daily volume threshold percentage.
        /// </summary>
        /// <param name="securityTimeBar">
        /// The security time bar.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long CalculateDailyVolumeThresholdPercentage(EquityInstrumentInterDayTimeBar securityTimeBar)
        {
            return (long)Math.Ceiling(
                this.parameters.FixedIncomeHighVolumePercentageDaily.GetValueOrDefault(0)
                * securityTimeBar.DailySummaryTimeBar.DailyVolume.Traded);
        }

        /// <summary>
        /// The calculate daily volume percentage.
        /// </summary>
        /// <param name="securityTimeBar">
        /// The security.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateDailyVolumePercentage(EquityInstrumentInterDayTimeBar securityTimeBar, decimal tradedVolume)
        {
            if (securityTimeBar.DailySummaryTimeBar.DailyVolume.Traded != 0 && tradedVolume != 0)
            {
                return tradedVolume / securityTimeBar.DailySummaryTimeBar.DailyVolume.Traded;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// The check window volume.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private FixedIncomeHighVolumeJudgement.BreachDetails CheckWindowVolume(Order order, decimal tradedVolume)
        {
            if (this.parameters.FixedIncomeHighVolumePercentageWindow == null)
            {
                this.logger.LogDebug(
                    $"Check Window Volume called for {order?.Instrument?.Identifiers} at {this.UniverseDateTime} but has null window percentage parameter");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            if (order == null)
            {
                this.logger.LogDebug($"Check Window Volume called at {this.UniverseDateTime} but had a null order");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            if (tradedVolume <= 0)
            {
                this.logger.LogDebug($"Check Window Volume called at {this.UniverseDateTime} but had a traded volume of zero");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            return this.WindowVolumeAnalysis(order, tradedVolume);
        }

        /// <summary>
        /// The window volume analysis.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="FixedIncomeHighVolumeJudgement.BreachDetails"/>.
        /// </returns>
        private FixedIncomeHighVolumeJudgement.BreachDetails WindowVolumeAnalysis(Order order, decimal tradedVolume)
        {
            var tradingHours = this.tradingHoursService.GetTradingHoursForMic(order.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.logger.LogError($"Request for trading hours was invalid. MIC - {order.Market?.MarketIdentifierCode}");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            var tradingDates = this.SetTradingHourRange(tradingHours, order);
            var marketRequest = this.ConstructMarketDataRequestWindowAnalysis(order, tradingHours);
            var marketResult = this.UniverseEquityIntradayCache.GetMarketsForRange(marketRequest, tradingDates, this.RunMode);

            if (marketResult.HadMissingData)
            {
                this.logger.LogTrace($"Unable to fetch market data frames for {order?.Market?.MarketIdentifierCode} at {this.UniverseDateTime}.");
                this.hadMissingMarketData = true;

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            var securityDataTicks = marketResult.Response;
            var windowVolume = securityDataTicks.Sum(sdt => sdt.SpreadTimeBar.Volume.Traded);
            var threshold = this.CalculateBreachThreshold(windowVolume);
            var breachPercentage = this.CalculateTradedVolumePercentageInWindow(tradedVolume, windowVolume);

            if (threshold <= 0)
            {
                this.hadMissingMarketData = true;
                this.logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");

                return FixedIncomeHighVolumeJudgement.BreachDetails.None();
            }

            if (tradedVolume >= threshold)
            {
                return new FixedIncomeHighVolumeJudgement.BreachDetails(true, breachPercentage, threshold, order.Market);
            }

            return FixedIncomeHighVolumeJudgement.BreachDetails.None();
        }

        /// <summary>
        /// The set trading hour range.
        /// </summary>
        /// <param name="tradingHours">
        /// The trading hours.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="DateRange"/>.
        /// </returns>
        private IReadOnlyCollection<DateRange> SetTradingHourRange(ITradingHours tradingHours, Order order)
        {
            var tradingHourFrom = tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.TradeBackwardWindowSize));
            var tradingHourTo = tradingHours.ClosingInUtcForDay(this.UniverseDateTime);

            var tradingDates = this.tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHourFrom,
                tradingHourTo,
                order.Market?.MarketIdentifierCode);

            return tradingDates;
        }

        /// <summary>
        /// The calculate breach threshold.
        /// </summary>
        /// <param name="windowVolume">
        /// The window volume.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long CalculateBreachThreshold(long windowVolume)
        {
            return (long)Math.Ceiling(this.parameters.FixedIncomeHighVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);
        }

        /// <summary>
        /// The calculate traded volume percentage in window.
        /// </summary>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <param name="windowVolume">
        /// The window volume.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateTradedVolumePercentageInWindow(decimal tradedVolume, long windowVolume)
        {
            return windowVolume != 0 && tradedVolume != 0
                ? (decimal)tradedVolume / (decimal)windowVolume
                : 0;
        }

        /// <summary>
        /// The construct market data request window analysis.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradingHours">
        /// The trading hours.
        /// </param>
        /// <returns>
        /// The <see cref="MarketDataRequest"/>.
        /// </returns>
        private MarketDataRequest ConstructMarketDataRequestWindowAnalysis(Order order, ITradingHours tradingHours)
        {
            var openingHours = tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.TradeBackwardWindowSize));
            var closingHours = tradingHours.ClosingInUtcForDay(this.UniverseDateTime);

            return new MarketDataRequest(
                order.Market?.MarketIdentifierCode,
                order.Instrument.Cfi,
                order.Instrument.Identifiers,
                openingHours,
                closingHours,
                this.RuleCtx?.Id(),
                DataSource.AllIntraday);
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasNoBreach(
            FixedIncomeHighVolumeJudgement.BreachDetails dailyBreach,
            FixedIncomeHighVolumeJudgement.BreachDetails windowBreach)
        {
            return !dailyBreach.HasBreach && !windowBreach.HasBreach;
        }

        /// <summary>
        /// The has empty trade window.
        /// </summary>
        /// <param name="tradeWindow">
        /// The trade window.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasEmptyTradeWindow(Stack<Order> tradeWindow)
        {
            return tradeWindow == null || !tradeWindow.Any();
        }

        /// <summary>
        /// The filter out over the counter trades.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <returns>
        /// The <see cref="Order"/>.
        /// </returns>
        private List<Order> FilterOutOtc(IReadOnlyCollection<Order> orders)
        {
            if (orders == null || !orders.Any())
            {
                return new List<Order>();
            }

            return
                orders
                    .Where(_ => _.Market?.Type != MarketTypes.OTC)
                    .Where(_ => _.OrderType != OrderTypes.OTC)
                    .ToList();
        }
    }
}