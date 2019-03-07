using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial;
using Domain.Core.Trading.Orders;
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

namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders
{
    /// <summary>
    /// Cancelled Orders Rule
    /// Ignores rule run mode as it doesn't use market data
    /// </summary>
    public class CancelledOrderRule : BaseUniverseRule, ICancelledOrderRule
    {
        private readonly ICancelledOrderRuleEquitiesParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _opCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;

        private readonly ILogger _logger;
        
        public CancelledOrderRule(
            ICancelledOrderRuleEquitiesParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            ILogger<CancelledOrderRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(60),
                Domain.Surveillance.Scheduling.Rules.CancelledOrders,
                Versioner.Version(2, 0),
                "Cancelled Order Rule",
                opCtx,
                factory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _opCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            var mostRecentTrade = tradeWindow.Pop();

            var tradingPosition =
                new TradePositionCancellations(
                    new List<Order>(),
                    _parameters.CancelledOrderPercentagePositionThreshold,
                    _parameters.CancelledOrderCountPercentageThreshold,
                    _logger);

            tradingPosition.Add(mostRecentTrade);
            var ruleBreach = CheckPositionForCancellations(tradeWindow, mostRecentTrade, tradingPosition);

            if (ruleBreach.HasBreachedRule())
            {
                _logger.LogInformation($"RunRule has breached parameter conditions for {mostRecentTrade?.Instrument?.Identifiers}. Adding message to alert stream.");
                var message = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.CancelledOrders, ruleBreach, _opCtx);
                _alertStream.Add(message);
            }
            else
            {
                _logger.LogInformation($"RunRule did not breach parameter conditions for {mostRecentTrade?.Instrument?.Identifiers}.");
            }
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

                if (_parameters.MinimumNumberOfTradesToApplyRuleTo > tradingPosition.Get().Count
                    || (_parameters.MaximumNumberOfTradesToApplyRuleTo.HasValue
                        && _parameters.MaximumNumberOfTradesToApplyRuleTo.Value < tradingPosition.Get().Count))
                {
                    continue;
                }

                if (_parameters.CancelledOrderCountPercentageThreshold != null
                    && tradingPosition.HighCancellationRatioByTradeCount())
                {
                    hasBreachedRuleByOrderCount = true;
                    cancellationRatioByOrderCount = tradingPosition.CancellationRatioByTradeCount();
                }

                if (_parameters.CancelledOrderPercentagePositionThreshold != null
                    && tradingPosition.HighCancellationRatioByPositionSize())
                {
                    hasBreachedRuleByPositionSize = true;
                    cancellationRatioByPositionSize = tradingPosition.CancellationRatioByPositionSize();
                }
            }

            var cancelledPositionOrders = tradingPosition.Get().Count(tp => tp.OrderStatus() == OrderStatus.Cancelled);
            var totalPositionOrders = tradingPosition.Get().Count;

            return new CancelledOrderRuleBreach(
                OrganisationFactorValue,
                _opCtx.SystemProcessOperationContext(),
                RuleCtx.CorrelationId(),
                _parameters,
                tradingPosition,
                tradingPosition?.Get()?.FirstOrDefault()?.Instrument,
                hasBreachedRuleByPositionSize,
                cancellationRatioByPositionSize,
                cancelledPositionOrders,
                totalPositionOrders,
                hasBreachedRuleByOrderCount,
                cancellationRatioByOrderCount);
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Universe Genesis occurred");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading Opened for exchange {exchange.MarketId}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading closed for exchange {exchange.MarketId}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Universe Eschaton occurred");
            _opCtx?.EndEvent();
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (CancelledOrderRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (CancelledOrderRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
