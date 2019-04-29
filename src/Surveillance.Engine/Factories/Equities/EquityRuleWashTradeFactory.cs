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

namespace Surveillance.Engine.Rules.Factories.Equities
{
    public class EquityRuleWashTradeFactory : IEquityRuleWashTradeFactory
    {
        private readonly ICurrencyConverterService _currencyConverterService;
        private readonly IClusteringService _clustering;
        private readonly IUniverseEquityOrderFilterService _orderFilterService;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public static string Version { get; } = Versioner.Version(1, 0);

        public EquityRuleWashTradeFactory(
            ICurrencyConverterService currencyConverterService,
            IClusteringService clustering,
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory factory,
            ILogger<WashTradeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            _clustering = clustering ?? throw new ArgumentNullException(nameof(clustering));
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public IWashTradeRule Build(
            IWashTradeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            if (ruleCtx == null)
            {
                throw new ArgumentNullException(nameof(ruleCtx));
            }

            if (equitiesParameters == null)
            {
                throw new ArgumentNullException(nameof(equitiesParameters));
            }

            return new WashTradeRule(
                equitiesParameters,
                ruleCtx,
                _clustering,
                alertStream,
                _currencyConverterService,
                _orderFilterService,
                _factory,
                runMode,
                _logger,
                _tradingHistoryLogger);
        }
    }
}