using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
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
        private readonly IUniverseFixedIncomeOrderFilterService _orderFilterService;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ILogger<FixedIncomeHighProfitsRule> _logger;

        public FixedIncomeHighProfitsRule(
            IHighProfitsRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilterService orderFilterService,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            IUniverseAlertStream alertStream,
            ILogger<FixedIncomeHighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.FixedIncomeHighProfits,
                Versioner.Version(1, 0),
                "Fixed Income High Profits Rule",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(_parameters));
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilterService.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunRule called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunRule completed for {UniverseDateTime}");
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunInitialSubmissionRule called at {UniverseDateTime}");



            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunInitialSubmissionRule completed for {UniverseDateTime}");
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunOrderFilledEvent called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighProfitsRule)} RunOrderFilledEvent completed for {UniverseDateTime}");
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

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeHighProfitsRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }
    }
}
