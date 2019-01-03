﻿using System;
using DomainV2.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Multiverse;
using Surveillance.Universe.Subscribers.Interfaces;

namespace Surveillance.Factories
{
    public class HighProfitRuleFactory : IHighProfitRuleFactory
    {
        private readonly IUniversePercentageCompletionLoggerFactory _percentageCompleteFactory;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;
        private readonly ICostCalculatorFactory _costCalculatorFactory;
        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;
        private readonly IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private readonly IUniverseMarketCacheFactory _marketCacheFactory;
        private readonly IDataRequestMessageSender _messageSender;
        private readonly ILogger<HighProfitsRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;
        private readonly ILogger<MarketCloseMultiverseTransformer> _transformerLogger;

        public HighProfitRuleFactory(
            IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniversePercentageCompletionLoggerFactory percentageCompleteFactory,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            IDataRequestMessageSender messageSender,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger, 
            ILogger<MarketCloseMultiverseTransformer> transformerLogger)
        {
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _costCalculatorFactory = costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            _revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            _exchangeRateProfitCalculator =
                exchangeRateProfitCalculator
                ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            _marketCacheFactory = marketCacheFactory ?? throw new ArgumentNullException(nameof(marketCacheFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
            _transformerLogger = transformerLogger ?? throw new ArgumentNullException(nameof(transformerLogger));
            _percentageCompleteFactory = percentageCompleteFactory ?? throw new ArgumentNullException(nameof(percentageCompleteFactory));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
        }

        public IHighProfitRule Build(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtxStream,
            ISystemProcessOperationRunRuleContext ruleCtxMarket,
            IUniverseAlertStream alertStream,
            DomainV2.Scheduling.ScheduledExecution scheduledExecution)
        {
            var runMode = scheduledExecution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;

            var stream = new HighProfitStreamRule(
                parameters,
                ruleCtxStream,
                alertStream,
                false,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _orderFilter,
                _marketCacheFactory,
                _messageSender,
                runMode,
                _logger,
                _tradingHistoryLogger);

            var marketClosure = new HighProfitMarketClosureRule(
                parameters,
                ruleCtxMarket,
                alertStream,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _orderFilter,
                _marketCacheFactory,
                _messageSender,
                runMode,
                _logger,
                _tradingHistoryLogger);

            var multiverseTransformer = new MarketCloseMultiverseTransformer(_unsubscriberFactory, _transformerLogger);
            multiverseTransformer.Subscribe(marketClosure);
            var percentageCompletionLogger = _percentageCompleteFactory.Build();
            percentageCompletionLogger.InitiateTimeLogger(scheduledExecution);
            multiverseTransformer.Subscribe(percentageCompletionLogger);
                
            return new HighProfitsRule(stream, multiverseTransformer, _logger);
        }

        public static string Version => Versioner.Version(2, 0);
    }
}