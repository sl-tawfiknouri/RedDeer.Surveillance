﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.TimeBars;
using Domain.Financial;
using Domain.Markets;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.MarkingTheClose
{
    public class MarkingTheCloseRule : BaseUniverseRule, IMarkingTheCloseRule
    {
        private readonly IMarkingTheCloseParameters _parameters;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private readonly ILogger _logger;
        private volatile bool _processingMarketClose;
        private MarketOpenClose _latestMarketClosure;
        private bool _hadMissingData = false;

        public MarkingTheCloseRule(
            IMarkingTheCloseParameters parameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursManager tradingHoursManager,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<MarkingTheCloseRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters?.Window ?? TimeSpan.FromMinutes(30),
                Domain.Scheduling.Rules.MarkingTheClose,
                MarkingTheCloseRuleFactory.Version,
                "Marking The Close",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
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
            if (_parameters.PercentageThresholdDailyVolume != null)
            {
                dailyVolumeBreach = CheckDailyVolumeTraded(marketSecurities);
            }

            VolumeBreach windowVolumeBreach = null;
            if (_parameters.PercentageThresholdWindowVolume != null)
            {
                windowVolumeBreach = CheckWindowVolumeTraded(marketSecurities);
            }

            if ((dailyVolumeBreach == null || !dailyVolumeBreach.HasBreach())
                && (windowVolumeBreach == null || !windowVolumeBreach.HasBreach()))
            {
                _logger.LogInformation($"MarkingTheCloseRule had no breaches for {marketSecurities.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}");
                return;
            }

            var position = new TradePosition(marketSecurities.ToList());
            var breach = new MarkingTheCloseBreach(
                _ruleCtx.SystemProcessOperationContext(),
                _ruleCtx.CorrelationId(),
                _parameters.Window,
                marketSecurities.FirstOrDefault()?.Instrument,
                _latestMarketClosure,
                position,
                _parameters,
                dailyVolumeBreach ?? new VolumeBreach(),
                windowVolumeBreach ?? new VolumeBreach());

            _logger.LogInformation($"MarkingTheCloseRule had a breach for {marketSecurities.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Adding to alert stream.");
            var alertEvent = new UniverseAlertEvent(Domain.Scheduling.Rules.MarkingTheClose, breach, _ruleCtx);
            _alertStream.Add(alertEvent);
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        private VolumeBreach CheckDailyVolumeTraded(
            Stack<Order> securities)
        {
            var marketDataRequest = new MarketDataRequest(
                securities.Peek().Market.MarketIdentifierCode,
                securities.Peek().Instrument.Cfi,
                securities.Peek().Instrument.Identifiers,
                UniverseDateTime.Subtract(WindowSize), // implicitly correct (market closure event trigger)
                UniverseDateTime,
                _ruleCtx?.Id());

            var dataResponse = UniverseEquityInterdayCache.Get(marketDataRequest);

            if (dataResponse.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogInformation($"Marking The Close Rule had missing data for {securities.Peek().Instrument.Identifiers} on {UniverseDateTime}");
                return new VolumeBreach();
            }

            var tradedSecurity = dataResponse.Response;

            var thresholdVolumeTraded = tradedSecurity.DailySummaryTimeBar.DailyVolume.Traded * _parameters.PercentageThresholdDailyVolume;

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

            var marketDataRequest =
                new MarketDataRequest(
                    securities.Peek().Market.MarketIdentifierCode,
                    securities.Peek().Instrument.Cfi,
                    securities.Peek().Instrument.Identifiers,
                    UniverseDateTime.Subtract(WindowSize), // implicitly correct (market closure event trigger)
                    UniverseDateTime,
                    _ruleCtx?.Id());

            // marking the close should not have windows exceeding a few hours
            var marketResult = UniverseEquityIntradayCache.GetMarkets(marketDataRequest);
            if (marketResult.HadMissingData)
            {
                _hadMissingData = true;
                return new VolumeBreach();
            }

            var securityUpdates = marketResult.Response;
            var securityVolume = securityUpdates.Sum(su => su.SpreadTimeBar.Volume.Traded);
            var thresholdVolumeTraded = securityVolume * _parameters.PercentageThresholdWindowVolume;

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
            long marketVolumeTraded)
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

        private decimal? CalculateBuyBreach(long volumeTradedBuy, long marketVolume, bool hasBuyVolumeBreach)
        {
            return hasBuyVolumeBreach
                && volumeTradedBuy > 0
                && marketVolume > 0
                // ReSharper disable RedundantCast
                ? (decimal?)((decimal)volumeTradedBuy / (decimal)marketVolume)
                // ReSharper restore RedundantCast
                    : null;
        }

        private decimal? CalculateSellBreach(long volumeTradedSell, long marketVolume, bool hasSellDailyVolumeBreach)
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
            _logger.LogInformation("Genesis occurred in the Marking The Close Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred in Marking The Close Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred in Marking The Close Rule at {exchange?.MarketClose}");

            _processingMarketClose = true;
            _latestMarketClosure = exchange;
            RunRuleForAllTradingHistoriesInMarket(exchange, exchange?.MarketClose);
            _processingMarketClose = false;
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured in Marking The Close Rule");

            if (_hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                _logger.LogInformation("Marking The Close Rule had missing data at eschaton. Recording to op ctx.");
                var alert = new UniverseAlertEvent(Domain.Scheduling.Rules.MarkingTheClose, null, _ruleCtx, false, true);
                _alertStream.Add(alert);

                _dataRequestSubscriber.SubmitRequest();
                _ruleCtx.EndEvent();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }
        }

        public object Clone()
        {
            var clone = (MarkingTheCloseRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}