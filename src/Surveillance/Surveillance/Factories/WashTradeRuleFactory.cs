using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.WashTrade;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;

namespace Surveillance.Factories
{
    public class WashTradeRuleFactory : IWashTradeRuleFactory
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IWashTradePositionPairer _positionPairer;
        private readonly IWashTradeClustering _clustering;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public static string Version { get; } = Versioner.Version(1, 0);

        public WashTradeRuleFactory(
            ICurrencyConverter currencyConverter,
            IWashTradePositionPairer positionPairer,
            IWashTradeClustering clustering,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            ILogger<WashTradeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _positionPairer = positionPairer ?? throw new ArgumentNullException(nameof(positionPairer));
            _clustering = clustering ?? throw new ArgumentNullException(nameof(clustering));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public IWashTradeRule Build(
            IWashTradeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            if (ruleCtx == null)
            {
                throw new ArgumentNullException(nameof(ruleCtx));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return new WashTradeRule(
                parameters,
                ruleCtx,
                _positionPairer,
                _clustering,
                alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                runMode,
                _logger,
                _tradingHistoryLogger);
        }
    }
}