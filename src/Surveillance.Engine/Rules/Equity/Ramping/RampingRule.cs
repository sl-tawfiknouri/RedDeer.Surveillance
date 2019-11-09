namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// We've tried to solve some issues imperatively with logic that can be
    /// solved with using statistics - such as using cross correlation and/or auto correlations
    /// in order to keep this rule simple/easy to understand
    /// if we need to increase the sophistication of this rule - stats is the route to go down - RT 07/05/2019
    /// </summary>
    public class RampingRule : BaseUniverseRule, IRampingRule
    {
        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The ramping parameters.
        /// </summary>
        private readonly IRampingRuleEquitiesParameters rampingParameters;

        /// <summary>
        /// The trading hours service.
        /// </summary>
        private readonly IMarketTradingHoursService tradingHoursService;

        /// <summary>
        /// The rule context.
        /// </summary>
        private readonly ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The ramping analyzer.
        /// </summary>
        private readonly IRampingAnalyser rampingAnalyzer;

        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream alertStream;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The had missing data.
        /// </summary>
        private bool hadMissingData = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RampingRule"/> class.
        /// </summary>
        /// <param name="rampingParameters">
        /// The ramping parameters.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="equityFactory">
        /// The factory.
        /// </param>
        /// <param name="fixedIncomeFactory">
        /// The factory.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="rampingAnalyzer">
        /// The ramping analyzer.
        /// </param>
        /// <param name="tradingHoursService">
        /// The trading hours service.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingStackLogger">
        /// The trading stack logger.
        /// </param>
        public RampingRule(
            IRampingRuleEquitiesParameters rampingParameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseEquityMarketCacheFactory equityFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeFactory,
            IUniverseOrderFilter orderFilter,
            RuleRunMode runMode,
            IRampingAnalyser rampingAnalyzer,
            IMarketTradingHoursService tradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                rampingParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(7),
                TimeSpan.FromDays(30),
                rampingParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.Ramping,
                EquityRuleRampingFactory.Version,
                "Ramping Rule",
                ruleContext,
                equityFactory,
                fixedIncomeFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this.rampingParameters = rampingParameters ?? throw new ArgumentNullException(nameof(rampingParameters));
            this.alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.ruleContext = ruleContext ?? throw new ArgumentNullException(nameof(ruleContext));
            this.orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this.rampingAnalyzer = rampingAnalyzer ?? throw new ArgumentNullException(nameof(rampingAnalyzer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this.dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
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
            if (this.rampingParameters == null)
            {
                return RuleDataConstraint.Empty().Case;
            }

            var constraint = new RuleDataSubConstraint(
                this.ForwardWindowSize,
                this.TradeBackwardWindowSize,
                DataSource.AnyIntraday,
                _ => !this.orderFilter.Filter(_));

            return new RuleDataConstraint(
                this.Rule,
                this.rampingParameters.Id,
                new[] { constraint });
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
            return this.orderFilter.Filter(value);
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            if (!this.ExceedsTradingFrequencyThreshold(tradeWindow))
            {
                // LOG THEN EXIT
                this.logger.LogInformation($"Trading Frequency of {this.rampingParameters.ThresholdOrdersExecutedInWindow} was not exceeded. Returning.");
                return;
            }

            if (!this.ExceedsTradingVolumeInWindowThreshold(tradeWindow.ToList(), tradeWindow.Any() ? tradeWindow.Peek() : null))
            {
                // LOG THEN EXIT
                this.logger.LogInformation($"Trading Volume of {this.rampingParameters.ThresholdVolumePercentageWindow} was not exceeded. Returning.");
                return;
            }

            var lastTrade = tradeWindow.Any() ? tradeWindow.Peek() : null;
            var tradingHours = this.tradingHoursService.GetTradingHoursForMic(lastTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.logger.LogError($"Request for trading hours was invalid. MIC - {lastTrade.Market?.MarketIdentifierCode}");
                return;
            }

            var tradingDates = this.tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                lastTrade.Market?.MarketIdentifierCode);

            var subtractDate = this.TradeBackwardWindowSize > TimeSpan.FromDays(30) ? this.TradeBackwardWindowSize : TimeSpan.FromDays(30);

            var marketDataRequest = new MarketDataRequest(
                lastTrade.Market?.MarketIdentifierCode,
                lastTrade.Instrument.Cfi,
                lastTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(subtractDate)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                this.ruleContext?.Id(),
                DataSource.AnyIntraday);

            var marketData = UniverseEquityIntradayCache.GetMarketsForRange(marketDataRequest, tradingDates, RunMode);

            if (marketData.HadMissingData)
            {
                this.hadMissingData = true;
                this.logger.LogWarning($"Missing data for {marketDataRequest}.");
                return;
            }

            var windowStackCopy = new Stack<Order>(tradeWindow);
            var rampingOrders = new Stack<Order>(windowStackCopy);
            var rampingAnalysisResults = new List<IRampingStrategySummaryPanel>();

            while (rampingOrders.Any())
            {
                var rampingOrderList = rampingOrders.ToList();
                var rootOrder = rampingOrders.Any() ? rampingOrders.Peek() : null;
                var marketDataSubset = marketData.Response.Where(_ => _.TimeStamp <= rootOrder.FilledDate).ToList();
                var rampingAnalysisResult = this.rampingAnalyzer.Analyse(rampingOrderList, marketDataSubset);
                rampingAnalysisResults.Add(rampingAnalysisResult);
                rampingOrders.Pop();
            }

            if (!rampingAnalysisResults.Any()
                || !rampingAnalysisResults.First().HasRampingStrategy()
                || rampingAnalysisResults.All(_ => !_.HasRampingStrategy()))
            {
                // LOG THEN EXIT
                this.logger.LogInformation($"A rule breach was not detected for {lastTrade?.Instrument?.Identifiers}. Returning.");
                return;
            }

            var rampingPrevalence = this.RampingPrevalence(rampingAnalysisResults);
            if (rampingPrevalence < this.rampingParameters.AutoCorrelationCoefficient)
            {
                // LOG THEN EXIT
                this.logger.LogInformation($"A rule breach was not detected due to an auto correlation of {rampingPrevalence} for {lastTrade?.Instrument?.Identifiers}. Returning.");
                return;
            }

            var tradePosition = new TradePosition(tradeWindow.ToList());

            // wrong but should be a judgement
            var breach =
                new RampingRuleBreach(
                    this.TradeBackwardWindowSize,
                    tradePosition,
                    lastTrade.Instrument,
                    this.rampingParameters.Id,
                    this.ruleContext?.Id(),
                    this.ruleContext?.CorrelationId(),
                    this.OrganisationFactorValue,
                    rampingAnalysisResults.Last(),
                    this.rampingParameters,
                    null,
                    null,
                    this.UniverseDateTime);

            this.logger.LogInformation($"RunRule has breached parameter conditions for {lastTrade?.Instrument?.Identifiers}. Adding message to alert stream.");
            var message = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Ramping, breach, this.ruleContext);
            this.alertStream.Add(message);
        }

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
        /// The ramping prevalence.
        /// </summary>
        /// <param name="panels">
        /// The panels.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public decimal RampingPrevalence(List<IRampingStrategySummaryPanel> panels)
        {
            if (panels == null
                || !panels.Any())
            {
                return 0;
            }

            decimal rampCount = panels.Count(_ => _.HasRampingStrategy());
            decimal totals = panels.Count;

            if (rampCount == 0)
            {
                return 0;
            }

            var rampingPercentage = rampCount / totals;

            return rampingPercentage;
        }

        /// <summary>
        /// The exceeds trading frequency threshold.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ExceedsTradingFrequencyThreshold(Stack<Order> orders)
        {
            if (this.rampingParameters?.ThresholdOrdersExecutedInWindow == null)
            {
                return true;
            }

            if (orders == null
                || !orders.Any())
            {
                return false;
            }

            return orders.Count >= this.rampingParameters.ThresholdOrdersExecutedInWindow.GetValueOrDefault(0);
        }

        /// <summary>
        /// The exceeds trading volume in window threshold.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ExceedsTradingVolumeInWindowThreshold(List<Order> orders, Order mostRecentTrade)
        {
            if (this.rampingParameters?.ThresholdVolumePercentageWindow == null
                || this.rampingParameters.ThresholdVolumePercentageWindow <= 0
                || orders == null
                || !orders.Any())
            {
                return true;
            }

            var tradingHours = this.tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
            }

            var tradingDates = this.tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
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
                    this.ruleContext?.Id(),
                    DataSource.AnyIntraday);

            var marketResult = UniverseEquityIntradayCache.GetMarketsForRange(marketRequest, tradingDates, RunMode);

            if (marketResult.HadMissingData)
            {
                this.logger.LogTrace($"Unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                this.hadMissingData = true;
                return false;
            }

            var securityDataTicks = marketResult.Response;
            var windowVolume = securityDataTicks.Sum(sdt => sdt.SpreadTimeBar.Volume.Traded);
            var threshold = (long)Math.Ceiling(this.rampingParameters.ThresholdVolumePercentageWindow.GetValueOrDefault(0) * windowVolume);

            if (threshold <= 0)
            {
                this.hadMissingData = true;
                this.logger.LogInformation($"Daily volume threshold of {threshold} was recorded.");
                return false;
            }

            var tradedVolume = orders.Sum(_ => _.OrderFilledVolume.GetValueOrDefault(0));
                
            if (tradedVolume >= threshold)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        { 
            // we don't use post order event in ramping rule
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // we don't use post order event in ramping rule
        }

        /// <summary>
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation("Genesis occurred");
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation("Eschaton occured");

            if (this.hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Ramping, null, this.ruleContext, false, true);
                this.alertStream.Add(alert);

                this.dataRequestSubscriber.SubmitRequest();
            }

            this.ruleContext?.EndEvent();
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
            var clone = (RampingRule)Clone();
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
            var clone = (RampingRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
