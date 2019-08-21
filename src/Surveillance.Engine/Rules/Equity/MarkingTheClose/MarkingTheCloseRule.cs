namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
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
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class MarkingTheCloseRule : BaseUniverseRule, IMarkingTheCloseRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly IMarkingTheCloseEquitiesParameters _equitiesParameters;

        private readonly ILogger _logger;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private bool _hadMissingData;

        private MarketOpenClose _latestMarketClosure;

        private volatile bool _processingMarketClose;

        public MarkingTheCloseRule(
            IMarkingTheCloseEquitiesParameters equitiesParameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketTradingHoursService tradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<MarkingTheCloseRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromMinutes(30),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.FromMinutes(30),
                Rules.MarkingTheClose,
                EquityRuleMarkingTheCloseFactory.Version,
                "Marking The Close",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this._equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._dataRequestSubscriber =
                dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (MarkingTheCloseRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (MarkingTheCloseRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation("Eschaton occured");

            if (this._hadMissingData && this.RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                this._logger.LogInformation("had missing data at eschaton. Recording to op ctx.");
                var alert = new UniverseAlertEvent(Rules.MarkingTheClose, null, this._ruleCtx, false, true);
                this._alertStream.Add(alert);

                this._dataRequestSubscriber.SubmitRequest();
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
            this._logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");

            this._processingMarketClose = true;
            this._latestMarketClosure = exchange;
            this.RunRuleForAllTradingHistoriesInMarket(exchange, exchange?.MarketClose);
            this._processingMarketClose = false;
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            if (!this._processingMarketClose || this._latestMarketClosure == null) return;

            history.ArchiveExpiredActiveItems(this._latestMarketClosure.MarketClose);

            var securities = history.ActiveTradeHistory();
            if (!securities.Any()) return;

            // filter the security list by the mic of the closing market....
            var filteredMarketSecurities = securities.Where(
                i => string.Equals(
                    i.Market?.MarketIdentifierCode,
                    this._latestMarketClosure.MarketId,
                    StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (!filteredMarketSecurities.Any()) return;

            var marketSecurities = new Stack<Order>(filteredMarketSecurities);

            VolumeBreach dailyVolumeBreach = null;
            if (this._equitiesParameters.PercentageThresholdDailyVolume != null)
                dailyVolumeBreach = this.CheckDailyVolumeTraded(marketSecurities);

            VolumeBreach windowVolumeBreach = null;
            if (this._equitiesParameters.PercentageThresholdWindowVolume != null)
                windowVolumeBreach = this.CheckWindowVolumeTraded(marketSecurities);

            if ((dailyVolumeBreach == null || !dailyVolumeBreach.HasBreach())
                && (windowVolumeBreach == null || !windowVolumeBreach.HasBreach()))
            {
                this._logger.LogInformation(
                    $"had no breaches for {marketSecurities.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}");
                return;
            }

            var position = new TradePosition(marketSecurities.ToList());

            // wrong but should be a judgement
            var breach = new MarkingTheCloseBreach(
                this.OrganisationFactorValue,
                this._ruleCtx.SystemProcessOperationContext(),
                this._ruleCtx.CorrelationId(),
                this._equitiesParameters.Windows.BackwardWindowSize,
                marketSecurities.FirstOrDefault()?.Instrument,
                this._latestMarketClosure,
                position,
                this._equitiesParameters,
                dailyVolumeBreach ?? new VolumeBreach(),
                windowVolumeBreach ?? new VolumeBreach(),
                null,
                null,
                this.UniverseDateTime);

            this._logger.LogInformation(
                $"had a breach for {marketSecurities.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. Adding to alert stream.");
            var alertEvent = new UniverseAlertEvent(Rules.MarkingTheClose, breach, this._ruleCtx);
            this._alertStream.Add(alertEvent);
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private decimal? CalculateBuyBreach(decimal volumeTradedBuy, decimal marketVolume, bool hasBuyVolumeBreach)
        {
            return hasBuyVolumeBreach && volumeTradedBuy > 0 && marketVolume > 0

                       // ReSharper disable RedundantCast
                       ? (decimal?)((decimal)volumeTradedBuy / (decimal)marketVolume)

                       // ReSharper restore RedundantCast
                       : null;
        }

        private decimal? CalculateSellBreach(
            decimal volumeTradedSell,
            decimal marketVolume,
            bool hasSellDailyVolumeBreach)
        {
            return hasSellDailyVolumeBreach && volumeTradedSell > 0 && marketVolume > 0

                       // ReSharper disable RedundantCast
                       ? (decimal?)((decimal)volumeTradedSell / (decimal)marketVolume)

                       // ReSharper restore RedundantCast
                       : null;
        }

        private VolumeBreach CalculateVolumeBreaches(
            Stack<Order> securities,
            decimal thresholdVolumeTraded,
            decimal marketVolumeTraded)
        {
            if (securities == null || !securities.Any()) return new VolumeBreach();

            var volumeTradedBuy = securities
                .Where(sec => sec.OrderDirection == OrderDirections.BUY || sec.OrderDirection == OrderDirections.COVER)
                .Sum(sec => sec.OrderFilledVolume.GetValueOrDefault(0));

            var volumeTradedSell = securities
                .Where(sec => sec.OrderDirection == OrderDirections.SELL || sec.OrderDirection == OrderDirections.SHORT)
                .Sum(sec => sec.OrderFilledVolume.GetValueOrDefault(0));

            var hasBuyDailyVolumeBreach = volumeTradedBuy >= thresholdVolumeTraded;
            var buyDailyPercentageBreach = this.CalculateBuyBreach(
                volumeTradedBuy,
                marketVolumeTraded,
                hasBuyDailyVolumeBreach);

            var hasSellDailyVolumeBreach = volumeTradedSell >= thresholdVolumeTraded;
            var sellDailyPercentageBreach = this.CalculateSellBreach(
                volumeTradedSell,
                marketVolumeTraded,
                hasSellDailyVolumeBreach);

            if (!hasSellDailyVolumeBreach && !hasBuyDailyVolumeBreach) return new VolumeBreach();

            return new VolumeBreach
                       {
                           BuyVolumeBreach = buyDailyPercentageBreach,
                           HasBuyVolumeBreach = hasBuyDailyVolumeBreach,
                           SellVolumeBreach = sellDailyPercentageBreach,
                           HasSellVolumeBreach = hasSellDailyVolumeBreach
                       };
        }

        private VolumeBreach CheckDailyVolumeTraded(Stack<Order> securities)
        {
            if (!securities.Any()) return new VolumeBreach();

            var marketDataRequest = new MarketDataRequest(
                securities.Peek().Market.MarketIdentifierCode,
                securities.Peek().Instrument.Cfi,
                securities.Peek().Instrument.Identifiers,
                this.UniverseDateTime.Subtract(
                    this.BackwardWindowSize), // implicitly correct (market closure event trigger)
                this.UniverseDateTime,
                this._ruleCtx?.Id(),
                DataSource.AllInterday);

            var dataResponse = this.UniverseEquityInterdayCache.Get(marketDataRequest);

            if (dataResponse.HadMissingData)
            {
                this._hadMissingData = true;
                this._logger.LogInformation(
                    $"had missing data for {securities.Peek().Instrument.Identifiers} on {this.UniverseDateTime}");
                return new VolumeBreach();
            }

            var tradedSecurity = dataResponse.Response;

            var thresholdVolumeTraded = tradedSecurity.DailySummaryTimeBar.DailyVolume.Traded
                                        * this._equitiesParameters.PercentageThresholdDailyVolume;

            if (thresholdVolumeTraded == null)
            {
                this._hadMissingData = true;
                return new VolumeBreach();
            }

            var result = this.CalculateVolumeBreaches(
                securities,
                thresholdVolumeTraded.GetValueOrDefault(0),
                tradedSecurity.DailySummaryTimeBar.DailyVolume.Traded);

            return result;
        }

        private VolumeBreach CheckWindowVolumeTraded(Stack<Order> securities)
        {
            if (!securities.Any()) return new VolumeBreach();

            var tradingHours =
                this._tradingHoursService.GetTradingHoursForMic(securities.Peek().Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {securities.Peek().Market?.MarketIdentifierCode}");
                return new VolumeBreach();
            }

            var tradingDates = this._tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(this.UniverseDateTime),
                securities.Peek().Market?.MarketIdentifierCode);

            var marketDataRequest = new MarketDataRequest(
                securities.Peek().Market.MarketIdentifierCode,
                securities.Peek().Instrument.Cfi,
                securities.Peek().Instrument.Identifiers,
                this.UniverseDateTime.Subtract(
                    this.BackwardWindowSize), // implicitly correct (market closure event trigger)
                this.UniverseDateTime,
                this._ruleCtx?.Id(),
                DataSource.AllIntraday);

            // marking the close should not have windows exceeding a few hours
            var marketResult = this.UniverseEquityIntradayCache.GetMarketsForRange(
                marketDataRequest,
                tradingDates,
                this.RunMode);
            if (marketResult.HadMissingData)
            {
                this._hadMissingData = true;
                return new VolumeBreach();
            }

            var securityUpdates = marketResult.Response;
            var securityVolume = securityUpdates.Sum(su => su.SpreadTimeBar.Volume.Traded);
            var thresholdVolumeTraded = securityVolume * this._equitiesParameters.PercentageThresholdWindowVolume;

            if (thresholdVolumeTraded == null)
            {
                this._hadMissingData = true;
                return new VolumeBreach();
            }

            var result = this.CalculateVolumeBreaches(
                securities,
                thresholdVolumeTraded.GetValueOrDefault(0),
                securityVolume);

            return result;
        }
    }
}