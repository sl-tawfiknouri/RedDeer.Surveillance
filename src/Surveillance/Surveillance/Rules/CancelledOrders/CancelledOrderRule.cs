using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.CancelledOrders
{
    /// <summary>
    /// Cancelled Orders Rule
    /// Ignores rule run mode as it doesn't use market data
    /// </summary>
    public class CancelledOrderRule : BaseUniverseRule, ICancelledOrderRule
    {
        private readonly ICancelledOrderRuleParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _opCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;

        private readonly ILogger _logger;
        
        public CancelledOrderRule(
            ICancelledOrderRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            ILogger<CancelledOrderRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(60),
                DomainV2.Scheduling.Rules.CancelledOrders,
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
                _logger.LogInformation($"CancelledOrderRule RunRule has breached parameter conditions for {mostRecentTrade?.Instrument?.Identifiers}. Adding message to alert stream.");
                var message = new UniverseAlertEvent(DomainV2.Scheduling.Rules.CancelledOrders, ruleBreach, _opCtx);
                _alertStream.Add(message);
            }
            else
            {
                _logger.LogInformation($"CancelledOrderRule RunRule did not breach parameter conditions for {mostRecentTrade?.Instrument?.Identifiers}.");
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
                if (nextTrade.OrderDirection != mostRecentTrade.OrderDirection)
                {
                    continue;
                }

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
            _logger.LogInformation("Universe Genesis occurred in the Cancelled Order Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading Opened for exchange {exchange.MarketId} in the Cancelled Order Rule");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading closed for exchange {exchange.MarketId} in the Cancelled Order Rule.");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Universe Eschaton occurred in the Cancelled Order Rule");
            _opCtx?.EndEvent();
        }

        public object Clone()
        {
            var clone = (CancelledOrderRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
