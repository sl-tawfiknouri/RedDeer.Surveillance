﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;
using Domain.Core.Trading.Orders;
using SharedKernel.Contracts.Markets;

namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose
{
    public class MarkingTheCloseRule : BaseUniverseRule, IMarkingTheCloseRule
    {
        private readonly IMarkingTheCloseEquitiesParameters _equitiesParameters;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private readonly ILogger _logger;
        private volatile bool _processingMarketClose;
        private MarketOpenClose _latestMarketClosure;
        private bool _hadMissingData = false;

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
                Domain.Surveillance.Scheduling.Rules.MarkingTheClose,
                EquityRuleMarkingTheCloseFactory.Version,
                "Marking The Close",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            if (!_processingMarketClose
                || _latestMarketClosure == null)
            {
                return;
            }

            history.ArchiveExpiredActiveItems(_latestMarketClosure.MarketClose);

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
                            _latestMarketClosure.MarketId, 
                            StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

            if (!filteredMarketSecurities.Any())
            {
                // no relevant securities were being traded within the market closure time window
                return;
            }

            var marketSecurities = new Stack<Order>(filteredMarketSecurities);

            VolumeBreach dailyVolumeBreach = null;
            if (_equitiesParameters.PercentageThresholdDailyVolume != null)
            {
                dailyVolumeBreach = CheckDailyVolumeTraded(marketSecurities);
            }

            VolumeBreach windowVolumeBreach = null;
            if (_equitiesParameters.PercentageThresholdWindowVolume != null)
            {
                windowVolumeBreach = CheckWindowVolumeTraded(marketSecurities);
            }

            if ((dailyVolumeBreach == null || !dailyVolumeBreach.HasBreach())
                && (windowVolumeBreach == null || !windowVolumeBreach.HasBreach()))
            {
                _logger.LogInformation($"had no breaches for {marketSecurities.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}");
                return;
            }

            var position = new TradePosition(marketSecurities.ToList());

            // wrong but should be a judgement
            var breach = new MarkingTheCloseBreach(
                OrganisationFactorValue,
                _ruleCtx.SystemProcessOperationContext(),
                _ruleCtx.CorrelationId(),
                _equitiesParameters.Windows.BackwardWindowSize,
                marketSecurities.FirstOrDefault()?.Instrument,
                _latestMarketClosure,
                position,
                _equitiesParameters,
                dailyVolumeBreach ?? new VolumeBreach(),
                windowVolumeBreach ?? new VolumeBreach(),
                "desc",
                "title",
                UniverseDateTime);

            _logger.LogInformation($"had a breach for {marketSecurities.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Adding to alert stream.");
            var alertEvent = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.MarkingTheClose, breach, _ruleCtx);
            _alertStream.Add(alertEvent);
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

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
                UniverseDateTime.Subtract(BackwardWindowSize), // implicitly correct (market closure event trigger)
                UniverseDateTime,
                _ruleCtx?.Id(),
                DataSource.AllInterday);

            var dataResponse = UniverseEquityInterdayCache.Get(marketDataRequest);

            if (dataResponse.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogInformation($"had missing data for {securities.Peek().Instrument.Identifiers} on {UniverseDateTime}");
                return new VolumeBreach();
            }

            var tradedSecurity = dataResponse.Response;

            var thresholdVolumeTraded = tradedSecurity.DailySummaryTimeBar.DailyVolume.Traded * _equitiesParameters.PercentageThresholdDailyVolume;

            if (thresholdVolumeTraded == null)
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            var result =
                CalculateVolumeBreaches(
                    securities,
                    thresholdVolumeTraded.GetValueOrDefault(0),
                    tradedSecurity.DailySummaryTimeBar.DailyVolume.Traded);

            return result;
        }

        private VolumeBreach CheckWindowVolumeTraded(Stack<Order> securities)
        {
            if (!securities.Any())
            {
                return new VolumeBreach();
            }

            var tradingHours = _tradingHoursService.GetTradingHoursForMic(securities.Peek().Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {securities.Peek().Market?.MarketIdentifierCode}");
                return new VolumeBreach();
            }

            var tradingDates = _tradingHoursService.GetTradingDaysWithinRangeAdjustedToTime(
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(BackwardWindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                securities.Peek().Market?.MarketIdentifierCode);

            var marketDataRequest =
                new MarketDataRequest(
                    securities.Peek().Market.MarketIdentifierCode,
                    securities.Peek().Instrument.Cfi,
                    securities.Peek().Instrument.Identifiers,
                    UniverseDateTime.Subtract(BackwardWindowSize), // implicitly correct (market closure event trigger)
                    UniverseDateTime,
                    _ruleCtx?.Id(),
                    DataSource.AllIntraday);
            
            // marking the close should not have windows exceeding a few hours
            var marketResult = UniverseEquityIntradayCache.GetMarketsForRange(marketDataRequest, tradingDates, RunMode);
            if (marketResult.HadMissingData)
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            var securityUpdates = marketResult.Response;
            var securityVolume = securityUpdates.Sum(su => su.SpreadTimeBar.Volume.Traded);
            var thresholdVolumeTraded = securityVolume * _equitiesParameters.PercentageThresholdWindowVolume;

            if (thresholdVolumeTraded == null)
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            var result =
                CalculateVolumeBreaches(
                    securities, 
                    thresholdVolumeTraded.GetValueOrDefault(0),
                    securityVolume);

            return result;
        }

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
            var buyDailyPercentageBreach = CalculateBuyBreach(volumeTradedBuy, marketVolumeTraded, hasBuyDailyVolumeBreach);

            var hasSellDailyVolumeBreach = volumeTradedSell >= thresholdVolumeTraded;
            var sellDailyPercentageBreach = CalculateSellBreach(volumeTradedSell, marketVolumeTraded, hasSellDailyVolumeBreach);

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

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred at {exchange?.MarketClose}");

            _processingMarketClose = true;
            _latestMarketClosure = exchange;
            RunRuleForAllTradingHistoriesInMarket(exchange, exchange?.MarketClose);
            _processingMarketClose = false;
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured");

            if (_hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                _logger.LogInformation("had missing data at eschaton. Recording to op ctx.");
                var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.MarkingTheClose, null, _ruleCtx, false, true);
                _alertStream.Add(alert);

                _dataRequestSubscriber.SubmitRequest();
                _ruleCtx.EndEvent();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (MarkingTheCloseRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (MarkingTheCloseRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
