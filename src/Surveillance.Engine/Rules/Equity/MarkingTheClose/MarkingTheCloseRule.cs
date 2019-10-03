namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading;
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
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The marking the close rule.
    /// </summary>
    public class MarkingTheCloseRule : BaseUniverseRule, IMarkingTheCloseRule
    {
        /// <summary>
        /// The equities parameters.
        /// </summary>
        private readonly IMarkingTheCloseEquitiesParameters equitiesParameters;

        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream alertStream;

        /// <summary>
        /// The rule context.
        /// </summary>
        private readonly ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// The trading hours service.
        /// </summary>
        private readonly IMarketTradingHoursService tradingHoursService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The processing market close.
        /// </summary>
        private volatile bool processingMarketClose;

        /// <summary>
        /// The latest market closure.
        /// </summary>
        private MarketOpenClose latestMarketClosure;

        /// <summary>
        /// The had missing data.
        /// </summary>
        private bool hadMissingData = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkingTheCloseRule"/> class.
        /// </summary>
        /// <param name="equitiesParameters">
        /// The equities parameters.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="marketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="tradingHoursService">
        /// The trading hours service.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// The trading history logger.
        /// </param>
        public MarkingTheCloseRule(
            IMarkingTheCloseEquitiesParameters equitiesParameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketTradingHoursService tradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<MarkingTheCloseRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(30),
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(30),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.FromMinutes(30),
                Domain.Surveillance.Scheduling.Rules.MarkingTheClose,
                EquityRuleMarkingTheCloseFactory.Version,
                "Marking The Close",
                ruleContext,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this.alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.ruleContext = ruleContext ?? throw new ArgumentNullException(nameof(ruleContext));
            this.orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this.tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this.dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organisation factor value.
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

            if (this.equitiesParameters.PercentageThresholdDailyVolume != null)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyInterday,
                    _ => !this.orderFilter.Filter(_));

                constraints.Add(constraint);
            }

            if (this.equitiesParameters.PercentageThresholdWindowVolume != null)
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
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            if (!this.processingMarketClose
                || this.latestMarketClosure == null)
            {
                return;
            }

            history.ArchiveExpiredActiveItems(this.latestMarketClosure.MarketClose);

            var securities = history.ActiveTradeHistory();
            if (!securities.Any())
            {
                // no securities were being traded within the market closure time window
                return;
            }

            // filter the security list by the mic of the closing market....
            var filteredMarketSecurities =
                securities
                    .Where(i => 
                        string.Equals(
                            i.Market?.MarketIdentifierCode,
                            this.latestMarketClosure.MarketId, 
                            StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

            if (!filteredMarketSecurities.Any())
            {
                // no relevant securities were being traded within the market closure time window
                return;
            }

            var marketSecurities = new Stack<Order>(filteredMarketSecurities);

            VolumeBreach dailyVolumeBreach = null;
            if (this.equitiesParameters.PercentageThresholdDailyVolume != null)
            {
                dailyVolumeBreach = this.CheckDailyVolumeTraded(marketSecurities);
            }

            VolumeBreach windowVolumeBreach = null;
            if (this.equitiesParameters.PercentageThresholdWindowVolume != null)
            {
                windowVolumeBreach = this.CheckWindowVolumeTraded(marketSecurities);
            }

            if ((dailyVolumeBreach == null || !dailyVolumeBreach.HasBreach())
                && (windowVolumeBreach == null || !windowVolumeBreach.HasBreach()))
            {
                this.logger.LogInformation($"had no breaches for {marketSecurities.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}");
                return;
            }

            var position = new TradePosition(marketSecurities.ToList());

            // wrong but should be a judgement
            var breach = new MarkingTheCloseBreach(
                this.OrganisationFactorValue,
                this.ruleContext.SystemProcessOperationContext(),
                this.ruleContext.CorrelationId(),
                this.equitiesParameters.Windows.BackwardWindowSize,
                marketSecurities.FirstOrDefault()?.Instrument,
                this.latestMarketClosure,
                position,
                this.equitiesParameters,
                dailyVolumeBreach ?? new VolumeBreach(),
                windowVolumeBreach ?? new VolumeBreach(),
                null,
                null,
                this.UniverseDateTime);

            this.logger.LogInformation($"had a breach for {marketSecurities.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Adding to alert stream.");
            var alertEvent = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.MarkingTheClose, breach, this.ruleContext);
            this.alertStream.Add(alertEvent);
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
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
        /// The check daily volume traded.
        /// </summary>
        /// <param name="securities">
        /// The securities.
        /// </param>
        /// <returns>
        /// The <see cref="VolumeBreach"/>.
        /// </returns>
        private VolumeBreach CheckDailyVolumeTraded(
            Stack<Order> securities)
        {
            if (!securities.Any())
            {
                return new VolumeBreach();
            }

            var marketDataRequest = new MarketDataRequest(
                securities.Peek().Market.MarketIdentifierCode,
                securities.Peek().Instrument.Cfi,
                securities.Peek().Instrument.Identifiers,
                UniverseDateTime.Subtract(this.TradeBackwardWindowSize), // implicitly correct (market closure event trigger)
                UniverseDateTime,
                this.ruleContext?.Id(),
                DataSource.AnyInterday);

            var dataResponse = UniverseEquityInterdayCache.Get(marketDataRequest);

            if (dataResponse.HadMissingData)
            {
                this.hadMissingData = true;
                this.logger.LogInformation($"had missing data for {securities.Peek().Instrument.Identifiers} on {UniverseDateTime}");
                return new VolumeBreach();
            }

            var tradedSecurity = dataResponse.Response;

            var thresholdVolumeTraded = tradedSecurity.DailySummaryTimeBar.DailyVolume.Traded * this.equitiesParameters.PercentageThresholdDailyVolume;

            if (thresholdVolumeTraded == null)
            {
                this.hadMissingData = true;
                return new VolumeBreach();
            }

            var result =
                this.CalculateVolumeBreaches(
                    securities,
                    thresholdVolumeTraded.GetValueOrDefault(0),
                    tradedSecurity.DailySummaryTimeBar.DailyVolume.Traded);

            return result;
        }

        /// <summary>
        /// The check window volume traded.
        /// </summary>
        /// <param name="securities">
        /// The securities.
        /// </param>
        /// <returns>
        /// The <see cref="VolumeBreach"/>.
        /// </returns>
        private VolumeBreach CheckWindowVolumeTraded(Stack<Order> securities)
        {
            if (!securities.Any())
            {
                return new VolumeBreach();
            }

            var tradingHours = this.tradingHoursService.GetTradingHoursForMic(securities.Peek().Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.logger.LogError($"Request for trading hours was invalid. MIC - {securities.Peek().Market?.MarketIdentifierCode}");
                return new VolumeBreach();
            }

            var tradingDates = this.tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                securities.Peek().Market?.MarketIdentifierCode);

            var marketDataRequest =
                new MarketDataRequest(
                    securities.Peek().Market.MarketIdentifierCode,
                    securities.Peek().Instrument.Cfi,
                    securities.Peek().Instrument.Identifiers,
                    UniverseDateTime.Subtract(this.TradeBackwardWindowSize), // implicitly correct (market closure event trigger)
                    UniverseDateTime,
                    this.ruleContext?.Id(),
                    DataSource.AnyIntraday);
            
            // marking the close should not have windows exceeding a few hours
            var marketResult = UniverseEquityIntradayCache.GetMarketsForRange(marketDataRequest, tradingDates, RunMode);
            if (marketResult.HadMissingData)
            {
                this.hadMissingData = true;
                return new VolumeBreach();
            }

            var securityUpdates = marketResult.Response;
            var securityVolume = securityUpdates.Sum(su => su.SpreadTimeBar.Volume.Traded);
            var thresholdVolumeTraded = securityVolume * this.equitiesParameters.PercentageThresholdWindowVolume;

            if (thresholdVolumeTraded == null)
            {
                this.hadMissingData = true;
                return new VolumeBreach();
            }

            var result =
                this.CalculateVolumeBreaches(
                    securities, 
                    thresholdVolumeTraded.GetValueOrDefault(0),
                    securityVolume);

            return result;
        }

        /// <summary>
        /// The calculate volume breaches.
        /// </summary>
        /// <param name="securities">
        /// The securities.
        /// </param>
        /// <param name="thresholdVolumeTraded">
        /// The threshold volume traded.
        /// </param>
        /// <param name="marketVolumeTraded">
        /// The market volume traded.
        /// </param>
        /// <returns>
        /// The <see cref="VolumeBreach"/>.
        /// </returns>
        private VolumeBreach CalculateVolumeBreaches(
            Stack<Order> securities,
            decimal thresholdVolumeTraded,
            decimal marketVolumeTraded)
        {
            if (securities == null
                || !securities.Any())
            {
                return new VolumeBreach();
            }

            var volumeTradedBuy =
                securities
                    .Where(sec => 
                        sec.OrderDirection == OrderDirections.BUY
                        || sec.OrderDirection == OrderDirections.COVER)
                    .Sum(sec => sec.OrderFilledVolume.GetValueOrDefault(0));

            var volumeTradedSell =
                securities
                    .Where(sec => 
                        sec.OrderDirection == OrderDirections.SELL
                        || sec.OrderDirection == OrderDirections.SHORT)
                    .Sum(sec => sec.OrderFilledVolume.GetValueOrDefault(0));

            var hasBuyDailyVolumeBreach = volumeTradedBuy >= thresholdVolumeTraded;
            var buyDailyPercentageBreach = this.CalculateBuyBreach(volumeTradedBuy, marketVolumeTraded, hasBuyDailyVolumeBreach);

            var hasSellDailyVolumeBreach = volumeTradedSell >= thresholdVolumeTraded;
            var sellDailyPercentageBreach = this.CalculateSellBreach(volumeTradedSell, marketVolumeTraded, hasSellDailyVolumeBreach);

            if (!hasSellDailyVolumeBreach
                && !hasBuyDailyVolumeBreach)
            {
                return new VolumeBreach();
            }

            return new VolumeBreach
            {
                BuyVolumeBreach = buyDailyPercentageBreach,
                HasBuyVolumeBreach = hasBuyDailyVolumeBreach,
                SellVolumeBreach = sellDailyPercentageBreach,
                HasSellVolumeBreach = hasSellDailyVolumeBreach
            };
        }

        /// <summary>
        /// The calculate buy breach.
        /// </summary>
        /// <param name="volumeTradedBuy">
        /// The volume traded buy.
        /// </param>
        /// <param name="marketVolume">
        /// The market volume.
        /// </param>
        /// <param name="hasBuyVolumeBreach">
        /// The has buy volume breach.
        /// </param>
        /// <returns>
        /// The <see cref="decimal?"/>.
        /// </returns>
        private decimal? CalculateBuyBreach(decimal volumeTradedBuy, decimal marketVolume, bool hasBuyVolumeBreach)
        {
            return hasBuyVolumeBreach
                && volumeTradedBuy > 0
                && marketVolume > 0
                // ReSharper disable RedundantCast
                ? (decimal?)((decimal)volumeTradedBuy / (decimal)marketVolume)
                // ReSharper restore RedundantCast
                    : null;
        }

        /// <summary>
        /// The calculate sell breach.
        /// </summary>
        /// <param name="volumeTradedSell">
        /// The volume traded sell.
        /// </param>
        /// <param name="marketVolume">
        /// The market volume.
        /// </param>
        /// <param name="hasSellDailyVolumeBreach">
        /// The has sell daily volume breach.
        /// </param>
        /// <returns>
        /// The <see cref="decimal?"/>.
        /// </returns>
        private decimal? CalculateSellBreach(decimal volumeTradedSell, decimal marketVolume, bool hasSellDailyVolumeBreach)
        {
            return hasSellDailyVolumeBreach
                   && volumeTradedSell > 0
                   && marketVolume > 0
                // ReSharper disable RedundantCast
                ? (decimal?)((decimal)volumeTradedSell / (decimal)marketVolume)
                // ReSharper restore RedundantCast
                : null;
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
            this.logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");

            this.processingMarketClose = true;
            this.latestMarketClosure = exchange;
            this.RunRuleForAllTradingHistoriesInMarket(exchange, exchange?.MarketClose);
            this.processingMarketClose = false;
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation("Eschaton occured");

            if (this.hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                this.logger.LogInformation("had missing data at eschaton. Recording to op ctx.");
                var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.MarkingTheClose, null, this.ruleContext, false, true);
                this.alertStream.Add(alert);

                this.dataRequestSubscriber.SubmitRequest();
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
            var clone = (MarkingTheCloseRule)Clone();
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
            var clone = (MarkingTheCloseRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
