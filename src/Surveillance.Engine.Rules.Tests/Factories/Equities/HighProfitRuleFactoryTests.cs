﻿using System;
using Domain.Surveillance.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    [TestFixture]
    public class HighProfitRuleFactoryTests
    {
        private IUniverseEquityOrderFilterService _orderFilterService;
        private ICostCalculatorFactory _costCalculatorFactory;
        private IRevenueCalculatorFactory _revenueCalculatorFactory;
        private IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private IUniverseMarketCacheFactory _marketCacheFactory;
        private IMarketDataCacheStrategyFactory _cacheStrategyFactory;
        private IJudgementService _judgementService;
        private ILogger<HighProfitsRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;
       
        private IHighProfitsRuleEquitiesParameters _equitiesParameters;
        private ISystemProcessOperationRunRuleContext _ruleCtxStream;
        private ISystemProcessOperationRunRuleContext _ruleCtxMarket;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private ScheduledExecution _scheduledExecution;

        [SetUp]
        public void Setup()
        {
            _orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            _costCalculatorFactory = A.Fake<ICostCalculatorFactory>();
            _revenueCalculatorFactory = A.Fake<IRevenueCalculatorFactory>();
            _exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            _marketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _cacheStrategyFactory = A.Fake<IMarketDataCacheStrategyFactory>();
            _judgementService = A.Fake<IJudgementService>();
            _logger = new NullLogger<HighProfitsRule>();
            _tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            _equitiesParameters = A.Fake<IHighProfitsRuleEquitiesParameters>();
            _ruleCtxStream = A.Fake<ISystemProcessOperationRunRuleContext>();
            _ruleCtxMarket = A.Fake<ISystemProcessOperationRunRuleContext>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _scheduledExecution = new ScheduledExecution();
        }

        [Test]
        public void Constructor_Null_CostCalculatorFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    null,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilterService,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_RevenueCalculatorFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    null,
                    _exchangeRateProfitCalculator,
                    _orderFilterService,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_ExchangeRateProfitCalculator_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    null,
                    _orderFilterService,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_PercentageComplete_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    null,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_OrderFilter_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilterService,
                    null,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_MarketCacheFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilterService,
                    _marketCacheFactory,
                    null,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_CacheStrategyFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilterService,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    null,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    null,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilterService,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    null));
        }


        [Test]
        public void Build_Returns_High_Profits_Rule()
        {
             var factory = new EquityRuleHighProfitFactory(
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _orderFilterService,
                _marketCacheFactory,
                _cacheStrategyFactory,
                _logger,
                _tradingHistoryLogger);

            var result =
                factory.Build(
                    _equitiesParameters,
                    _ruleCtxStream, 
                    _ruleCtxMarket,
                    _dataRequestSubscriber,
                    _judgementService,
                    _scheduledExecution);

            Assert.IsNotNull(result);
        }
    }
}
