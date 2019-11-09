namespace Surveillance.Engine.Rules.Factories.Equities
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
    using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class EquityRuleWashTradeFactory : IEquityRuleWashTradeFactory
    {
        private readonly IClusteringService _clustering;

        private readonly ICurrencyConverterService _currencyConverterService;

        private readonly IUniverseEquityMarketCacheFactory _equityFactory;

        private readonly IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;

        private readonly ILogger _logger;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleWashTradeFactory(
            ICurrencyConverterService currencyConverterService,
            IClusteringService clustering,
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseEquityMarketCacheFactory equityFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeFactory,
            ILogger<WashTradeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._currencyConverterService = currencyConverterService
                                             ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this._clustering = clustering ?? throw new ArgumentNullException(nameof(clustering));
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._equityFactory = equityFactory ?? throw new ArgumentNullException(nameof(equityFactory));
            this._fixedIncomeFactory = fixedIncomeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public static string Version { get; } = Versioner.Version(1, 0);

        public IWashTradeRule Build(
            IWashTradeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            if (ruleCtx == null) throw new ArgumentNullException(nameof(ruleCtx));

            if (equitiesParameters == null) throw new ArgumentNullException(nameof(equitiesParameters));

            return new WashTradeRule(
                equitiesParameters,
                ruleCtx,
                this._clustering,
                alertStream,
                this._currencyConverterService,
                this._orderFilterService,
                this._equityFactory,
                this._fixedIncomeFactory,
                runMode,
                this._logger,
                this._tradingHistoryLogger);
        }
    }
}