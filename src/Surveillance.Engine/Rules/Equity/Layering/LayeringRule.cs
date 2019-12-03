namespace Surveillance.Engine.Rules.Rules.Equity.Layering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
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

    /// <summary>
    /// The layering rule.
    /// </summary>
    public class LayeringRule : BaseUniverseRule, ILayeringRule
    {
        /// <summary>
        /// The trading hours service.
        /// </summary>
        private readonly IMarketTradingHoursService tradingHoursService;

        /// <summary>
        /// The rule context.
        /// </summary>
        private readonly ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream alertStream;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// The equities parameters.
        /// </summary>
        private readonly ILayeringRuleEquitiesParameters equitiesParameters;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The had missing data.
        /// </summary>
        private bool hadMissingData = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeringRule"/> class.
        /// </summary>
        /// <param name="equitiesParameters">
        /// The equities parameters.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="equityMarketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="fixedIncomeMarketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="tradingHoursService">
        /// The trading hours service.
        /// </param>
        /// <param name="operationContext">
        /// The op context.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// The trading history logger.
        /// </param>
        public LayeringRule(
            ILayeringRuleEquitiesParameters equitiesParameters,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            ILogger logger,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            IMarketTradingHoursService tradingHoursService,
            ISystemProcessOperationRunRuleContext operationContext,
            RuleRunMode runMode,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(20),
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(20),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.Layering,
                EquityRuleLayeringFactory.Version,
                "Layering Rule",
                operationContext,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this.ruleContext = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            this.alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public override IRuleDataConstraint DataConstraints()
        {
            if (this.equitiesParameters == null)
            {
                return RuleDataConstraint.Empty().Case;
            }

            var constraints = new List<RuleDataSubConstraint>();

            if (this.equitiesParameters.PercentageOfMarketDailyVolume != null)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyInterday,
                    _ => !this.orderFilter.Filter(_));

                constraints.Add(constraint);
            }

            if (this.equitiesParameters.PercentageOfMarketWindowVolume != null)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyInterday,
                    _ => !this.orderFilter.Filter(_));

                constraints.Add(constraint);
            }

            return new RuleDataConstraint(
                this.Rule,
                this.equitiesParameters.Id,
                constraints);
        }

        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.orderFilter.Filter(value);
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
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

            this.AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            var tradingPosition =
                (mostRecentTrade.OrderDirection == OrderDirections.BUY 
                 || mostRecentTrade.OrderDirection == OrderDirections.COVER)
                    ? buyPosition
                    : sellPosition;

            var opposingPosition =
                (mostRecentTrade.OrderDirection == OrderDirections.SELL
                 || mostRecentTrade.OrderDirection == OrderDirections.SHORT)
                    ? buyPosition
                    : sellPosition;

            var layeringRuleBreach =
                this.CheckPositionForLayering(
                    tradeWindow,
                    buyPosition,
                    sellPosition,
                    tradingPosition,
                    opposingPosition,
                    mostRecentTrade);

            if (layeringRuleBreach != null)
            {
                this.logger.LogInformation($"RunInitialSubmissionRule had a breach for {mostRecentTrade?.Instrument?.Identifiers}. Passing to alert stream.");
                var universeAlert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Layering, layeringRuleBreach, this.ruleContext);
                this.alertStream.Add(universeAlert);
            }
        }

        /// <summary>
        /// The add to positions.
        /// </summary>
        /// <param name="buyPosition">
        /// The buy position.
        /// </param>
        /// <param name="sellPosition">
        /// The sell position.
        /// </param>
        /// <param name="nextTrade">
        /// The next trade.
        /// </param>
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
                    this.logger.LogError("not considering an out of range order direction");
                    this.ruleContext.EventException("not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
        }

        /// <summary>
        /// The check position for layering.
        /// </summary>
        /// <param name="tradeWindow">
        /// The trade window.
        /// </param>
        /// <param name="buyPosition">
        /// The buy position.
        /// </param>
        /// <param name="sellPosition">
        /// The sell position.
        /// </param>
        /// <param name="tradingPosition">
        /// The trading position.
        /// </param>
        /// <param name="opposingPosition">
        /// The opposing position.
        /// </param>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <returns>
        /// The <see cref="ILayeringRuleBreach"/>.
        /// </returns>
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
                this.AddToPositions(buyPosition, sellPosition, nextTrade);

                if (!tradingPosition.Get().Any()
                    || !opposingPosition.Get().Any())
                {
                    continue;
                }

                // IF ALL PARAMETERS ARE NULL JUST DO THE BIDIRECTIONAL TRADE CHECK
                if (this.equitiesParameters.PercentageOfMarketDailyVolume == null
                    && this.equitiesParameters.PercentageOfMarketWindowVolume == null
                    && this.equitiesParameters.CheckForCorrespondingPriceMovement == null)
                {
                    hasBidirectionalBreach = new RuleBreachDescription
                    {
                        RuleBreached = true,
                        Description = " Trading in both buy/sell positions simultaneously was detected."
                    };
                }

                if (this.equitiesParameters.PercentageOfMarketDailyVolume != null)
                {
                    hasDailyVolumeBreach = this.CheckDailyVolumeBreach(opposingPosition, mostRecentTrade);
                }

                if (this.equitiesParameters.PercentageOfMarketWindowVolume != null)
                {
                    hasWindowVolumeBreach = this.CheckWindowVolumeBreach(opposingPosition, mostRecentTrade);
                }

                if (this.equitiesParameters.CheckForCorrespondingPriceMovement != null
                    && this.equitiesParameters.CheckForCorrespondingPriceMovement.Value)
                {
                    priceMovementBreach = this.CheckForPriceMovement(opposingPosition, mostRecentTrade);
                }
            }

            var allTradesInPositions = opposingPosition.Get().Concat(tradingPosition.Get()).ToList();
            var allTrades = new TradePosition(allTradesInPositions);

            if (!this.HasRuleBreach(
                    hasBidirectionalBreach,
                    hasDailyVolumeBreach,
                    hasWindowVolumeBreach,
                    priceMovementBreach))
            {
                return null;
            }

            return new LayeringRuleBreach(
                this.OrganisationFactorValue,
                this.ruleContext.SystemProcessOperationContext(),
                this.ruleContext.CorrelationId(),
                this.equitiesParameters,
                this.equitiesParameters.Windows?.BackwardWindowSize ?? TimeSpan.Zero,
                allTrades,
                mostRecentTrade.Instrument,
                hasBidirectionalBreach,
                hasDailyVolumeBreach,
                hasWindowVolumeBreach,
                priceMovementBreach,
                null,
                null,
                UniverseDateTime);
        }

        /// <summary>
        /// The has rule breach.
        /// </summary>
        /// <param name="hasBidirectionalBreach">
        /// The has bidirectional breach.
        /// </param>
        /// <param name="hasDailyVolumeBreach">
        /// The has daily volume breach.
        /// </param>
        /// <param name="hasWindowVolumeBreach">
        /// The has window volume breach.
        /// </param>
        /// <param name="priceMovementBreach">
        /// The price movement breach.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasRuleBreach(
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

        /// <summary>
        /// The check daily volume breach.
        /// </summary>
        /// <param name="opposingPosition">
        /// The opposing position.
        /// </param>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <returns>
        /// The <see cref="RuleBreachDescription"/>.
        /// </returns>
        private RuleBreachDescription CheckDailyVolumeBreach(
            ITradePosition opposingPosition,
            Order mostRecentTrade)
        {
            var tradingHoursManager = this.tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market.MarketIdentifierCode);

            if (!tradingHoursManager.IsValid)
            {
                this.logger.LogInformation($"unable to fetch market data for ({mostRecentTrade.Market.MarketIdentifierCode}) for the most recent trade {mostRecentTrade?.Instrument?.Identifiers} the market data did not contain the security indicated as trading in that market");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var marketRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHoursManager.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                    tradingHoursManager.ClosingInUtcForDay(UniverseDateTime),
                    this.ruleContext?.Id(),
                    DataSource.AnyInterday);
            
            var marketResult = UniverseEquityInterdayCache.Get(marketRequest);
            if (marketResult.HadMissingData)
            {
                this.logger.LogInformation($"unable to fetch market data for ({mostRecentTrade.Market.MarketIdentifierCode}) for the most recent trade {mostRecentTrade?.Instrument?.Identifiers} the market data did not contain the security indicated as trading in that market");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var marketSecurityData = marketResult.Response;
            if (marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded <= 0
                || opposingPosition.TotalVolumeOrderedOrFilled() <= 0)
            {
                this.logger.LogInformation($"unable to evaluate for {mostRecentTrade?.Instrument?.Identifiers} either the market daily volume data was not available or the opposing position had a bad total volume value (daily volume){marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded} - (opposing position){opposingPosition.TotalVolumeOrderedOrFilled()}");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var percentageDailyVolume = (decimal)opposingPosition.TotalVolumeOrderedOrFilled() / (decimal)marketSecurityData?.DailySummaryTimeBar?.DailyVolume.Traded;
            if (percentageDailyVolume >= this.equitiesParameters.PercentageOfMarketDailyVolume)
            {
                return new RuleBreachDescription
                {
                    RuleBreached = true,
                    Description = $" Percentage of market daily volume traded within a {this.equitiesParameters.Windows.BackwardWindowSize.TotalSeconds} second window exceeded the layering window threshold of {this.equitiesParameters.PercentageOfMarketDailyVolume * 100}%."
                };
            }

            return RuleBreachDescription.False();
        }

        /// <summary>
        /// The check window volume breach.
        /// </summary>
        /// <param name="opposingPosition">
        /// The opposing position.
        /// </param>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <returns>
        /// The <see cref="RuleBreachDescription"/>.
        /// </returns>
        private RuleBreachDescription CheckWindowVolumeBreach(
            ITradePosition opposingPosition,
            Order mostRecentTrade)
        {
            var marketDataRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    UniverseDateTime.Subtract(this.TradeBackwardWindowSize),
                    UniverseDateTime,
                    this.ruleContext?.Id(),
                    DataSource.AnyIntraday);

            var tradingDays =
                this.tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                    UniverseDateTime.Subtract(this.TradeBackwardWindowSize),
                    UniverseDateTime,
                    mostRecentTrade.Market.MarketIdentifierCode);

            var securityResult = UniverseEquityIntradayCache.GetMarketsForRange(marketDataRequest, tradingDays, RunMode);
            if (securityResult.HadMissingData)
            {
                this.logger.LogWarning($"unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var windowVolume = securityResult.Response.Sum(sdt => sdt?.SpreadTimeBar.Volume.Traded);
            if (windowVolume <= 0)
            {
                this.logger.LogInformation($"unable to sum meaningful volume from market data frames for volume window in {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }

            if (opposingPosition.TotalVolumeOrderedOrFilled() <= 0)
            {
                this.logger.LogInformation($"unable to calculate opposing position volume window in {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var percentageWindowVolume = (decimal)opposingPosition.TotalVolumeOrderedOrFilled() / (decimal)windowVolume;
            if (percentageWindowVolume >= this.equitiesParameters.PercentageOfMarketWindowVolume)
            {
                return new RuleBreachDescription
                {
                    RuleBreached = true,
                    Description = $" Percentage of market volume traded within a {this.equitiesParameters.Windows.BackwardWindowSize.TotalSeconds} second window exceeded the layering window threshold of {this.equitiesParameters.PercentageOfMarketWindowVolume * 100}%."
                };
            }

            return RuleBreachDescription.False();
        }

        /// <summary>
        /// The check for price movement.
        /// </summary>
        /// <param name="opposingPosition">
        /// The opposing position.
        /// </param>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <returns>
        /// The <see cref="RuleBreachDescription"/>.
        /// </returns>
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
                    startDate.Subtract(this.TradeBackwardWindowSize),
                    endDate,
                    this.ruleContext?.Id(),
                    DataSource.AnyIntraday);

            var tradingDays =
                this.tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                    UniverseDateTime.Subtract(this.TradeBackwardWindowSize),
                    UniverseDateTime,
                    mostRecentTrade.Market.MarketIdentifierCode);

            var marketResult = UniverseEquityIntradayCache.GetMarketsForRange(marketRequest, tradingDays, RunMode);

            if (marketResult.HadMissingData)
            {
                this.logger.LogInformation($"unable to fetch market data frames for {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }
            
            if (mostRecentTrade.PlacedDate > endDate)
            {
                endDate = mostRecentTrade.PlacedDate.GetValueOrDefault();
            }

            var securityDataTicks = marketResult.Response;
            var startTick = this.StartTick(securityDataTicks, startDate);
            if (startTick == null)
            {
                this.logger.LogInformation($"unable to fetch starting exchange tick data for ({startDate}) {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var endTick = this.EndTick(securityDataTicks, endDate);
            if (endTick == null)
            {
                this.logger.LogInformation($"unable to fetch ending exchange tick data for ({endDate}) {mostRecentTrade.Market.MarketIdentifierCode} at {UniverseDateTime}.");

                this.hadMissingData = true;
                return RuleBreachDescription.False();
            }

            var priceMovement = endTick.SpreadTimeBar.Price.Value - startTick.SpreadTimeBar.Price.Value;

            return this.BuildDescription(mostRecentTrade, priceMovement, startTick, endTick);
        }

        /// <summary>
        /// The build description.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="priceMovement">
        /// The price movement.
        /// </param>
        /// <param name="startTick">
        /// The start tick.
        /// </param>
        /// <param name="endTick">
        /// The end tick.
        /// </param>
        /// <returns>
        /// The <see cref="RuleBreachDescription"/>.
        /// </returns>
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
                        ? new RuleBreachDescription { RuleBreached = true, Description = $" Prices in {mostRecentTrade.Instrument.Name} moved from ({endTick.SpreadTimeBar.Price.Currency}) {endTick.SpreadTimeBar.Price.Value} to ({startTick.SpreadTimeBar.Price.Currency}) {startTick.SpreadTimeBar.Price.Value} for a net change of {startTick.SpreadTimeBar.Price.Currency} {priceMovement} in line with the layering price pressure influence." }
                        : RuleBreachDescription.False();
                case OrderDirections.SELL:
                case OrderDirections.SHORT:
                    return priceMovement > 0
                        ? new RuleBreachDescription { RuleBreached = true, Description = $" Prices in {mostRecentTrade.Instrument.Name} moved from ({endTick.SpreadTimeBar.Price.Currency}) {endTick.SpreadTimeBar.Price.Value} to ({startTick.SpreadTimeBar.Price.Currency}) {startTick.SpreadTimeBar.Price.Value} for a net change of {startTick.SpreadTimeBar.Price.Currency} {priceMovement} in line with the layering price pressure influence." } : RuleBreachDescription.False();
                default:
                    this.logger.LogError($"Layering rule is not taking into account a new order position value (handles buy/sell) {mostRecentTrade.OrderDirection} (Arg Out of Range)");
                    this.ruleContext.EventException($"Layering rule is not taking into account a new order position value (handles buy/sell) {mostRecentTrade.OrderDirection} (Arg Out of Range)");
                    return RuleBreachDescription.False();
            }
        }

        /// <summary>
        /// The start tick.
        /// </summary>
        /// <param name="securityDataTicks">
        /// The security data ticks.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <returns>
        /// The <see cref="EquityInstrumentIntraDayTimeBar"/>.
        /// </returns>
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

        /// <summary>
        /// The end tick.
        /// </summary>
        /// <param name="securityDataTicks">
        /// The security data ticks.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The <see cref="EquityInstrumentIntraDayTimeBar"/>.
        /// </returns>
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

        /// <summary>
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            // we don't analyse rules based on when their status last changed in the layering rule
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // we don't analyse rules based on fills in the layering rule
        }

        /// <summary>
        /// The run post order event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run initial submission event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run order filled event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation("Genesis occurred");
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred {exchange?.MarketClose}");
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation("Eschaton occured");

            var universeAlert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Layering, null, this.ruleContext, true);
            this.alertStream.Add(universeAlert);

            if (this.hadMissingData)
            {
                this.logger.LogInformation($"had missing data. Updating rule context with state.");
                this.ruleContext.EndEvent();
            }
            else
            {
                this.ruleContext?.EndEvent();
            }
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (LayeringRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            var clone = (LayeringRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
