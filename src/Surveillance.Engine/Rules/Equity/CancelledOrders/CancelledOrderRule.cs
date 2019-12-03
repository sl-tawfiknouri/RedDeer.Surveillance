namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    ///     Cancelled Orders Rule
    ///     Ignores rule run mode as it doesn't use market data
    /// </summary>
    public class CancelledOrderRule : BaseUniverseRule, ICancelledOrderRule
    {
        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream alertStream;

        /// <summary>
        /// The operation context.
        /// </summary>
        private readonly ISystemProcessOperationRunRuleContext operationContext;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// The parameters.
        /// </summary>
        private readonly ICancelledOrderRuleEquitiesParameters parameters;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledOrderRule"/> class.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
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
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// The trading history logger.
        /// </param>
        public CancelledOrderRule(
            ICancelledOrderRuleEquitiesParameters parameters,
            ISystemProcessOperationRunRuleContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            RuleRunMode runMode,
            ILogger<CancelledOrderRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(60),
                parameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(60),
                parameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.CancelledOrders,
                Versioner.Version(2, 0),
                "Cancelled Order Rule",
                operationContext,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.operationContext = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            this.alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

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
            var clone = (CancelledOrderRule)this.Clone();
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
            var clone = (CancelledOrderRule)this.MemberwiseClone();
            clone.BaseClone();

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
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public override IRuleDataConstraint DataConstraints()
        {
            var constraint = new RuleDataSubConstraint(
                this.ForwardWindowSize,
                this.TradeBackwardWindowSize,
                DataSource.NoPrices,
                _ => !this.orderFilter.Filter(_));

            return new RuleDataConstraint(
                this.Rule, 
                this.parameters.Id, 
                new[] { constraint });
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation("Universe Eschaton occurred");
            this.operationContext?.EndEvent();
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
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation("Universe Genesis occurred");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Trading closed for exchange {exchange.MarketId}");
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Trading Opened for exchange {exchange.MarketId}");
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
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
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null || !tradeWindow.Any())
            {
                return;
            }

            var mostRecentTrade = tradeWindow.Pop();

            var tradingPosition = new TradePositionCancellations(
                new List<Order>(),
                this.parameters.CancelledOrderPercentagePositionThreshold,
                this.parameters.CancelledOrderCountPercentageThreshold,
                this.logger);

            tradingPosition.Add(mostRecentTrade);
            var ruleBreach = this.CheckPositionForCancellations(tradeWindow, mostRecentTrade, tradingPosition);

            if (ruleBreach.HasBreachedRule())
            {
                this.logger.LogInformation(
                    $"RunRule has breached parameter conditions for {mostRecentTrade?.Instrument?.Identifiers}. Adding message to alert stream.");
                var message = new UniverseAlertEvent(Rules.CancelledOrders, ruleBreach, this.operationContext);
                this.alertStream.Add(message);
            }
            else
            {
                this.logger.LogInformation(
                    $"RunRule did not breach parameter conditions for {mostRecentTrade?.Instrument?.Identifiers}.");
            }
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
        /// The check position for cancellations.
        /// </summary>
        /// <param name="tradeWindow">
        /// The trade window.
        /// </param>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="tradingPosition">
        /// The trading position.
        /// </param>
        /// <returns>
        /// The <see cref="ICancelledOrderRuleBreach"/>.
        /// </returns>
        private ICancelledOrderRuleBreach CheckPositionForCancellations(
            Stack<Order> tradeWindow,
            Order mostRecentTrade,
            ITradePositionCancellations tradingPosition)
        {
            var hasBreachedRuleByOrderCount = false;
            var hasBreachedRuleByPositionSize = false;
            var cancellationRatioByOrderCount = 0m;
            var cancellationRatioByPositionSize = 0m;

            var hasTradesInWindow = tradeWindow.Any();

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (hasTradesInWindow)
            {
                if (!tradeWindow.Any())
                {
                    // ReSharper disable once RedundantAssignment
                    hasTradesInWindow = false;
                    break;
                }

                var nextTrade = tradeWindow.Pop();

                tradingPosition.Add(nextTrade);

                if (this.parameters.MinimumNumberOfTradesToApplyRuleTo > tradingPosition.Get().Count
                    || this.parameters.MaximumNumberOfTradesToApplyRuleTo.HasValue
                    && this.parameters.MaximumNumberOfTradesToApplyRuleTo.Value
                    < tradingPosition.Get().Count) continue;

                if (this.parameters.CancelledOrderCountPercentageThreshold != null
                    && tradingPosition.HighCancellationRatioByTradeCount())
                {
                    hasBreachedRuleByOrderCount = true;
                    cancellationRatioByOrderCount = tradingPosition.CancellationRatioByTradeCount();
                }

                if (this.parameters.CancelledOrderPercentagePositionThreshold != null
                    && tradingPosition.HighCancellationRatioByPositionSize())
                {
                    hasBreachedRuleByPositionSize = true;
                    cancellationRatioByPositionSize = tradingPosition.CancellationRatioByPositionSize();
                }
            }

            var cancelledPositionOrders = tradingPosition.Get().Count(tp => tp.OrderStatus() == OrderStatus.Cancelled);
            var totalPositionOrders = tradingPosition.Get().Count;

            // wrong should use a judgement
            return new CancelledOrderRuleBreach(
                this.OrganisationFactorValue,
                this.operationContext.SystemProcessOperationContext(),
                this.RuleCtx.CorrelationId(),
                this.parameters,
                tradingPosition,
                tradingPosition?.Get()?.FirstOrDefault()?.Instrument,
                hasBreachedRuleByPositionSize,
                cancellationRatioByPositionSize,
                cancelledPositionOrders,
                totalPositionOrders,
                hasBreachedRuleByOrderCount,
                cancellationRatioByOrderCount,
                null,
                null,
                this.UniverseDateTime);
        }
    }
}