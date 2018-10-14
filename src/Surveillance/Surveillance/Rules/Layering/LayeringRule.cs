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
        private bool _hadMissingData = false;

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

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
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
                    opposingPosition,
                    mostRecentTrade);

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
            ITradePosition opposingPosition,
            TradeOrderFrame mostRecentTrade)
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

                if (!tradingPosition.Get().Any()
                    || !opposingPosition.Get().Any())
                {
                    continue;
                }

                // IF ALL PARAMETERS ARE NULL JUST DO THE BIDIRECTIONAL TRADE CHECK
                if (_parameters.PercentageOfMarketDailyVolume == null)
                {
                    return true;
                }

                if (_parameters.PercentageOfMarketDailyVolume != null
                    && CheckDailyVolumeBreach(opposingPosition, mostRecentTrade))
                {
                    hasBreachedLayeringRule = true;
                }
            }

            return hasBreachedLayeringRule;
        }

        private bool CheckDailyVolumeBreach(
            ITradePosition opposingPosition,
            TradeOrderFrame mostRecentTrade)
        {
            var marketId = mostRecentTrade.Market.Id;

            if (marketId == null)
            {
                _logger.LogInformation($"Layering unable to evaluate the market id for the most recent trade {mostRecentTrade?.Security?.Identifiers}");

                _hadMissingData = true;
                return false;
            }

            if (!LatestExchangeFrameBook.ContainsKey(marketId))
            {
                _logger.LogInformation($"Layering unable to fetch market data for ({marketId}) for the most recent trade {mostRecentTrade?.Security?.Identifiers}");

                _hadMissingData = true;
                return false;
            }

            LatestExchangeFrameBook.TryGetValue(marketId, out var frame);

            if (frame == null)
            {
                _logger.LogInformation($"Layering unable to fetch market data for ({marketId}) for the most recent trade {mostRecentTrade?.Security?.Identifiers} the frame was null");

                _hadMissingData = true;
                return false;
            }

            var marketSecurityData = frame
                .Securities
                ?.FirstOrDefault(sec => Equals(sec.Security?.Identifiers, mostRecentTrade.Security.Identifiers));

            if (marketSecurityData == null)
            {
                _logger.LogInformation($"Layering unable to fetch market data for ({marketId}) for the most recent trade {mostRecentTrade?.Security?.Identifiers} the market data did not contain the security indicated as trading in that market");

                _hadMissingData = true;
                return false;
            }

            if (marketSecurityData?.DailyVolume.Traded <= 0
                || opposingPosition.TotalVolume() <= 0)
            {
                _logger.LogInformation($"Layering unable to evaluate for {mostRecentTrade?.Security?.Identifiers} either the market daily volume data was not available or the opposing position had a bad total volume value (daily volume){marketSecurityData?.DailyVolume.Traded} - (opposing position){opposingPosition.TotalVolume()}");

                _hadMissingData = true;
                return false;
            }

            var percentageDailyVolume = (decimal)opposingPosition.TotalVolume() / (decimal)marketSecurityData.DailyVolume.Traded;

            if (percentageDailyVolume >= _parameters.PercentageOfMarketDailyVolume)
            {
                return true;
            }

            return false;
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            // we don't analyse rules based on when their status last changed in the layering rule
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

            if (_hadMissingData)
            {
                _ruleCtx.EndEvent().EndEventWithMissingDataError();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }

            _alertCount = 0;
        }
    }
}
