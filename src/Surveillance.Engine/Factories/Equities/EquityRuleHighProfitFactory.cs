﻿using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class EquityRuleHighProfitFactory : IEquityRuleHighProfitFactory
    {
        private readonly IEquityMarketDataCacheStrategyFactory _cacheStrategyFactory;

        private readonly ICostCalculatorFactory _costCalculatorFactory;

        private readonly IExchangeRateProfitCalculator _exchangeRateProfitCalculator;

        private readonly ILogger<HighProfitsRule> _logger;

        private readonly IUniverseEquityMarketCacheFactory _equityMarketCacheFactory;

        private readonly IUniverseFixedIncomeMarketCacheFactory _fixedIncomeMarketCacheFactory;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;

        private readonly ICurrencyConverterService _currencyConversionService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleHighProfitFactory(
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            IEquityMarketDataCacheStrategyFactory cacheStrategyFactory,
            ICurrencyConverterService currencyConversionService,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._costCalculatorFactory =
                costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            this._revenueCalculatorFactory = revenueCalculatorFactory
                                             ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            this._exchangeRateProfitCalculator = exchangeRateProfitCalculator
                                                 ?? throw new ArgumentNullException(
                                                     nameof(exchangeRateProfitCalculator));
            this._equityMarketCacheFactory =
                equityMarketCacheFactory ?? throw new ArgumentNullException(nameof(equityMarketCacheFactory));
            this._fixedIncomeMarketCacheFactory =
                fixedIncomeMarketCacheFactory ?? throw new ArgumentNullException(nameof(fixedIncomeMarketCacheFactory));
            this._cacheStrategyFactory =
                cacheStrategyFactory ?? throw new ArgumentNullException(nameof(cacheStrategyFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._currencyConversionService = currencyConversionService ?? throw new ArgumentNullException(nameof(currencyConversionService));
        }

        public static string Version => Versioner.Version(3, 0);

        public IHighProfitRule Build(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtxStream,
            ISystemProcessOperationRunRuleContext ruleCtxMarket,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            ScheduledExecution scheduledExecution)
        {
            var runMode = scheduledExecution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;

            var stream = new HighProfitStreamRule(
                equitiesParameters,
                ruleCtxStream,
                this._costCalculatorFactory,
                this._revenueCalculatorFactory,
                this._exchangeRateProfitCalculator,
                this._orderFilterService,
                this._equityMarketCacheFactory,
                this._fixedIncomeMarketCacheFactory,
                this._cacheStrategyFactory,
                dataRequestSubscriber,
                judgementService,
                this._currencyConversionService,
                runMode,
                this._logger,
                this._tradingHistoryLogger);

            var marketClosure = new HighProfitMarketClosureRule(
                equitiesParameters,
                ruleCtxMarket,
                this._costCalculatorFactory,
                this._revenueCalculatorFactory,
                this._exchangeRateProfitCalculator,
                this._orderFilterService,
                this._equityMarketCacheFactory,
                this._fixedIncomeMarketCacheFactory,
                this._cacheStrategyFactory,
                dataRequestSubscriber,
                judgementService,
                this._currencyConversionService,
                runMode,
                this._logger,
                this._tradingHistoryLogger);

            return new HighProfitsRule(equitiesParameters, stream, marketClosure, this._logger);
        }
    }
}