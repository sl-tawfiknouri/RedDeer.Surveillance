namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using MathNet.Numerics.Distributions;
    using MathNet.Numerics.Statistics;

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
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class PlacingOrdersWithNoIntentToExecuteRule : BaseUniverseRule, IPlacingOrdersWithNoIntentToExecuteRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly ILogger _logger;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters _parameters;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private bool _hadMissingData;

        private MarketOpenClose _latestMarketClosure;

        private bool _processingMarketClose;

        public PlacingOrdersWithNoIntentToExecuteRule(
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters parameters,
            IUniverseOrderFilter orderFilter,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory marketCacheFactory,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IMarketTradingHoursService tradingHoursService,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                TimeSpan.FromHours(24),
                TimeSpan.Zero,
                Rules.PlacingOrderWithNoIntentToExecute,
                EquityRulePlacingOrdersWithoutIntentToExecuteFactory.Version,
                "Placing Orders With No Intent To Execute Rule",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._dataRequestSubscriber =
                dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (PlacingOrdersWithNoIntentToExecuteRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (PlacingOrdersWithNoIntentToExecuteRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // placing order with no intent to execute will not use this
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation("Eschaton occurred");

            if (this._hadMissingData && this.RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(
                    Rules.PlacingOrderWithNoIntentToExecute,
                    null,
                    this._ruleCtx,
                    false,
                    true);

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
            this._logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");

            this._processingMarketClose = true;
            this._latestMarketClosure = exchange;
            this.RunRuleForAllTradingHistoriesInMarket(exchange, exchange?.MarketClose);
            this._processingMarketClose = false;
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // placing order with no intent to execute will not use this
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            if (!this._processingMarketClose || this._latestMarketClosure == null) return;

            var ordersToCheck = history.ActiveTradeHistory().Where(
                _ => _.OrderLimitPrice?.Value != null && _.OrderType == OrderTypes.LIMIT
                                                      && (_.OrderStatus() == OrderStatus.Placed
                                                          || _.OrderStatus() == OrderStatus.Booked
                                                          || _.OrderStatus() == OrderStatus.Amended
                                                          || _.OrderStatus() == OrderStatus.Cancelled
                                                          || _.OrderStatus() == OrderStatus.Rejected)).ToList();

            if (!ordersToCheck.Any())
            {
                this._logger.LogInformation(
                    "RunPostOrderEvent did not have any orders to check after filtering for invalid order status values");
                return;
            }

            var openingHours = this._latestMarketClosure.MarketClose - this._latestMarketClosure.MarketOpen;
            var benchmarkOrder = ordersToCheck.First();

            var tradingHours =
                this._tradingHoursService.GetTradingHoursForMic(benchmarkOrder.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {benchmarkOrder.Market?.MarketIdentifierCode}");

            var tradingDates = this._tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                benchmarkOrder.Market?.MarketIdentifierCode);

            var marketDataRequest = new MarketDataRequest(
                benchmarkOrder.Market.MarketIdentifierCode,
                benchmarkOrder.Instrument.Cfi,
                benchmarkOrder.Instrument.Identifiers,
                this.UniverseDateTime.Subtract(openingHours), // implicitly correct (market closure event trigger)
                this.UniverseDateTime,
                this._ruleCtx?.Id(),
                DataSource.AllIntraday);

            var dataResponse = this.UniverseEquityIntradayCache.GetMarketsForRange(
                marketDataRequest,
                tradingDates,
                this.RunMode);

            if (dataResponse.HadMissingData || dataResponse.Response == null || !dataResponse.Response.Any())
            {
                this._logger.LogInformation(
                    $"RunPostOrderEvent could not find relevant market data for {benchmarkOrder.Instrument?.Identifiers} on {this._latestMarketClosure?.MarketId}");
                this._hadMissingData = true;
                return;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            var pricesInTimeBars = dataResponse.Response.Select(_ => (double)_.SpreadTimeBar.Price.Value).ToList();
            var sd = (decimal)pricesInTimeBars.StandardDeviation();
            var mean = (decimal)pricesInTimeBars.Mean();

            var ruleBreaches = ordersToCheck.Select(_ => this.ReferenceOrderSigma(_, sd, mean))
                .Where(_ => _.Item1 > 0 && _.Item1 > this._parameters.Sigma)
                .Where(_ => this.CheckIfOrderWouldNotOfExecuted(_, dataResponse.Response)).ToList();

            if (!ruleBreaches.Any()) return;

            var position = new TradePosition(ruleBreaches.Select(_ => _.Item2).ToList());
            var poe = ruleBreaches.Select(_ => this.ExecutionUnderNormalDistribution(_.Item2, mean, sd, _.Item1))
                .ToList();

            // wrong but should be a judgement
            var breach = new PlacingOrderWithNoIntentToExecuteRuleRuleBreach(
                this._parameters.Windows.BackwardWindowSize,
                position,
                benchmarkOrder.Instrument,
                this.OrganisationFactorValue,
                mean,
                sd,
                poe,
                this._parameters,
                this._ruleCtx,
                null,
                null,
                this.UniverseDateTime);

            var alertEvent = new UniverseAlertEvent(Rules.PlacingOrderWithNoIntentToExecute, breach, this._ruleCtx);
            this._alertStream.Add(alertEvent);
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private bool CheckIfOrderWouldNotOfExecuted(
            Tuple<decimal, Order> order,
            List<EquityInstrumentIntraDayTimeBar> timeBars)
        {
            if (order?.Item2 == null) return true;

            if (timeBars == null || !timeBars.Any()) return true;

            var init = order.Item2.PlacedDate;
            var end = order.Item2.CancelledDate ?? order.Item2.RejectedDate;
            var tbars = new List<EquityInstrumentIntraDayTimeBar>(timeBars);

            if (init == null) return true;

            if (end != null) tbars = tbars.Where(_ => _.TimeStamp <= end).ToList();

            if (!tbars.Any()) return true;

            if (order.Item2.OrderDirection == OrderDirections.BUY
                || order.Item2.OrderDirection == OrderDirections.COVER)
            {
                var bestBuyPointDuringLiveDay = tbars.Min(_ => _.SpreadTimeBar.Price.Value);
                return bestBuyPointDuringLiveDay >= order.Item2.OrderLimitPrice?.Value;
            }

            if (order.Item2.OrderDirection == OrderDirections.SELL
                || order.Item2.OrderDirection == OrderDirections.SHORT)
            {
                var bestSellPointDuringLiveDay = tbars.Max(_ => _.SpreadTimeBar.Price.Value);
                return bestSellPointDuringLiveDay <= order.Item2.OrderLimitPrice?.Value;
            }

            throw new ArgumentOutOfRangeException(nameof(order.Item2.OrderDirection));
        }

        private PlacingOrderWithNoIntentToExecuteRuleRuleBreach.ProbabilityOfExecution ExecutionUnderNormalDistribution(
            Order order,
            decimal mean,
            decimal sd,
            decimal sigma)
        {
            if (order?.OrderLimitPrice == null) return null;

            var z = order.OrderLimitPrice.Value.Value;
            var cdf = (decimal)Normal.CDF((double)mean, (double)sd, (double)z);

            if (order.OrderDirection == OrderDirections.BUY || order.OrderDirection == OrderDirections.COVER)
            {
                var buyAdjust = 1 - cdf;
                return new PlacingOrderWithNoIntentToExecuteRuleRuleBreach.ProbabilityOfExecution(
                    order.OrderId,
                    sd,
                    mean,
                    order.OrderLimitPrice.Value.Value,
                    buyAdjust,
                    sigma);
            }

            if (order.OrderDirection == OrderDirections.SELL || order.OrderDirection == OrderDirections.SHORT)
                return new PlacingOrderWithNoIntentToExecuteRuleRuleBreach.ProbabilityOfExecution(
                    order.OrderId,
                    sd,
                    mean,
                    order.OrderLimitPrice.Value.Value,
                    cdf,
                    sigma);

            var exception = new ArgumentOutOfRangeException(nameof(order.OrderDirection));
            this._logger?.LogError($"{exception.Message}");
            throw exception;
        }

        private Tuple<decimal, Order> ReferenceOrderSigma(Order order, decimal sd, decimal mean)
        {
            // guard clauses are order sensitive
            if (order == null) return new Tuple<decimal, Order>(0, null);

            if ((order?.OrderLimitPrice?.Value ?? 0) == 0) return new Tuple<decimal, Order>(0, order);

            if (mean == 0) return new Tuple<decimal, Order>(0, order);

            if (mean == (order?.OrderLimitPrice?.Value ?? 0)) return new Tuple<decimal, Order>(0, order);

            if (sd == 0) return new Tuple<decimal, Order>(0, order);

            var differenceFromMean = Math.Abs(mean - (order.OrderLimitPrice?.Value ?? 0));

            if (differenceFromMean == 0) return new Tuple<decimal, Order>(0, order);

            var sigma = differenceFromMean / sd;

            return new Tuple<decimal, Order>(sigma, order);
        }
    }
}