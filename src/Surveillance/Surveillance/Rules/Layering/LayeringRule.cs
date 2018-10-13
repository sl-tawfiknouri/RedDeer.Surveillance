using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.Layering
{
    public class LayeringRule : BaseUniverseRule, ILayeringRule
    {
        private readonly ILogger _logger;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly ILayeringRuleParameters _parameters;
        private int _alertCount = 0;

        public LayeringRule(
            ILayeringRuleParameters parameters,
            ILogger logger,
            ISystemProcessOperationRunRuleContext opCtx)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(20),
                Domain.Scheduling.Rules.Layering,
                Versioner.Version(1, 0),
                "Layering Rule",
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
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

            if (mostRecentTrade.OrderStatus != OrderStatus.Fulfilled)
            {
                // we need to start from a fulfilled order   ayyy lmao so true..we need to change this to filled at some point lol
                return;
            }

            var buyPosition = new TradePosition(new List<TradeOrderFrame>());
            var sellPosition = new TradePosition(new List<TradeOrderFrame>());

            AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            var tradingPosition =
                mostRecentTrade.Position == OrderPosition.Buy
                    ? buyPosition
                    : sellPosition;

            var opposingPosition =
                mostRecentTrade.Position == OrderPosition.Sell
                    ? buyPosition
                    : sellPosition;

            var hasBreachedLayeringRule =
                CheckPositionForLayering(
                    tradeWindow,
                    buyPosition,
                    sellPosition,
                    tradingPosition,
                    opposingPosition);

            if (hasBreachedLayeringRule)
            {
                _alertCount += 1;
            }
        }

        private void AddToPositions(ITradePosition buyPosition, ITradePosition sellPosition, TradeOrderFrame nextTrade)
        {
            switch (nextTrade.Position)
            {
                case OrderPosition.Buy:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderPosition.Sell:
                    sellPosition.Add(nextTrade);
                    break;
                default:
                    _logger.LogError("Layering rule not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
        }

        private bool CheckPositionForLayering(
            Stack<TradeOrderFrame> tradeWindow,
            ITradePosition buyPosition,
            ITradePosition sellPosition,
            ITradePosition tradingPosition,
            ITradePosition opposingPosition)
        {
            var hasBreachedLayeringRule = false;
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
                AddToPositions(buyPosition, sellPosition, nextTrade);

                if (tradingPosition.Get().Any()
                    && opposingPosition.Get().Any())
                {
                    hasBreachedLayeringRule = true;
                }
            }

            return hasBreachedLayeringRule;
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred in the Layering Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred in the Layering Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred in Layering Rule at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured in Layering Rule");
            _ruleCtx.UpdateAlertEvent(_alertCount);
            _ruleCtx?.EndEvent();
            _alertCount = 0;
        }
    }
}
