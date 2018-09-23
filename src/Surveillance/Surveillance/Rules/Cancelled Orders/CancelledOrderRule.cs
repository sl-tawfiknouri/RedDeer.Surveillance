using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Stub;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders
{
    public class CancelledOrderRule : BaseUniverseRule, ICancelledOrderRule
    {
        private readonly ICancelledOrderRuleParameters _parameters;
        private readonly ICancelledOrderRuleCachedMessageSender _cachedMessageSender;

        private readonly ILogger _logger;
        
        public CancelledOrderRule(
            ICancelledOrderRuleParameters parameters,
            ICancelledOrderRuleCachedMessageSender cachedMessageSender,
            ILogger<CancelledOrderRule> logger)
            : base(parameters?.WindowSize ?? TimeSpan.FromMinutes(60), Domain.Scheduling.Rules.CancelledOrders, Versioner.Version(2, 0), "Cancelled Order Rule(2)", logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _cachedMessageSender = cachedMessageSender ?? throw new ArgumentNullException(nameof(cachedMessageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                    new List<TradeOrderFrame>(),
                    _parameters.CancelledOrderPercentagePositionThreshold,
                    _parameters.CancelledOrderCountPercentageThreshold,
                    _logger);

            tradingPosition.Add(mostRecentTrade);
            var ruleBreach = CheckPositionForCancellations(tradeWindow, mostRecentTrade, tradingPosition);

            if (ruleBreach.HasBreachedRule())
            {
                _cachedMessageSender.Send(ruleBreach);
            }
        }

        private ICancelledOrderRuleBreach CheckPositionForCancellations(
            Stack<TradeOrderFrame> tradeWindow,
            TradeOrderFrame mostRecentTrade,
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
                if (nextTrade.Position != mostRecentTrade.Position)
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

            return new CancelledOrderRuleBreach(
                _parameters,
                tradingPosition,
                tradingPosition?.Get()?.FirstOrDefault()?.Security, hasBreachedRuleByPositionSize,
                cancellationRatioByPositionSize,
                hasBreachedRuleByOrderCount,
                cancellationRatioByOrderCount);
        }

        protected override void Genesis()
        {
            _logger.LogDebug("Universe Genesis occurred in the Cancelled Order Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Trading Opened for exchange {exchange.MarketId} in the Cancelled Order Rule");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Trading closed for exchange {exchange.MarketId} in the Cancelled Order Rule.");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogDebug("Universe Eschaton occurred in the Cancelled Order Rule");

            _cachedMessageSender.Flush();
        }
    }
}
