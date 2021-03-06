﻿namespace Surveillance.Engine.Rules.Factories.Equities
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class EquityRuleMarkingTheCloseFactory : IEquityRuleMarkingTheCloseFactory
    {
        private readonly IUniverseEquityMarketCacheFactory _equityFactory;

        private readonly IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;

        private readonly ILogger<MarkingTheCloseRule> _logger;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private readonly IMarketTradingHoursService _tradingHoursService;

        public EquityRuleMarkingTheCloseFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseEquityMarketCacheFactory equityFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeFactory,
            IMarketTradingHoursService tradingHoursService,
            ILogger<MarkingTheCloseRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._equityFactory = equityFactory ?? throw new ArgumentNullException(nameof(equityFactory));
            this._fixedIncomeFactory = fixedIncomeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeFactory));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public static string Version => Versioner.Version(1, 0);

        public IMarkingTheCloseRule Build(
            IMarkingTheCloseEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            return new MarkingTheCloseRule(
                equitiesParameters,
                alertStream,
                ruleCtx,
                this._orderFilterService,
                this._equityFactory,
                this._fixedIncomeFactory,
                this._tradingHoursService,
                dataRequestSubscriber,
                runMode,
                this._logger,
                this._tradingHistoryLogger);
        }
    }
}