namespace Surveillance.Engine.Rules.Factories.Equities
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.Layering;
    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class EquityRuleLayeringFactory : IEquityRuleLayeringFactory
    {
        private readonly IUniverseEquityMarketCacheFactory _equityFactory;

        private readonly IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;

        private readonly ILogger<EquityRuleLayeringFactory> _logger;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private readonly IMarketTradingHoursService _tradingHoursService;

        public EquityRuleLayeringFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IMarketTradingHoursService tradingHoursService,
            IUniverseEquityMarketCacheFactory equityFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeFactory,
            ILogger<EquityRuleLayeringFactory> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._equityFactory = equityFactory ?? throw new ArgumentNullException(nameof(equityFactory));
            this._fixedIncomeFactory = fixedIncomeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public static string Version => Versioner.Version(1, 0);

        public ILayeringRule Build(
            ILayeringRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new LayeringRule(
                equitiesParameters,
                alertStream,
                this._orderFilterService,
                this._logger,
                this._equityFactory,
                this._fixedIncomeFactory,
                this._tradingHoursService,
                ruleCtx,
                runMode,
                this._tradingHistoryLogger);
        }
    }
}