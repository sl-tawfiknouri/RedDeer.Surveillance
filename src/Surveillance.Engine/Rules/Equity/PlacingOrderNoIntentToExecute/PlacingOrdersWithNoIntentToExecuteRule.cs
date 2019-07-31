using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
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

namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
{
    public class PlacingOrdersWithNoIntentToExecuteRule : BaseUniverseRule, IPlacingOrdersWithNoIntentToExecuteRule
    {
        private bool _hadMissingData;
        private bool _processingMarketClose;
        private MarketOpenClose _latestMarketClosure;

        private readonly ILogger _logger;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private readonly IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters _parameters;

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
                Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute,
                EquityRulePlacingOrdersWithoutIntentToExecuteFactory.Version,
                "Placing Orders With No Intent To Execute Rule",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            if (!_processingMarketClose
                || _latestMarketClosure == null)
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
                _logger.LogInformation("RunPostOrderEvent did not have any orders to check after filtering for invalid order status values");
                return;
            }

            var openingHours = _latestMarketClosure.MarketClose - _latestMarketClosure.MarketOpen;
            var benchmarkOrder = ordersToCheck.First();

            var tradingHours = _tradingHoursService.GetTradingHoursForMic(benchmarkOrder.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {benchmarkOrder.Market?.MarketIdentifierCode}");
            }

            var tradingDates = _tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                benchmarkOrder.Market?.MarketIdentifierCode);

            var marketDataRequest = new MarketDataRequest(
                benchmarkOrder.Market.MarketIdentifierCode,
                benchmarkOrder.Instrument.Cfi,
                benchmarkOrder.Instrument.Identifiers,
                UniverseDateTime.Subtract(openingHours), // implicitly correct (market closure event trigger)
                UniverseDateTime,
                _ruleCtx?.Id(),
                DataSource.AllIntraday);

            var dataResponse = UniverseEquityIntradayCache.GetMarketsForRange(marketDataRequest, tradingDates, RunMode);

            if (dataResponse.HadMissingData
                || dataResponse.Response == null
                || !dataResponse.Response.Any())
            {
                _logger.LogInformation($"RunPostOrderEvent could not find relevant market data for {benchmarkOrder.Instrument?.Identifiers} on {_latestMarketClosure?.MarketId}");
                _hadMissingData = true;
                return;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            var pricesInTimeBars = dataResponse.Response.Select(_ => (double)_.SpreadTimeBar.Price.Value).ToList();
            var sd = (decimal)MathNet.Numerics.Statistics.Statistics.StandardDeviation(pricesInTimeBars);
            var mean = (decimal)MathNet.Numerics.Statistics.Statistics.Mean(pricesInTimeBars);

            var ruleBreaches = 
                ordersToCheck
                    .Select(_ => ReferenceOrderSigma(_, sd, mean))
                    .Where(_ => _.Item1 > 0 && _.Item1 > _parameters.Sigma)
                    .Where(_ => CheckIfOrderWouldNotOfExecuted(_, dataResponse.Response))
                    .ToList();

            if (!ruleBreaches.Any())
            {
                return;
            }

            var position = new TradePosition(ruleBreaches.Select(_ => _.Item2).ToList());
            var poe = ruleBreaches.Select(_ => ExecutionUnderNormalDistribution(_.Item2, mean, sd, _.Item1)).ToList();

            // wrong but should be a judgement
            var breach =
                new PlacingOrderWithNoIntentToExecuteRuleRuleBreach(
                    _parameters.Windows.BackwardWindowSize,
                    position,
                    benchmarkOrder.Instrument,
                    OrganisationFactorValue,
                    mean, 
                    sd,
                    poe,
                    _parameters,
                    _ruleCtx,
                    null,
                    null,
                    UniverseDateTime);

            var alertEvent = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute, breach, _ruleCtx);
            _alertStream.Add(alertEvent);
        }

        private bool CheckIfOrderWouldNotOfExecuted(
            Tuple<decimal, Order> order,
            List<Domain.Core.Markets.Timebars.EquityInstrumentIntraDayTimeBar> timeBars)
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
                _logger?.LogError($"{exception.Message}");
                throw exception;
            }
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // placing order with no intent to execute will not use this
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // placing order with no intent to execute will not use this
        }

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
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");

            _processingMarketClose = true;
            _latestMarketClosure = exchange;
            RunRuleForAllTradingHistoriesInMarket(exchange, exchange?.MarketClose);
            _processingMarketClose = false;
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occurred");

            if (_hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(
                    Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute, 
                    null,
                    _ruleCtx,
                    false,
                    true);

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
            var clone = (PlacingOrdersWithNoIntentToExecuteRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (PlacingOrdersWithNoIntentToExecuteRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
