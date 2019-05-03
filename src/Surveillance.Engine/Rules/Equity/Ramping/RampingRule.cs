﻿using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingRule : BaseUniverseRule, IRampingRule
    {
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private readonly IRampingRuleEquitiesParameters _rampingParameters;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IRampingAnalyser _rampingAnalyser;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;

        private readonly ILogger _logger;
        private bool _hadMissingData = false;

        public RampingRule(
            IRampingRuleEquitiesParameters rampingParameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            IUniverseOrderFilter orderFilter,
            RuleRunMode runMode,
            IRampingAnalyser rampingAnalyser,
            IMarketTradingHoursService tradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                rampingParameters?.WindowSize ?? TimeSpan.FromDays(7),
                Domain.Surveillance.Scheduling.Rules.Ramping,
                EquityRuleRampingFactory.Version,
                "Ramping Rule",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _rampingParameters = rampingParameters ?? throw new ArgumentNullException(nameof(rampingParameters));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _rampingAnalyser = rampingAnalyser ?? throw new ArgumentNullException(nameof(rampingAnalyser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
        }

        public IFactorValue OrganisationFactorValue { get; set; }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            if (!ExceedsTradingFrequencyThreshold())
            {
                // LOG THEN EXIT
                _logger.LogInformation($"Trading Frequency of {_rampingParameters.ThresholdOrdersExecutedInWindow} was not exceeded. Returning.");
                return;
            }

            if (!ExceedsTradingVolumeInWindowThreshold())
            {
                // LOG THEN EXIT
                _logger.LogInformation($"Trading Volume of {_rampingParameters.ThresholdVolumePercentageWindow} was not exceeded. Returning.");
                return;
            }

            var lastTrade = tradeWindow.Peek();
            var tradingHours = _tradingHoursService.GetTradingHoursForMic(lastTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {lastTrade.Market?.MarketIdentifierCode}");
            }

            var marketDataRequest = new MarketDataRequest(
                lastTrade.Market?.MarketIdentifierCode,
                lastTrade.Instrument.Cfi,
                lastTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(WindowSize)),
                tradingHours.ClosingInUtcForDay(UniverseDateTime),
                _ruleCtx?.Id());

            var marketData = UniverseEquityIntradayCache.GetMarkets(marketDataRequest);

            if (marketData.HadMissingData)
            {
                _hadMissingData = true;
                _logger.LogWarning($"Missing data for {marketDataRequest}.");
                return;
            }
            
            var rampingAnalysis = _rampingAnalyser.Analyse(tradeWindow, marketData.Response);
            var breachDetected = rampingAnalysis.HasRampingStrategy();

            if (!breachDetected)
            {
                // LOG THEN EXIT
                _logger.LogInformation($"A rule breach was not detected. Returning.");
                return;
            }

            var tradePosition = new TradePosition(tradeWindow.ToList());

            var breach =
                new RampingRuleBreach(
                    WindowSize,
                    tradePosition,
                    lastTrade.Instrument,
                    _rampingParameters.Id,
                    _ruleCtx.Id(),
                    _ruleCtx.CorrelationId(),
                    OrganisationFactorValue,
                    rampingAnalysis);

            _logger.LogInformation($"RunRule has breached parameter conditions for {lastTrade?.Instrument?.Identifiers}. Adding message to alert stream.");
            var message = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Ramping, breach, _ruleCtx);
            _alertStream.Add(message);
        }

        private bool ExceedsTradingFrequencyThreshold()
        {
            if (_rampingParameters?.ThresholdOrdersExecutedInWindow == null)
            {
                return true;
            }

            return false;
        }

        private bool ExceedsTradingVolumeInWindowThreshold()
        {
            if (_rampingParameters?.ThresholdVolumePercentageWindow == null)
            {
                return true;
            }

            return false;
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
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
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured");

            if (_hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                // delete event
                var alert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.Ramping, null, _ruleCtx, false, true);
                _alertStream.Add(alert);

                _dataRequestSubscriber.SubmitRequest();
            }

            _ruleCtx?.EndEvent();
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (RampingRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (RampingRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
