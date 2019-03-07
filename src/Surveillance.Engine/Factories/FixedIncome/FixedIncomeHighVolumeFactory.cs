﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    public class FixedIncomeHighVolumeFactory : IFixedIncomeHighVolumeFactory
    {
        private readonly IUniverseFixedIncomeOrderFilterService _filterService;
        private readonly IUniverseMarketCacheFactory _marketCacheFactory;

        private readonly ILogger<FixedIncomeHighVolumeIssuanceRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingLogger;

        public FixedIncomeHighVolumeFactory(
            IUniverseFixedIncomeOrderFilterService filterService,
            IUniverseMarketCacheFactory marketCacheFactory,
            ILogger<FixedIncomeHighVolumeIssuanceRule> logger,
            ILogger<TradingHistoryStack> tradingLogger)
        {
            _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            _marketCacheFactory = marketCacheFactory ?? throw new ArgumentNullException(nameof(_marketCacheFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingLogger = tradingLogger ?? throw new ArgumentNullException(nameof(tradingLogger));
        }

        public IFixedIncomeHighVolumeRule BuildRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new FixedIncomeHighVolumeIssuanceRule(
                parameters,
                _filterService,
                opCtx,
                _marketCacheFactory,
                runMode,
                alertStream,
                _logger,
                _tradingLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
