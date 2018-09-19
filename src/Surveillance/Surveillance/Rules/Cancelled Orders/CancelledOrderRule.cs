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
        private readonly ICancelledOrderRuleParameters _parameters;
        private readonly ILogger _logger;

        public CancelledOrderRule(
            ICancelledOrderRuleParameters parameters,
            Domain.Scheduling.Rules rule,
            string version,
            ILogger<CancelledOrderRule> logger) 
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(30),
                rule,
                version,
                logger)
        {
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

            if (tradeWindow.All(trades => trades.Position == tradeWindow.First().Position))
            {
                return;
            }

            var mostRecentTrade = tradeWindow.Pop();

            var tradingPosition =
                new TradePosition(
                    new List<TradeOrderFrame>(),
                    _parameters.CancelledOrderCountPercentageThreshold,
                    _parameters.CancelledOrderPercentagePositionThreshold,
                    _logger);

            tradingPosition.Add(mostRecentTrade);
            var hasBreachedRule = CheckPositionForCancellations(tradeWindow, mostRecentTrade, tradingPosition);

            if (hasBreachedRule)
            {
                RecordRuleBreach();
            }
        }

        private bool CheckPositionForCancellations(
            Stack<TradeOrderFrame> tradeWindow,
            TradeOrderFrame mostRecentTrade,
            TradePosition tradingPosition)
        {
            var hasBreachedRule = false;
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

                if (_parameters.MinimumNumberOfTradesToApplyRuleTo > tradingPosition.Get().Count
                    || _parameters.MaximumNumberOfTradesToApplyRuleTo < tradingPosition.Get().Count)
                {
                    continue;
                }

                if (_parameters.CancelledOrderCountPercentageThreshold != null
                    && tradingPosition.HighCancellationRatioByTradeCount())
                {
                    hasBreachedRule = true;
                }

                if (_parameters.CancelledOrderPercentagePositionThreshold != null
                    && tradingPosition.HighCancellationRatioByPositionSize())
                {
                    hasBreachedRule = true;
                }
            }

            return hasBreachedRule;
        }

        private void RecordRuleBreach()
        {

        }
    }
}