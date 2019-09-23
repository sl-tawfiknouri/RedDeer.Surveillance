namespace Surveillance.Engine.Rules.Factories.Equities
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class EquityRuleRampingFactory : IEquityRuleRampingFactory
    {
        private readonly IUniverseMarketCacheFactory _factory;

        private readonly ILogger<IRampingRule> _logger;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly IRampingAnalyser _rampingAnalyser;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private readonly IMarketTradingHoursService _tradingHoursService;

        public EquityRuleRampingFactory(
            IRampingAnalyser rampingAnalyser,
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursService tradingHoursService,
            ILogger<IRampingRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._rampingAnalyser = rampingAnalyser ?? throw new ArgumentNullException(nameof(rampingAnalyser));
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
        }

        public static string Version => Versioner.Version(1, 0);

        public IRampingRule Build(
            IRampingRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            return new RampingRule(
                equitiesParameters,
                alertStream,
                ruleCtx,
                this._factory,
                this._orderFilterService,
                runMode,
                this._rampingAnalyser,
                this._tradingHoursService,
                dataRequestSubscriber,
                this._logger,
                this._tradingHistoryLogger);
        }
    }
}