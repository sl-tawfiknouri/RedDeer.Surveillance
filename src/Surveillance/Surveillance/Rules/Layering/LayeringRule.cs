using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Markets;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Rules.Layering
{
    public class LayeringRule : BaseUniverseRule, ILayeringRule
    {
        private readonly ILogger _logger;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly ILayeringRuleParameters _parameters;
        private bool _hadMissingData = false;

        public LayeringRule(
            ILayeringRuleParameters parameters,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            ILogger logger,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursManager tradingHoursManager,
            ISystemProcessOperationRunRuleContext opCtx,
            RuleRunMode runMode,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(20),
                DomainV2.Scheduling.Rules.Layering,
                LayeringRuleFactory.Version,
                "Layering Rule",
                opCtx,
                factory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            if (tradeWindow.All(trades => trades.OrderDirection == tradeWindow.First().OrderDirection))
            {
                return;
            }

            var mostRecentTrade = tradeWindow.Pop();

            if (mostRecentTrade.OrderStatus() != OrderStatus.Filled)
            {
                return;
            }

            var buyPosition = new TradePosition(new List<Order>());
            var sellPosition = new TradePosition(new List<Order>());

            AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            var tradingPosition =
                (mostRecentTrade.OrderDirection == OrderDirections.BUY)
                    ? buyPosition
                    : sellPosition;

            var opposingPosition =
                mostRecentTrade.OrderDirection == OrderDirections.SELL
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
                _logger.LogInformation($"LayeringRule RunInitialSubmissionRule had a breach for {mostRecentTrade?.Instrument?.Identifiers}. Passing to alert stream.");
                var universeAlert = new UniverseAlertEvent(DomainV2.Scheduling.Rules.Layering, layeringRuleBreach, _ruleCtx);
                _alertStream.Add(universeAlert);
            }
        }

        private void AddToPositions(ITradePosition buyPosition, ITradePosition sellPosition, Order nextTrade)
        {
            switch (nextTrade.OrderDirection)
            {
                case OrderDirections.BUY:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderDirections.SELL:
                    sellPosition.Add(nextTrade);
                    break;
                default:
                    _logger.LogError("Layering rule not considering an out of range order direction");
                    _ruleCtx.EventException("Layering rule not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
        }

        private ILayeringRuleBreach CheckPositionForLayering(
            Stack<Order> tradeWindow,
            ITradePosition buyPosition,
            ITradePosition sellPosition,
            ITradePosition tradingPosition,
            ITradePosition opposingPosition,
            Order mostRecentTrade)
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

            var allTradesInPositions = opposingPosition.Get().Concat(tradingPosition.Get()).ToList();
            var allTrades = new TradePosition(allTradesInPositions);

            return (HasRuleBreach(hasBidirectionalBreach, hasDailyVolumeBreach, hasWindowVolumeBreach, priceMovementBreach))
                ? new LayeringRuleBreach(
                    _parameters,
                    _parameters.WindowSize,
                    allTrades,
                    mostRecentTrade.Instrument,
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
            Order mostRecentTrade)
        {
            var tradingHoursManager = _tradingHoursManager.Get(mostRecentTrade.Market.MarketIdentifierCode);

            if (!tradingHoursManager.IsValid)
            {
                _logger.LogInformation($"Layering unable to fetch market data for ({mostRecentTrade.Market.MarketIdentifierCode}) for the most recent trade {mostRecentTrade?.Instrument?.Identifiers} the market data did not contain the security indicated as trading in that market");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var marketRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHoursManager.OpeningInUtcForDay(UniverseDateTime),
                    tradingHoursManager.ClosingInUtcForDay(UniverseDateTime),
                    _ruleCtx?.Id());

            var marketResult = UniverseEquityIntradayCache.Get(marketRequest);
            if (marketResult.HadMissingData)
            {
                _logger.LogInformation($"Layering unable to fetch market data for ({mostRecentTrade.Market.MarketIdentifierCode}) for the most recent trade {mostRecentTrade?.Instrument?.Identifiers} the market data did not contain the security indicated as trading in that market");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var marketSecurityData = marketResult.Response;
            if (marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded <= 0
                || opposingPosition.TotalVolumeOrderedOrFilled() <= 0)
            {
                _logger.LogInformation($"Layering unable to evaluate for {mostRecentTrade?.Instrument?.Identifiers} either the market daily volume data was not available or the opposing position had a bad total volume value (daily volume){marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded} - (opposing position){opposingPosition.TotalVolumeOrderedOrFilled()}");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var percentageDailyVolume = (decimal)opposingPosition.TotalVolumeOrderedOrFilled() / (decimal)marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded;
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
            Order mostRecentTrade)
        {
            var marketDataRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    UniverseDateTime.Subtract(WindowSize),
                    UniverseDateTime,
                    _ruleCtx?.Id());

            var securityResult = UniverseEquityIntradayCache.GetMarkets(marketDataRequest);
            if (securityResult.HadMissingData)
            {
                _logger.LogWarning($"Layering unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var windowVolume = securityResult.Response.Sum(sdt => sdt?.SpreadTimeBar.Volume.Traded);
            if (windowVolume <= 0)
            {
                _logger.LogInformation($"Layering unable to sum meaningful volume from market data frames for volume window in {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            if (opposingPosition.TotalVolumeOrderedOrFilled() <= 0)
            {
                _logger.LogInformation($"Layering unable to calculate opposing position volume window in {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var percentageWindowVolume = (decimal)opposingPosition.TotalVolumeOrderedOrFilled() / (decimal)windowVolume;
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
            Order mostRecentTrade)
        {
            var startDate = opposingPosition.Get().Where(op => op.PlacedDate != null).Min(op => op.PlacedDate).GetValueOrDefault();
            var endDate = opposingPosition.Get().Where(op => op.PlacedDate != null).Max(op => op.PlacedDate).GetValueOrDefault();

            if (endDate.Subtract(startDate) < TimeSpan.FromMinutes(1))
            {
                endDate = endDate.AddMinutes(1);
            }

            var marketRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    startDate,
                    endDate,
                    _ruleCtx?.Id());

            var marketResult = UniverseEquityIntradayCache.GetMarkets(marketRequest);

            if (marketResult.HadMissingData)
            {
                _logger.LogInformation($"Layering unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }
            
            if (mostRecentTrade.PlacedDate > endDate)
            {
                endDate = mostRecentTrade.PlacedDate.GetValueOrDefault();
            }

            var securityDataTicks = marketResult.Response;
            var startTick = StartTick(securityDataTicks, startDate);
            if (startTick == null)
            {
                _logger.LogInformation($"Layering unable to fetch starting exchange tick data for ({startDate}) {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var endTick = EndTick(securityDataTicks, endDate);
            if (endTick == null)
            {
                _logger.LogInformation($"Layering unable to fetch ending exchange tick data for ({endDate}) {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                _hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var priceMovement = endTick.SpreadTimeBar.Price.Value - startTick.SpreadTimeBar.Price.Value;

            return BuildDescription(mostRecentTrade, priceMovement, startTick, endTick);
        }

        private RuleBreachDescription BuildDescription(
            Order mostRecentTrade,
            decimal priceMovement,
            EquityInstrumentIntraDayTimeBar startTick,
            EquityInstrumentIntraDayTimeBar endTick)
        {
            switch (mostRecentTrade.OrderDirection)
            {
                case OrderDirections.BUY:
                    return priceMovement < 0
                        ? new RuleBreachDescription { RuleBreached = true, Description = $" Prices in {mostRecentTrade.Instrument.Name} moved from ({endTick.SpreadTimeBar.Price.Currency}) {endTick.SpreadTimeBar.Price.Value} to ({startTick.SpreadTimeBar.Price.Currency}) {startTick.SpreadTimeBar.Price.Value} for a net change of {startTick.SpreadTimeBar.Price.Currency} {priceMovement} in line with the layering price pressure influence." }
                        : RuleBreachDescription.False();
                case OrderDirections.SELL:
                    return priceMovement > 0
                        ? new RuleBreachDescription { RuleBreached = true, Description = $" Prices in {mostRecentTrade.Instrument.Name} moved from ({endTick.SpreadTimeBar.Price.Currency}) {endTick.SpreadTimeBar.Price.Value} to ({startTick.SpreadTimeBar.Price.Currency}) {startTick.SpreadTimeBar.Price.Value} for a net change of {startTick.SpreadTimeBar.Price.Currency} {priceMovement} in line with the layering price pressure influence." } : RuleBreachDescription.False();
                default:
                    _logger.LogError($"Layering rule is not taking into account a new order position value (handles buy/sell) {mostRecentTrade.OrderDirection} (Arg Out of Range)");
                    _ruleCtx.EventException($"Layering rule is not taking into account a new order position value (handles buy/sell) {mostRecentTrade.OrderDirection} (Arg Out of Range)");
                    return RuleBreachDescription.False();
            }
        }

        private EquityInstrumentIntraDayTimeBar StartTick(List<EquityInstrumentIntraDayTimeBar> securityDataTicks, DateTime startDate)
        {
            if (securityDataTicks == null
                || !securityDataTicks.Any())
            {
                return null;
            }

            EquityInstrumentIntraDayTimeBar startTick;
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

        private EquityInstrumentIntraDayTimeBar EndTick(List<EquityInstrumentIntraDayTimeBar> securityDataTicks, DateTime endDate)
        {
            if (securityDataTicks == null
                || !securityDataTicks.Any())
            {
                return null;
            }

            EquityInstrumentIntraDayTimeBar endTick;
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

            var universeAlert = new UniverseAlertEvent(DomainV2.Scheduling.Rules.Layering, null, _ruleCtx, true);
            _alertStream.Add(universeAlert);

            if (_hadMissingData)
            {
                _logger.LogInformation($"LayeringRule had missing data. Updating rule context with state.");
                _ruleCtx.EndEvent().EndEventWithMissingDataError();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }
        }

        public object Clone()
        {
            var clone = (LayeringRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
