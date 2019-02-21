﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume
{
    public class FixedIncomeHighVolumeRule : BaseUniverseRule, IFixedIncomeHighVolumeRule
    {
        private readonly IUniverseFixedIncomeOrderFilter _orderFilter;
        private readonly ILogger<FixedIncomeHighVolumeRule> _logger;

        public FixedIncomeHighVolumeRule(
            TimeSpan windowSize,
            IUniverseFixedIncomeOrderFilter orderFilter,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            ILogger<FixedIncomeHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                windowSize,
                Domain.Scheduling.Rules.FixedIncomeHighVolume,
                Versioner.Version(1, 0),
                $"{nameof(FixedIncomeHighVolumeRule)}",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} RunRule called at {UniverseDateTime}");

            

            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} RunRule completed for {UniverseDateTime}");
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} RunInitialSubmissionRule called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} RunInitialSubmissionRule completed for {UniverseDateTime}");
        }

        protected override void Genesis()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} Genesis called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} Genesis completed for {UniverseDateTime}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} MarketOpen called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} MarketOpen completed for {UniverseDateTime}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} MarketClose called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} MarketClose completed for {UniverseDateTime}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} EndOfUniverse called at {UniverseDateTime}");


            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} EndOfUniverse completed for {UniverseDateTime}");
        }

        public object Clone()
        {
            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} Clone called at {UniverseDateTime}");

            var clone = (FixedIncomeHighVolumeRule)this.MemberwiseClone();
            clone.BaseClone();

            _logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} Clone completed for {UniverseDateTime}");

            return clone;
        }
    }
}
