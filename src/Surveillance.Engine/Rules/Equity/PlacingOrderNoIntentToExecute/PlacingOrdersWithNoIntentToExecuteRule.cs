namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
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
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The placing orders with no intent to execute rule.
    /// </summary>
    public class PlacingOrdersWithNoIntentToExecuteRule : BaseUniverseRule, IPlacingOrdersWithNoIntentToExecuteRule
    {
        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream alertStream;

        /// <summary>
        /// The rule context.
        /// </summary>
        private readonly ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The trading hours service.
        /// </summary>
        private readonly IMarketTradingHoursService tradingHoursService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The rule parameters.
        /// </summary>
        private readonly IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters parameters;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The had missing data.
        /// </summary>
        private bool hadMissingData;

        /// <summary>
        /// The processing market close.
        /// </summary>
        private bool processingMarketClose;

        /// <summary>
        /// The latest market closure.
        /// </summary>
        private MarketOpenClose latestMarketClosure;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingOrdersWithNoIntentToExecuteRule"/> class.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="marketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="tradingHoursService">
        /// The trading hours service.
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
        public PlacingOrdersWithNoIntentToExecuteRule(
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters parameters,
            IUniverseOrderFilter orderFilter,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseMarketCacheFactory marketCacheFactory,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IMarketTradingHoursService tradingHoursService,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger) 
            : base(
                TimeSpan.FromHours(24),
                TimeSpan.FromHours(24),
                TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute,
                EquityRulePlacingOrdersWithoutIntentToExecuteFactory.Version,
                "Placing Orders With No Intent To Execute Rule",
                ruleContext,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ruleContext = ruleContext ?? throw new ArgumentNullException(nameof(ruleContext));
            this.alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this.orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
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
            if (this.parameters == null)
            {
                return RuleDataConstraint.Empty().Case;
            }

            var constraint = new RuleDataSubConstraint(
                this.ForwardWindowSize,
                this.TradeBackwardWindowSize,
                DataSource.AllIntraday,
                _ => !this.orderFilter.Filter(_));

            return new RuleDataConstraint(
                this.Rule,
                this.parameters.Id,
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
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            if (!this.processingMarketClose
                || this.latestMarketClosure == null)
            {
                return;
            }
            
            var ordersToCheck = 
                history
                    .ActiveTradeHistory()
                    .Where(_ =>
                        _.OrderLimitPrice?.Value != null
                        && _.OrderType == OrderTypes.LIMIT
                        && (
                                _.OrderStatus() == OrderStatus.Placed
                                || _.OrderStatus() == OrderStatus.Booked
                                || _.OrderStatus() == OrderStatus.Amended
                                || _.OrderStatus() == OrderStatus.Cancelled
                                || _.OrderStatus() == OrderStatus.Rejected))
                    .ToList();

            if (!ordersToCheck.Any())
            {
                this.logger.LogInformation("RunPostOrderEvent did not have any orders to check after filtering for invalid order status values");
                return;
            }

            var openingHours = this.latestMarketClosure.MarketClose - this.latestMarketClosure.MarketOpen;
            var benchmarkOrder = ordersToCheck.First();

            var tradingHours = this.tradingHoursService.GetTradingHoursForMic(benchmarkOrder.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.logger.LogError($"Request for trading hours was invalid. MIC - {benchmarkOrder.Market?.MarketIdentifierCode}");
            }

            var tradingDates = this.tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                benchmarkOrder.Market?.MarketIdentifierCode);

            var marketDataRequest = new MarketDataRequest(
                benchmarkOrder.Market.MarketIdentifierCode,
                benchmarkOrder.Instrument.Cfi,
                benchmarkOrder.Instrument.Identifiers,
                UniverseDateTime.Subtract(openingHours), // implicitly correct (market closure event trigger)
                UniverseDateTime,
                this.ruleContext?.Id(),
                DataSource.AllIntraday);

            var dataResponse = UniverseEquityIntradayCache.GetMarketsForRange(marketDataRequest, tradingDates, RunMode);

            if (dataResponse.HadMissingData
                || dataResponse.Response == null
                || !dataResponse.Response.Any())
            {
                this.logger.LogInformation($"RunPostOrderEvent could not find relevant market data for {benchmarkOrder.Instrument?.Identifiers} on {this.latestMarketClosure?.MarketId}");
                this.hadMissingData = true;
                return;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            var pricesInTimeBars = dataResponse.Response.Select(_ => (double)_.SpreadTimeBar.Price.Value).ToList();
            var sd = (decimal)MathNet.Numerics.Statistics.Statistics.StandardDeviation(pricesInTimeBars);
            var mean = (decimal)MathNet.Numerics.Statistics.Statistics.Mean(pricesInTimeBars);

            var ruleBreaches = 
                ordersToCheck
                    .Select(_ => this.ReferenceOrderSigma(_, sd, mean))
                    .Where(_ => _.Item1 > 0 && _.Item1 > this.parameters.Sigma)
                    .Where(_ => this.CheckIfOrderWouldNotOfExecuted(_, dataResponse.Response))
                    .ToList();

            if (!ruleBreaches.Any())
            {
                return;
            }

            var position = new TradePosition(ruleBreaches.Select(_ => _.Item2).ToList());
            var poe = ruleBreaches.Select(_ => this.ExecutionUnderNormalDistribution(_.Item2, mean, sd, _.Item1)).ToList();

            // wrong but should be a judgement
            var breach =
                new PlacingOrderWithNoIntentToExecuteRuleRuleBreach(
                    this.parameters.Windows.BackwardWindowSize,
                    position,
                    benchmarkOrder.Instrument,
                    this.OrganisationFactorValue,
                    mean, 
                    sd,
                    poe,
                    this.parameters,
                    this.ruleContext,
                    null,
                    null,
                    this.UniverseDateTime);

            var alertEvent = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute, breach, this.ruleContext);
            this.alertStream.Add(alertEvent);
        }

        /// <summary>
        /// The check if order would not of executed.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="timeBars">
        /// The time bars.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Ensure that the order directions are expansive to all scenarios
        /// </exception>
        private bool CheckIfOrderWouldNotOfExecuted(
            Tuple<decimal, Order> order,
            IList<Domain.Core.Markets.Timebars.EquityInstrumentIntraDayTimeBar> timeBars)
        {
            if (order?.Item2 == null)
            {
                return true;
            }

            if (timeBars == null
                || !timeBars.Any())
            {
                return true;
            }

            var init = order.Item2.PlacedDate;
            var end = order.Item2.CancelledDate ?? order.Item2.RejectedDate;
            var tbars = new List<Domain.Core.Markets.Timebars.EquityInstrumentIntraDayTimeBar>(timeBars);

            if (init == null)
            {
                return true;
            }

            if (end != null)
            {
                tbars = tbars.Where(_ => _.TimeStamp <= end).ToList();
            }

            if (!tbars.Any())
            {
                return true;
            }

            if (order.Item2.OrderDirection == OrderDirections.BUY
                || order.Item2.OrderDirection == OrderDirections.COVER)
            {
                var bestBuyPointDuringLiveDay = tbars.Min(_ => _.SpreadTimeBar.Price.Value);
                return bestBuyPointDuringLiveDay >= order.Item2.OrderLimitPrice?.Value;
            }
            else if (order.Item2.OrderDirection == OrderDirections.SELL
                  || order.Item2.OrderDirection == OrderDirections.SHORT)
            {
                var bestSellPointDuringLiveDay = tbars.Max(_ => _.SpreadTimeBar.Price.Value);
                return bestSellPointDuringLiveDay <= order.Item2.OrderLimitPrice?.Value;
            }

            throw new ArgumentOutOfRangeException(nameof(order.Item2.OrderDirection));
        }

        /// <summary>
        /// The reference order sigma.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="sd">
        /// The standard deviation.
        /// </param>
        /// <param name="mean">
        /// The mean.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        private Tuple<decimal, Order> ReferenceOrderSigma(Order order, decimal sd, decimal mean)
        {
            // guard clauses are order sensitive
            if (order == null)
            {
                return new Tuple<decimal, Order>(0, null);
            }

            if ((order?.OrderLimitPrice?.Value ?? 0) == 0)
            {
                return new Tuple<decimal, Order>(0, order);
            }

            if (mean == 0)
            {
                return new Tuple<decimal, Order>(0, order);
            }

            if (mean == (order?.OrderLimitPrice?.Value ?? 0))
            {
                return new Tuple<decimal, Order>(0, order);
            }

            if (sd == 0)
            {
                return new Tuple<decimal, Order>(0, order);
            }

            var differenceFromMean = Math.Abs(mean - (order.OrderLimitPrice?.Value ?? 0));

            if (differenceFromMean == 0)
            {
                return new Tuple<decimal, Order>(0, order);
            }

            var sigma = differenceFromMean / sd;

            return new Tuple<decimal, Order>(sigma, order);
        }

        /// <summary>
        /// The execution under normal distribution.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="mean">
        /// The mean.
        /// </param>
        /// <param name="sd">
        /// The standard deviant.
        /// </param>
        /// <param name="sigma">
        /// The sigma.
        /// </param>
        /// <returns>
        /// The <see cref="ProbabilityOfExecution"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Ensure order direction enumeration coverage is completed
        /// </exception>
        private PlacingOrderWithNoIntentToExecuteRuleRuleBreach.ProbabilityOfExecution ExecutionUnderNormalDistribution(Order order, decimal mean, decimal sd, decimal sigma)
        {
            if (order?.OrderLimitPrice == null)
            {
                return null;
            }

            var z = order.OrderLimitPrice.Value.Value;
            var cdf = (decimal)MathNet.Numerics.Distributions.Normal.CDF((double)mean, (double)sd, (double)z);

            if (order.OrderDirection == OrderDirections.BUY || order.OrderDirection == OrderDirections.COVER)
            {
                var buyAdjust = 1 - cdf;
                return new PlacingOrderWithNoIntentToExecuteRuleRuleBreach.ProbabilityOfExecution(order.OrderId, sd, mean, order.OrderLimitPrice.Value.Value, buyAdjust, sigma);
            }
            else if (order.OrderDirection == OrderDirections.SELL || order.OrderDirection == OrderDirections.SHORT)
            {
                return new PlacingOrderWithNoIntentToExecuteRuleRuleBreach.ProbabilityOfExecution(order.OrderId, sd, mean, order.OrderLimitPrice.Value.Value, cdf, sigma);
            }
            else
            {
                var exception = new ArgumentOutOfRangeException(nameof(order.OrderDirection));
                this.logger?.LogError($"{exception.Message}");
                throw exception;
            }
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // placing order with no intent to execute will not use this
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // placing order with no intent to execute will not use this
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

            this.processingMarketClose = true;
            this.latestMarketClosure = exchange;
            this.RunRuleForAllTradingHistoriesInMarket(exchange, exchange?.MarketClose);
            this.processingMarketClose = false;
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation("Eschaton occurred");

            if (this.hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(
                    Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute, 
                    null,
                    this.ruleContext,
                    false,
                    true);

                this.alertStream.Add(alert);

                this.dataRequestSubscriber.SubmitRequest();
                this.ruleContext.EndEvent();
            }
            else
            {
                this.ruleContext?.EndEvent();
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
            var clone = (PlacingOrdersWithNoIntentToExecuteRule)Clone();
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
            var clone = (PlacingOrdersWithNoIntentToExecuteRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
