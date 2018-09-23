using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders
{
    /// <summary>
    /// This rule looks at unusual order cancellation from several angles.
    /// We consider rules that are cancelled by value i.e. 1 order over 1 million gbp
    /// Order cancellation ratios by % of trade orders that were cancelled
    /// Order cancellation ratios by % of position volume that was cancelled
    /// </summary>
    public class CancelledOrderRule : BaseTradeRule, ICancelledOrderRule
    {
        private readonly ICancelledOrderPositionDeDuplicator _deduplicatingMessageSender;
        private readonly ICancelledOrderRuleParameters _parameters;
        private readonly ILogger _logger;

        public CancelledOrderRule(
            ICancelledOrderPositionDeDuplicator deduplicatingMessageSender,
            ICancelledOrderRuleParameters parameters,
            ILogger<CancelledOrderRule> logger) 
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(30),
                Domain.Scheduling.Rules.CancelledOrders,
                "V1.0",
                logger)
        {
            _deduplicatingMessageSender =
                deduplicatingMessageSender
                ?? throw new ArgumentNullException(nameof(deduplicatingMessageSender));

            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
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
                var message =
                    new CancelledOrderMessageSenderParameters(mostRecentTrade.Security.Identifiers)
                    {
                        TradePosition = tradingPosition,
                        Parameters = _parameters,
                        RuleBreach = ruleBreach
                    };

                _deduplicatingMessageSender.Send(message);
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
    }
}