using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;
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
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IRampingRuleEquitiesParameters _rampingParameters;
        private readonly ITimeSeriesTrendClassifier _trendClassifier;
        private readonly IPriceImpactClassifier _priceImpactClassifier;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly ILogger _logger;

        public RampingRule(
            IRampingRuleEquitiesParameters rampingParameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            IUniverseOrderFilter orderFilter,
            RuleRunMode runMode,
            ITimeSeriesTrendClassifier trendClassifier,
            IPriceImpactClassifier priceImpactClassifier,
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
            _trendClassifier = trendClassifier ?? throw new ArgumentNullException(nameof(trendClassifier));
            _priceImpactClassifier = priceImpactClassifier ?? throw new ArgumentNullException(nameof(priceImpactClassifier));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            var pricingTrends = _trendClassifier.Classify(lastTrade.Instrument, WindowSize, UniverseDateTime);

            if (pricingTrends == null
                || !pricingTrends.Any())
            {
                // LOG THEN EXIT
                _logger.LogInformation($"Pricing trends unable to calculate on {UniverseDateTime} for window size {WindowSize} with instrument {lastTrade.Instrument}.");
                return;
            }

            // var weightedVolumePriceImpact = _priceImpactClassifier.ClassifyByWeightedVolume(tradeWindow);
            var tradeCountPriceImpact = _priceImpactClassifier.ClassifyByTradeCount(tradeWindow);

            // now add together the price impact scalar with the price trend vector...lets just make it a vector now
            

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
