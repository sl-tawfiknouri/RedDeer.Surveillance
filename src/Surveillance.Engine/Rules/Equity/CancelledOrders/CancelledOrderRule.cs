namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    /// <summary>
    ///     Cancelled Orders Rule
    ///     Ignores rule run mode as it doesn't use market data
    /// </summary>
    public class CancelledOrderRule : BaseUniverseRule, ICancelledOrderRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly ILogger _logger;

        private readonly ISystemProcessOperationRunRuleContext _opCtx;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly ICancelledOrderRuleEquitiesParameters _parameters;

        public CancelledOrderRule(
            ICancelledOrderRuleEquitiesParameters parameters,
            ISystemProcessOperationRunRuleContext opContext,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
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
                opContext,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this._parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this._opCtx = opContext ?? throw new ArgumentNullException(nameof(opContext));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (CancelledOrderRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (CancelledOrderRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation("Universe Eschaton occurred");
            this._opCtx?.EndEvent();
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation("Universe Genesis occurred");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Trading closed for exchange {exchange.MarketId}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Trading Opened for exchange {exchange.MarketId}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null || !tradeWindow.Any()) return;

            var mostRecentTrade = tradeWindow.Pop();

            var tradingPosition = new TradePositionCancellations(
                new List<Order>(),
                this._parameters.CancelledOrderPercentagePositionThreshold,
                this._parameters.CancelledOrderCountPercentageThreshold,
                this._logger);

            tradingPosition.Add(mostRecentTrade);
            var ruleBreach = this.CheckPositionForCancellations(tradeWindow, mostRecentTrade, tradingPosition);

            if (ruleBreach.HasBreachedRule())
            {
                this._logger.LogInformation(
                    $"RunRule has breached parameter conditions for {mostRecentTrade?.Instrument?.Identifiers}. Adding message to alert stream.");
                var message = new UniverseAlertEvent(Rules.CancelledOrders, ruleBreach, this._opCtx);
                this._alertStream.Add(message);
            }
            else
            {
                this._logger.LogInformation(
                    $"RunRule did not breach parameter conditions for {mostRecentTrade?.Instrument?.Identifiers}.");
            }
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

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

                if (this._parameters.MinimumNumberOfTradesToApplyRuleTo > tradingPosition.Get().Count
                    || this._parameters.MaximumNumberOfTradesToApplyRuleTo.HasValue
                    && this._parameters.MaximumNumberOfTradesToApplyRuleTo.Value
                    < tradingPosition.Get().Count) continue;

                if (this._parameters.CancelledOrderCountPercentageThreshold != null
                    && tradingPosition.HighCancellationRatioByTradeCount())
                {
                    hasBreachedRuleByOrderCount = true;
                    cancellationRatioByOrderCount = tradingPosition.CancellationRatioByTradeCount();
                }

                if (this._parameters.CancelledOrderPercentagePositionThreshold != null
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
                this._opCtx.SystemProcessOperationContext(),
                this.RuleCtx.CorrelationId(),
                this._parameters,
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