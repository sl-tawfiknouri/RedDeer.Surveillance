﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance
{
    public class FixedIncomeHighVolumeIssuanceRule : BaseUniverseRule, IFixedIncomeHighVolumeRule
    {
        private readonly IHighVolumeIssuanceRuleFixedIncomeParameters _parameters;
        private readonly IUniverseFixedIncomeOrderFilterService _orderFilterService;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ILogger<FixedIncomeHighVolumeIssuanceRule> _logger;

        public FixedIncomeHighVolumeIssuanceRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilterService orderFilterService,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            IUniverseAlertStream alertStream,
            ILogger<FixedIncomeHighVolumeIssuanceRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Surveillance.Scheduling.Rules.FixedIncomeHighVolumeIssuance,
                Versioner.Version(1, 0),
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)}",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
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
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunRule called at {UniverseDateTime}");

            

            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunRule completed for {UniverseDateTime}");
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunInitialSubmissionRule called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunInitialSubmissionRule completed for {UniverseDateTime}");
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunOrderFilledEvent called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunOrderFilledEvent completed for {UniverseDateTime}");
        }

        protected override void Genesis()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} Genesis called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} Genesis completed for {UniverseDateTime}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketOpen called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketOpen completed for {UniverseDateTime}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketClose called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketClose completed for {UniverseDateTime}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} EndOfUniverse called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} EndOfUniverse completed for {UniverseDateTime}");
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeHighVolumeIssuanceRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} Clone called at {UniverseDateTime}");

            var clone = (FixedIncomeHighVolumeIssuanceRule)this.MemberwiseClone();
            clone.BaseClone();

            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} Clone completed for {UniverseDateTime}");

            return clone;
        }
    }
}
