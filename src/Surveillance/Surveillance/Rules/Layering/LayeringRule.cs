using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.Frames;
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
        private readonly ILayeringCachedMessageSender _messageSender;
        private readonly ILayeringRuleParameters _parameters;
        private int _alertCount = 0;
        private bool _hadMissingData = false;

        public LayeringRule(
            ILayeringRuleParameters parameters,
            ILayeringCachedMessageSender messageSender,
            ILogger logger,
            ISystemProcessOperationRunRuleContext opCtx)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(20),
                Domain.Scheduling.Rules.Layering,
                Versioner.Version(1, 0),
                "Layering Rule",
                opCtx,
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            _messageSender = messageSender;
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

            var layeringRuleBreach =
                CheckPositionForLayering(
                    tradeWindow,
                    buyPosition,
                    sellPosition,
                    tradingPosition,
                    opposingPosition,
                    mostRecentTrade);

            if (layeringRuleBreach != null)
            {
                _messageSender.Send(layeringRuleBreach);
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
                    _ruleCtx.EventException("Layering rule not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
        }

        private ILayeringRuleBreach CheckPositionForLayering(
            Stack<TradeOrderFrame> tradeWindow,
            ITradePosition buyPosition,
            ITradePosition sellPosition,
            ITradePosition tradingPosition,
            ITradePosition opposingPosition,
            TradeOrderFrame mostRecentTrade)
        {
            var hasTradesInWindow = tradeWindow.Any();
            RuleBreachDescription hasBidirectionalBreach = RuleBreachDescription.False();
            RuleBreachDescription hasDailyVolumeBreach = RuleBreachDescription.False();
            RuleBreachDescription hasWindowVolumeBreach = RuleBreachDescription.False();
            RuleBreachDescription priceMovementBreach = RuleBreachDescription.False();

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
                if (_parameters.PercentageOfMarketDailyVolume == null
                    && _parameters.PercentageOfMarketWindowVolume == null
                    && _parameters.CheckForCorrespondingPriceMovement == null)
                {
                    hasBidirectionalBreach = new RuleBreachDescription
                    {
                        RuleBreached = true,
                        Description = " Trading in both buy/sell positions simultaneously was detected."
                    };
                }

                if (_parameters.PercentageOfMarketDailyVolume != null)
                {
                    hasDailyVolumeBreach = CheckDailyVolumeBreach(opposingPosition, mostRecentTrade);
                }

                if (_parameters.PercentageOfMarketWindowVolume != null)
                {
                    hasWindowVolumeBreach = CheckWindowVolumeBreach(opposingPosition, mostRecentTrade);
                }

                if (_parameters.CheckForCorrespondingPriceMovement != null
                    && _parameters.CheckForCorrespondingPriceMovement.Value)
                {
                    priceMovementBreach = CheckForPriceMovement(opposingPosition, mostRecentTrade);
                }
            }

            opposingPosition.Add(mostRecentTrade);

            return (HasRuleBreach(hasBidirectionalBreach, hasDailyVolumeBreach, hasWindowVolumeBreach, priceMovementBreach))
                ? new LayeringRuleBreach(
                    _parameters,
                    _parameters.WindowSize,
                    opposingPosition,
                    mostRecentTrade.Security,
                    hasBidirectionalBreach,
                    hasDailyVolumeBreach,
                    hasWindowVolumeBreach,
                    priceMovementBreach)
                : null;
        }

        private static bool HasRuleBreach(
            RuleBreachDescription hasBidirectionalBreach,
            RuleBreachDescription hasDailyVolumeBreach,
            RuleBreachDescription hasWindowVolumeBreach,
            RuleBreachDescription priceMovementBreach)
        {
            return (hasBidirectionalBreach?.RuleBreached ?? false)
                || (hasDailyVolumeBreach?.RuleBreached ?? false)
                || (hasWindowVolumeBreach?.RuleBreached ?? false)
                || (priceMovementBreach?.RuleBreached ?? false);
        }

        private RuleBreachDescription CheckDailyVolumeBreach(
            ITradePosition opposingPosition,
            TradeOrderFrame mostRecentTrade)
        {
            var marketId = mostRecentTrade.Market.Id;

            if (marketId == null)
            {
                _logger.LogInformation($"Layering unable to evaluate the market id for the most recent trade {mostRecentTrade?.Security?.Identifiers}");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            if (!LatestExchangeFrameBook.ContainsKey(marketId))
            {
                _logger.LogInformation($"Layering unable to fetch market data for ({marketId}) for the most recent trade {mostRecentTrade?.Security?.Identifiers}");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            LatestExchangeFrameBook.TryGetValue(marketId, out var frame);

            if (frame == null)
            {
                _logger.LogInformation($"Layering unable to fetch market data for ({marketId}) for the most recent trade {mostRecentTrade?.Security?.Identifiers} the frame was null");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var marketSecurityData = frame
                .Securities
                ?.FirstOrDefault(sec => Equals(sec.Security?.Identifiers, mostRecentTrade.Security.Identifiers));

            if (marketSecurityData == null)
            {
                _logger.LogInformation($"Layering unable to fetch market data for ({marketId}) for the most recent trade {mostRecentTrade?.Security?.Identifiers} the market data did not contain the security indicated as trading in that market");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            if (marketSecurityData?.DailyVolume.Traded <= 0
                || opposingPosition.TotalVolume() <= 0)
            {
                _logger.LogInformation($"Layering unable to evaluate for {mostRecentTrade?.Security?.Identifiers} either the market daily volume data was not available or the opposing position had a bad total volume value (daily volume){marketSecurityData?.DailyVolume.Traded} - (opposing position){opposingPosition.TotalVolume()}");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var percentageDailyVolume = (decimal)opposingPosition.TotalVolume() / (decimal)marketSecurityData.DailyVolume.Traded;
            if (percentageDailyVolume >= _parameters.PercentageOfMarketDailyVolume)
            {
                return new RuleBreachDescription
                {
                    RuleBreached = true,
                    Description = $" Percentage of market daily volume traded within a {_parameters.WindowSize.TotalSeconds} second window exceeded the layering window threshold of {_parameters.PercentageOfMarketDailyVolume * 100}%."
                };
            }

            return RuleBreachDescription.False();
        }

        private RuleBreachDescription CheckWindowVolumeBreach(
            ITradePosition opposingPosition,
            TradeOrderFrame mostRecentTrade)
        {
            if (!MarketHistory.TryGetValue(mostRecentTrade.Market.Id, out var marketStack))
            {
                _logger.LogInformation($"Layering unable to fetch market data frames for {mostRecentTrade.Market.Id} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var securityDataTicks = marketStack
                .ActiveMarketHistory()
                .Where(amh => amh != null)
                .Select(amh =>
                    amh.Securities?.FirstOrDefault(sec =>
                        Equals(sec.Security.Identifiers, mostRecentTrade.Security.Identifiers)))
                .Where(sec => sec != null)
                .ToList();

            var windowVolume = securityDataTicks.Sum(sdt => sdt.Volume.Traded);

            if (windowVolume <= 0)
            {
                _logger.LogInformation($"Layering unable to sum meaningful volume from market data frames for volume window in {mostRecentTrade.Market.Id} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            if (opposingPosition.TotalVolume() <= 0)
            {
                _logger.LogInformation($"Layering unable to calculate opposing position volume window in {mostRecentTrade.Market.Id} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var percentageWindowVolume = (decimal)opposingPosition.TotalVolume() / (decimal)windowVolume;
            if (percentageWindowVolume >= _parameters.PercentageOfMarketWindowVolume)
            {
                return new RuleBreachDescription
                {
                    RuleBreached = true,
                    Description = $" Percentage of market volume traded within a {_parameters.WindowSize.TotalSeconds} second window exceeded the layering window threshold of {_parameters.PercentageOfMarketWindowVolume * 100}%."
                };
            }

            return RuleBreachDescription.False();
        }

        private RuleBreachDescription CheckForPriceMovement(
            ITradePosition opposingPosition,
            TradeOrderFrame mostRecentTrade)
        {
            if (!MarketHistory.TryGetValue(mostRecentTrade.Market.Id, out var marketStack))
            {
                _logger.LogInformation($"Layering unable to fetch market data frames for {mostRecentTrade.Market.Id} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var securityDataTicks = marketStack
                .ActiveMarketHistory()
                .Where(amh => amh != null)
                .Select(amh =>
                    amh.Securities?.FirstOrDefault(sec =>
                        Equals(sec.Security.Identifiers, mostRecentTrade.Security.Identifiers)))
                .Where(sec => sec != null)
                .ToList();

            if (!securityDataTicks.Any())
            {
                _logger.LogInformation($"Layering unable to fetch market data frames for {mostRecentTrade.Market.Id} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var startDate = opposingPosition.Get().Min(op => op.TradeSubmittedOn);
            var endDate = opposingPosition.Get().Max(op => op.TradeSubmittedOn);

            if (mostRecentTrade.TradeSubmittedOn > endDate)
            {
                endDate = mostRecentTrade.TradeSubmittedOn;
            }

            var startTick = StartTick(securityDataTicks, startDate);
            if (startTick == null)
            {
                _logger.LogInformation($"Layering unable to fetch starting exchange tick data for ({startDate}) {mostRecentTrade.Market.Id} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var endTick = EndTick(securityDataTicks, endDate);
            if (endTick == null)
            {
                _logger.LogInformation($"Layering unable to fetch ending exchange tick data for ({endDate}) {mostRecentTrade.Market.Id} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }
            
            var priceMovement = endTick.Spread.Price.Value - startTick.Spread.Price.Value;
            switch (mostRecentTrade.Position)
            {
                case OrderPosition.Buy:
                    return priceMovement < 0
                        ? new RuleBreachDescription { RuleBreached = true, Description = $" Prices in {mostRecentTrade.Security.Name} moved from ({endTick.Spread.Price.Currency}) {endTick.Spread.Price.Value} to ({startTick.Spread.Price.Currency}) {startTick.Spread.Price.Value} for a net change of {startTick.Spread.Price.Currency} {priceMovement} in line with the layering price pressure influence." }
                        : RuleBreachDescription.False();
                case OrderPosition.Sell:
                    return priceMovement > 0
                        ? new RuleBreachDescription { RuleBreached = true, Description = $" Prices in {mostRecentTrade.Security.Name} moved from ({endTick.Spread.Price.Currency}) {endTick.Spread.Price.Value} to ({startTick.Spread.Price.Currency}) {startTick.Spread.Price.Value} for a net change of {startTick.Spread.Price.Currency} {priceMovement} in line with the layering price pressure influence." } : RuleBreachDescription.False();
                default:
                    _logger.LogError($"Layering rule is not taking into account a new order position value (handles buy/sell) {mostRecentTrade.Position} (Arg Out of Range)");
                    _ruleCtx.EventException($"Layering rule is not taking into account a new order position value (handles buy/sell) {mostRecentTrade.Position} (Arg Out of Range)");
                    break;
            }

            return RuleBreachDescription.False();
        }

        private SecurityTick StartTick(List<SecurityTick> securityDataTicks, DateTime startDate)
        {
            if (securityDataTicks == null
                || !securityDataTicks.Any())
            {
                return null;
            }

            SecurityTick startTick;
            if (securityDataTicks.Any(sdt => sdt.TimeStamp < startDate))
            {
                startTick =
                    securityDataTicks
                        .Where(sdt => sdt.TimeStamp < startDate)
                        .OrderBy(sdt => sdt.TimeStamp)
                        .Reverse().FirstOrDefault();
            }
            else
            {
                startTick =
                    securityDataTicks
                        .OrderBy(sdt => sdt.TimeStamp)
                        .FirstOrDefault();
            }

            return startTick;
        }

        private SecurityTick EndTick(List<SecurityTick> securityDataTicks, DateTime endDate)
        {
            if (securityDataTicks == null
                || !securityDataTicks.Any())
            {
                return null;
            }

            SecurityTick endTick;
            if (securityDataTicks.Any(sdt => sdt.TimeStamp > endDate))
            {
                endTick =
                    securityDataTicks
                        .Where(sdt => sdt.TimeStamp > endDate)
                        .OrderBy(sdt => sdt.TimeStamp)
                        .FirstOrDefault();
            }
            else
            {
                endTick =
                    securityDataTicks
                        .OrderBy(sdt => sdt.TimeStamp)
                        .Reverse()
                        .FirstOrDefault();
            }

            return endTick;
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
            _messageSender.Flush(_ruleCtx);

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
