namespace Surveillance.Engine.Rules.Rules.Equity.Layering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class LayeringRule : BaseUniverseRule, ILayeringRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly ILayeringRuleEquitiesParameters _equitiesParameters;

        private readonly ILogger _logger;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private bool _hadMissingData;

        public LayeringRule(
            ILayeringRuleEquitiesParameters equitiesParameters,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            ILogger logger,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketTradingHoursService tradingHoursService,
            ISystemProcessOperationRunRuleContext opCtx,
            RuleRunMode runMode,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(20),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.Layering,
                EquityRuleLayeringFactory.Version,
                "Layering Rule",
                opCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this._equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._ruleCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (LayeringRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (LayeringRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // we don't analyse rules based on fills in the layering rule
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation("Eschaton occured");

            var universeAlert = new UniverseAlertEvent(Rules.Layering, null, this._ruleCtx, true);
            this._alertStream.Add(universeAlert);

            if (this._hadMissingData)
            {
                this._logger.LogInformation("had missing data. Updating rule context with state.");
                this._ruleCtx.EndEvent();
            }
            else
            {
                this._ruleCtx?.EndEvent();
            }
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation("Genesis occurred");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred {exchange?.MarketClose}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null || !tradeWindow.Any()) return;

            if (tradeWindow.All(trades => trades.OrderDirection == tradeWindow.First().OrderDirection)) return;

            var mostRecentTrade = tradeWindow.Pop();

            if (mostRecentTrade.OrderStatus() != OrderStatus.Filled) return;

            var buyPosition = new TradePosition(new List<Order>());
            var sellPosition = new TradePosition(new List<Order>());

            this.AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            var tradingPosition =
                mostRecentTrade.OrderDirection == OrderDirections.BUY
                || mostRecentTrade.OrderDirection == OrderDirections.COVER
                    ? buyPosition
                    : sellPosition;

            var opposingPosition =
                mostRecentTrade.OrderDirection == OrderDirections.SELL
                || mostRecentTrade.OrderDirection == OrderDirections.SHORT
                    ? buyPosition
                    : sellPosition;

            var layeringRuleBreach = this.CheckPositionForLayering(
                tradeWindow,
                buyPosition,
                sellPosition,
                tradingPosition,
                opposingPosition,
                mostRecentTrade);

            if (layeringRuleBreach != null)
            {
                this._logger.LogInformation(
                    $"RunInitialSubmissionRule had a breach for {mostRecentTrade?.Instrument?.Identifiers}. Passing to alert stream.");
                var universeAlert = new UniverseAlertEvent(Rules.Layering, layeringRuleBreach, this._ruleCtx);
                this._alertStream.Add(universeAlert);
            }
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            // we don't analyse rules based on when their status last changed in the layering rule
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private static bool HasRuleBreach(
            RuleBreachDescription hasBidirectionalBreach,
            RuleBreachDescription hasDailyVolumeBreach,
            RuleBreachDescription hasWindowVolumeBreach,
            RuleBreachDescription priceMovementBreach)
        {
            return (hasBidirectionalBreach?.RuleBreached ?? false) || (hasDailyVolumeBreach?.RuleBreached ?? false)
                                                                   || (hasWindowVolumeBreach?.RuleBreached ?? false)
                                                                   || (priceMovementBreach?.RuleBreached ?? false);
        }

        private void AddToPositions(ITradePosition buyPosition, ITradePosition sellPosition, Order nextTrade)
        {
            switch (nextTrade.OrderDirection)
            {
                case OrderDirections.BUY:
                case OrderDirections.COVER:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderDirections.SELL:
                case OrderDirections.SHORT:
                    sellPosition.Add(nextTrade);
                    break;
                default:
                    this._logger.LogError("not considering an out of range order direction");
                    this._ruleCtx.EventException("not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
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
                case OrderDirections.COVER:
                    return priceMovement < 0
                               ? new RuleBreachDescription
                                     {
                                         RuleBreached = true,
                                         Description =
                                             $" Prices in {mostRecentTrade.Instrument.Name} moved from ({endTick.SpreadTimeBar.Price.Currency}) {endTick.SpreadTimeBar.Price.Value} to ({startTick.SpreadTimeBar.Price.Currency}) {startTick.SpreadTimeBar.Price.Value} for a net change of {startTick.SpreadTimeBar.Price.Currency} {priceMovement} in line with the layering price pressure influence."
                                     }
                               : RuleBreachDescription.False();
                case OrderDirections.SELL:
                case OrderDirections.SHORT:
                    return priceMovement > 0
                               ? new RuleBreachDescription
                                     {
                                         RuleBreached = true,
                                         Description =
                                             $" Prices in {mostRecentTrade.Instrument.Name} moved from ({endTick.SpreadTimeBar.Price.Currency}) {endTick.SpreadTimeBar.Price.Value} to ({startTick.SpreadTimeBar.Price.Currency}) {startTick.SpreadTimeBar.Price.Value} for a net change of {startTick.SpreadTimeBar.Price.Currency} {priceMovement} in line with the layering price pressure influence."
                                     }
                               : RuleBreachDescription.False();
                default:
                    this._logger.LogError(
                        $"Layering rule is not taking into account a new order position value (handles buy/sell) {mostRecentTrade.OrderDirection} (Arg Out of Range)");
                    this._ruleCtx.EventException(
                        $"Layering rule is not taking into account a new order position value (handles buy/sell) {mostRecentTrade.OrderDirection} (Arg Out of Range)");
                    return RuleBreachDescription.False();
            }
        }

        private RuleBreachDescription CheckDailyVolumeBreach(ITradePosition opposingPosition, Order mostRecentTrade)
        {
            var tradingHoursManager =
                this._tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market.MarketIdentifierCode);

            if (!tradingHoursManager.IsValid)
            {
                this._logger.LogInformation(
                    $"unable to fetch market data for ({mostRecentTrade.Market.MarketIdentifierCode}) for the most recent trade {mostRecentTrade?.Instrument?.Identifiers} the market data did not contain the security indicated as trading in that market");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var marketRequest = new MarketDataRequest(
                mostRecentTrade.Market.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHoursManager.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHoursManager.ClosingInUtcForDay(this.UniverseDateTime),
                this._ruleCtx?.Id(),
                DataSource.AllInterday);

            var marketResult = this.UniverseEquityInterdayCache.Get(marketRequest);
            if (marketResult.HadMissingData)
            {
                this._logger.LogInformation(
                    $"unable to fetch market data for ({mostRecentTrade.Market.MarketIdentifierCode}) for the most recent trade {mostRecentTrade?.Instrument?.Identifiers} the market data did not contain the security indicated as trading in that market");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var marketSecurityData = marketResult.Response;
            if (marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded <= 0
                || opposingPosition.TotalVolumeOrderedOrFilled() <= 0)
            {
                this._logger.LogInformation(
                    $"unable to evaluate for {mostRecentTrade?.Instrument?.Identifiers} either the market daily volume data was not available or the opposing position had a bad total volume value (daily volume){marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded} - (opposing position){opposingPosition.TotalVolumeOrderedOrFilled()}");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var percentageDailyVolume = opposingPosition.TotalVolumeOrderedOrFilled()
                                        / (decimal)marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded;
            if (percentageDailyVolume >= this._equitiesParameters.PercentageOfMarketDailyVolume)
                return new RuleBreachDescription
                           {
                               RuleBreached = true,
                               Description =
                                   $" Percentage of market daily volume traded within a {this._equitiesParameters.Windows.BackwardWindowSize.TotalSeconds} second window exceeded the layering window threshold of {this._equitiesParameters.PercentageOfMarketDailyVolume * 100}%."
                           };

            return RuleBreachDescription.False();
        }

        private RuleBreachDescription CheckForPriceMovement(ITradePosition opposingPosition, Order mostRecentTrade)
        {
            var startDate = opposingPosition.Get().Where(op => op.PlacedDate != null).Min(op => op.PlacedDate)
                .GetValueOrDefault();
            var endDate = opposingPosition.Get().Where(op => op.PlacedDate != null).Max(op => op.PlacedDate)
                .GetValueOrDefault();

            if (endDate.Subtract(startDate) < TimeSpan.FromMinutes(1)) endDate = endDate.AddMinutes(1);

            var marketRequest = new MarketDataRequest(
                mostRecentTrade.Market.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                startDate.Subtract(this.BackwardWindowSize),
                endDate,
                this._ruleCtx?.Id(),
                DataSource.AllIntraday);

            var tradingDays = this._tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                this.UniverseDateTime.Subtract(this.BackwardWindowSize),
                this.UniverseDateTime,
                mostRecentTrade.Market.MarketIdentifierCode);

            var marketResult =
                this.UniverseEquityIntradayCache.GetMarketsForRange(marketRequest, tradingDays, this.RunMode);

            if (marketResult.HadMissingData)
            {
                this._logger.LogInformation(
                    $"unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {this.UniverseDateTime}.");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            if (mostRecentTrade.PlacedDate > endDate) endDate = mostRecentTrade.PlacedDate.GetValueOrDefault();

            var securityDataTicks = marketResult.Response;
            var startTick = this.StartTick(securityDataTicks, startDate);
            if (startTick == null)
            {
                this._logger.LogInformation(
                    $"unable to fetch starting exchange tick data for ({startDate}) {mostRecentTrade.Market.MarketIdentifierCode} at {this.UniverseDateTime}.");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var endTick = this.EndTick(securityDataTicks, endDate);
            if (endTick == null)
            {
                this._logger.LogInformation(
                    $"unable to fetch ending exchange tick data for ({endDate}) {mostRecentTrade.Market.MarketIdentifierCode} at {this.UniverseDateTime}.");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var priceMovement = endTick.SpreadTimeBar.Price.Value - startTick.SpreadTimeBar.Price.Value;

            return this.BuildDescription(mostRecentTrade, priceMovement, startTick, endTick);
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
            var hasBidirectionalBreach = RuleBreachDescription.False();
            var hasDailyVolumeBreach = RuleBreachDescription.False();
            var hasWindowVolumeBreach = RuleBreachDescription.False();
            var priceMovementBreach = RuleBreachDescription.False();

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
                this.AddToPositions(buyPosition, sellPosition, nextTrade);

                if (!tradingPosition.Get().Any() || !opposingPosition.Get().Any()) continue;

                // IF ALL PARAMETERS ARE NULL JUST DO THE BIDIRECTIONAL TRADE CHECK
                if (this._equitiesParameters.PercentageOfMarketDailyVolume == null
                    && this._equitiesParameters.PercentageOfMarketWindowVolume == null
                    && this._equitiesParameters.CheckForCorrespondingPriceMovement == null)
                    hasBidirectionalBreach = new RuleBreachDescription
                                                 {
                                                     RuleBreached = true,
                                                     Description =
                                                         " Trading in both buy/sell positions simultaneously was detected."
                                                 };

                if (this._equitiesParameters.PercentageOfMarketDailyVolume != null)
                    hasDailyVolumeBreach = this.CheckDailyVolumeBreach(opposingPosition, mostRecentTrade);

                if (this._equitiesParameters.PercentageOfMarketWindowVolume != null)
                    hasWindowVolumeBreach = this.CheckWindowVolumeBreach(opposingPosition, mostRecentTrade);

                if (this._equitiesParameters.CheckForCorrespondingPriceMovement != null
                    && this._equitiesParameters.CheckForCorrespondingPriceMovement.Value)
                    priceMovementBreach = this.CheckForPriceMovement(opposingPosition, mostRecentTrade);
            }

            var allTradesInPositions = opposingPosition.Get().Concat(tradingPosition.Get()).ToList();
            var allTrades = new TradePosition(allTradesInPositions);

            // wrong should use a judgement
            return HasRuleBreach(
                       hasBidirectionalBreach,
                       hasDailyVolumeBreach,
                       hasWindowVolumeBreach,
                       priceMovementBreach)
                       ? new LayeringRuleBreach(
                           this.OrganisationFactorValue,
                           this._ruleCtx.SystemProcessOperationContext(),
                           this._ruleCtx.CorrelationId(),
                           this._equitiesParameters,
                           this._equitiesParameters.Windows?.BackwardWindowSize ?? TimeSpan.Zero,
                           allTrades,
                           mostRecentTrade.Instrument,
                           hasBidirectionalBreach,
                           hasDailyVolumeBreach,
                           hasWindowVolumeBreach,
                           priceMovementBreach,
                           null,
                           null,
                           this.UniverseDateTime)
                       : null;
        }

        private RuleBreachDescription CheckWindowVolumeBreach(ITradePosition opposingPosition, Order mostRecentTrade)
        {
            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                this.UniverseDateTime.Subtract(this.BackwardWindowSize),
                this.UniverseDateTime,
                this._ruleCtx?.Id(),
                DataSource.AllIntraday);

            var tradingDays = this._tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                this.UniverseDateTime.Subtract(this.BackwardWindowSize),
                this.UniverseDateTime,
                mostRecentTrade.Market.MarketIdentifierCode);

            var securityResult = this.UniverseEquityIntradayCache.GetMarketsForRange(
                marketDataRequest,
                tradingDays,
                this.RunMode);
            if (securityResult.HadMissingData)
            {
                this._logger.LogWarning(
                    $"unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {this.UniverseDateTime}.");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var windowVolume = securityResult.Response.Sum(sdt => sdt?.SpreadTimeBar.Volume.Traded);
            if (windowVolume <= 0)
            {
                this._logger.LogInformation(
                    $"unable to sum meaningful volume from market data frames for volume window in {mostRecentTrade.Market.MarketIdentifierCode} at {this.UniverseDateTime}.");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            if (opposingPosition.TotalVolumeOrderedOrFilled() <= 0)
            {
                this._logger.LogInformation(
                    $"unable to calculate opposing position volume window in {mostRecentTrade.Market.MarketIdentifierCode} at {this.UniverseDateTime}.");

                this._hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var percentageWindowVolume = opposingPosition.TotalVolumeOrderedOrFilled() / (decimal)windowVolume;
            if (percentageWindowVolume >= this._equitiesParameters.PercentageOfMarketWindowVolume)
                return new RuleBreachDescription
                           {
                               RuleBreached = true,
                               Description =
                                   $" Percentage of market volume traded within a {this._equitiesParameters.Windows.BackwardWindowSize.TotalSeconds} second window exceeded the layering window threshold of {this._equitiesParameters.PercentageOfMarketWindowVolume * 100}%."
                           };

            return RuleBreachDescription.False();
        }

        private EquityInstrumentIntraDayTimeBar EndTick(
            List<EquityInstrumentIntraDayTimeBar> securityDataTicks,
            DateTime endDate)
        {
            if (securityDataTicks == null || !securityDataTicks.Any()) return null;

            EquityInstrumentIntraDayTimeBar endTick;
            if (securityDataTicks.Any(sdt => sdt.TimeStamp > endDate))
                endTick = securityDataTicks.Where(sdt => sdt.TimeStamp > endDate).OrderBy(sdt => sdt.TimeStamp)
                    .FirstOrDefault();
            else
                endTick = securityDataTicks.OrderBy(sdt => sdt.TimeStamp).Reverse().FirstOrDefault();

            return endTick;
        }

        private EquityInstrumentIntraDayTimeBar StartTick(
            List<EquityInstrumentIntraDayTimeBar> securityDataTicks,
            DateTime startDate)
        {
            if (securityDataTicks == null || !securityDataTicks.Any()) return null;

            EquityInstrumentIntraDayTimeBar startTick;
            if (securityDataTicks.Any(sdt => sdt.TimeStamp < startDate))
                startTick = securityDataTicks.Where(sdt => sdt.TimeStamp < startDate).OrderBy(sdt => sdt.TimeStamp)
                    .Reverse().FirstOrDefault();
            else
                startTick = securityDataTicks.OrderBy(sdt => sdt.TimeStamp).FirstOrDefault();

            return startTick;
        }
    }
}