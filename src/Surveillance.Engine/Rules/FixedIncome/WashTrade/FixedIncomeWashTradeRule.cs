using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade
{
    public class FixedIncomeWashTradeRule : BaseUniverseRule, IFixedIncomeWashTradeRule
    {
        private readonly IWashTradeRuleFixedIncomeParameters _parameters;
        private readonly IUniverseFixedIncomeOrderFilter _orderFilter;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IClusteringService _clusteringService;
        private readonly ILogger<FixedIncomeWashTradeRule> _logger;

        public FixedIncomeWashTradeRule(
            IWashTradeRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilter orderFilter,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            IUniverseAlertStream alertStream,
            IClusteringService clusteringService,
            ILogger<FixedIncomeWashTradeRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Scheduling.Rules.FixedIncomeWashTrades,
                Versioner.Version(1, 0),
                $"",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _clusteringService = clusteringService ?? throw new ArgumentNullException(nameof(clusteringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            _logger.LogInformation($"RunRule called at {UniverseDateTime}");

            if (_parameters.PerformClusteringPositionAnalysis)
            {

            }

            _logger.LogInformation($"RunRule completed for {UniverseDateTime}");
        }

        private void ClusteringAnalysis(ITradingHistoryStack tradingHistory)
        {
            var activeTrades = tradingHistory.ActiveTradeHistory();

            if (activeTrades.Count == 0)
            {
                return;
            }




        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"RunInitialSubmissionRule called at {UniverseDateTime}");


            _logger.LogInformation($"RunInitialSubmissionRule completed for {UniverseDateTime}");
        }

        protected override void Genesis()
        {
            _logger.LogInformation($"Genesis called at {UniverseDateTime}");


            _logger.LogInformation($"Genesis completed for {UniverseDateTime}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"MarketOpen called at {UniverseDateTime} for {exchange.MarketId}");


            _logger.LogInformation($"MarketOpen completed for {UniverseDateTime} for {exchange.MarketId}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"MarketClose called at {UniverseDateTime} for {exchange.MarketId}");


            _logger.LogInformation($"MarketClose completed for {UniverseDateTime} for {exchange.MarketId}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"EndOfUniverse called at {UniverseDateTime}");

            RuleCtx?.EndEvent();

            _logger.LogInformation($"EndOfUniverse completed for {UniverseDateTime}");
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeWashTradeRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            _logger.LogInformation($"Clone called at {UniverseDateTime}");

            var clone = (FixedIncomeWashTradeRule)this.MemberwiseClone();
            clone.BaseClone();

            _logger.LogInformation($"Clone completed for {UniverseDateTime}");

            return clone;
        }
    }
}
