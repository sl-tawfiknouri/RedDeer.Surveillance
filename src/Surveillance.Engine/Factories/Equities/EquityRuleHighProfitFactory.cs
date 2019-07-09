using System;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities
{
    public class EquityRuleHighProfitFactory : IEquityRuleHighProfitFactory
    {
        private readonly IUniverseEquityOrderFilterService _orderFilterService;
        private readonly ICostCalculatorFactory _costCalculatorFactory;
        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;
        private readonly IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private readonly IUniverseMarketCacheFactory _marketCacheFactory;
        private readonly IMarketDataCacheStrategyFactory _cacheStrategyFactory;
        private readonly IJudgementServiceFactory _judgementServiceFactory;
        private readonly ILogger<HighProfitsRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleHighProfitFactory(
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketDataCacheStrategyFactory cacheStrategyFactory,
            IJudgementServiceFactory judgementServiceFactory,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _costCalculatorFactory = costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            _revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            _exchangeRateProfitCalculator =
                exchangeRateProfitCalculator
                ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            _marketCacheFactory = marketCacheFactory ?? throw new ArgumentNullException(nameof(marketCacheFactory));
            _cacheStrategyFactory = cacheStrategyFactory ?? throw new ArgumentNullException(nameof(cacheStrategyFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _judgementServiceFactory = judgementServiceFactory ?? throw new ArgumentNullException(nameof(judgementServiceFactory));
        }

        public IHighProfitRule Build(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtxStream,
            ISystemProcessOperationRunRuleContext ruleCtxMarket,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ScheduledExecution scheduledExecution)
        {
            var runMode = scheduledExecution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;

            var stream = new HighProfitStreamRule(
                equitiesParameters,
                ruleCtxStream,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _orderFilterService,
                _marketCacheFactory,
                _cacheStrategyFactory,
                dataRequestSubscriber,
                _judgementServiceFactory.Build(),
                runMode,
                _logger,
                _tradingHistoryLogger);

            var marketClosure = new HighProfitMarketClosureRule(
                equitiesParameters,
                ruleCtxMarket,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _orderFilterService,
                _marketCacheFactory,
                _cacheStrategyFactory,
                dataRequestSubscriber,
                _judgementServiceFactory.Build(),
                runMode,
                _logger,
                _tradingHistoryLogger);
               
            return new HighProfitsRule(equitiesParameters, stream, marketClosure, _logger);
        }

        public static string Version => Versioner.Version(3, 0);
    }
}