using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;
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
        private readonly ILogger<FixedIncomeWashTradeRule> _logger;

        public FixedIncomeWashTradeRule(
            TimeSpan windowSize,
            IWashTradeRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilter orderFilter,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            ILogger<FixedIncomeWashTradeRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                windowSize,
                Domain.Scheduling.Rules.FixedIncomeWashTrades,
                Versioner.Version(1, 0),
                $"{nameof(FixedIncomeWashTradeRule)}",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} RunRule called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} RunRule completed for {UniverseDateTime}");
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} RunInitialSubmissionRule called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} RunInitialSubmissionRule completed for {UniverseDateTime}");
        }

        protected override void Genesis()
        {
            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} Genesis called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} Genesis completed for {UniverseDateTime}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} MarketOpen called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} MarketOpen completed for {UniverseDateTime}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} MarketClose called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} MarketClose completed for {UniverseDateTime}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} EndOfUniverse called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} EndOfUniverse completed for {UniverseDateTime}");
        }

        public object Clone()
        {
            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} Clone called at {UniverseDateTime}");

            var clone = (FixedIncomeWashTradeRule)this.MemberwiseClone();
            clone.BaseClone();

            _logger.LogInformation($"{nameof(FixedIncomeWashTradeRule)} Clone completed for {UniverseDateTime}");

            return clone;
        }
    }
}
