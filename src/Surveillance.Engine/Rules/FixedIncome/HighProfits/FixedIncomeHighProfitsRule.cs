using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits
{
    public class FixedIncomeHighProfitsRule : BaseUniverseRule, IFixedIncomeHighProfitsRule
    {
        private readonly IHighProfitsRuleFixedIncomeParameters _parameters;
        private readonly IUniverseFixedIncomeOrderFilter _orderFilter;
        private readonly ILogger<FixedIncomeHighProfitsRule> _logger;

        public FixedIncomeHighProfitsRule(
            IHighProfitsRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilter orderFilter,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            ILogger<FixedIncomeHighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Scheduling.Rules.FixedIncomeHighProfits,
                Versioner.Version(1, 0),
                "Fixed Income High Profits Rule",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(_parameters));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunRule called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunRule completed for {UniverseDateTime}");
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunInitialSubmissionRule called at {UniverseDateTime}");



            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunInitialSubmissionRule completed for {UniverseDateTime}");
        }

        protected override void Genesis()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Universe Genesis called at {UniverseDateTime}");



            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Universe Genesis completed for {UniverseDateTime}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Market Open called at {UniverseDateTime} for {exchange?.MarketId}");



            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Market Open completed at {UniverseDateTime} for {exchange?.MarketId}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Market Close called at {UniverseDateTime} for {exchange?.MarketId}");



            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Market Close completed at {UniverseDateTime} for {exchange?.MarketId}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Eschaton called at {UniverseDateTime}");



            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Eschaton completed for {UniverseDateTime}");

        }

        public object Clone()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Clone called at {UniverseDateTime}");

            var clone = (FixedIncomeHighProfitsRule)this.MemberwiseClone();
            clone.BaseClone();

            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} Clone completed for {UniverseDateTime}");
            return clone;
        }
    }
}
