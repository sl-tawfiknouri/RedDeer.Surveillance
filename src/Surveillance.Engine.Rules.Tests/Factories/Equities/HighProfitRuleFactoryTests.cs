using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    using System;

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
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    [TestFixture]
    public class HighProfitRuleFactoryTests
    {
        private IEquityMarketDataCacheStrategyFactory _cacheStrategyFactory;

        private ICostCalculatorFactory _costCalculatorFactory;

        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private IHighProfitsRuleEquitiesParameters _equitiesParameters;

        private IExchangeRateProfitCalculator _exchangeRateProfitCalculator;

        private IJudgementService _judgementService;

        private ILogger<HighProfitsRule> _logger;

        private IUniverseEquityMarketCacheFactory _equityMarketCacheFactory;

        private IUniverseFixedIncomeMarketCacheFactory _fixedIncomeMarketCacheFactory;

        private IUniverseEquityOrderFilterService _orderFilterService;

        private IRevenueCalculatorFactory _revenueCalculatorFactory;

        private ISystemProcessOperationRunRuleContext _ruleCtxMarket;

        private ISystemProcessOperationRunRuleContext _ruleCtxStream;

        private ScheduledExecution _scheduledExecution;

        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        [Test]
        public void Build_Returns_High_Profits_Rule()
        {
            var factory = new EquityRuleHighProfitFactory(
                this._costCalculatorFactory,
                this._revenueCalculatorFactory,
                this._exchangeRateProfitCalculator,
                this._orderFilterService,
                this._equityMarketCacheFactory,
                this._fixedIncomeMarketCacheFactory,
                this._cacheStrategyFactory,
                this._logger,
                this._tradingHistoryLogger);

            var result = factory.Build(
                this._equitiesParameters,
                this._ruleCtxStream,
                this._ruleCtxMarket,
                this._dataRequestSubscriber,
                this._judgementService,
                this._scheduledExecution);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Constructor_Null_CacheStrategyFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new EquityRuleHighProfitFactory(
                        this._costCalculatorFactory,
                        this._revenueCalculatorFactory,
                        this._exchangeRateProfitCalculator,
                        this._orderFilterService,
                        this._equityMarketCacheFactory,
                        this._fixedIncomeMarketCacheFactory,
                        this._cacheStrategyFactory,
                        null,
                        this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_CostCalculatorFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new EquityRuleHighProfitFactory(
                        null,
                        this._revenueCalculatorFactory,
                        this._exchangeRateProfitCalculator,
                        this._orderFilterService,
                        this._equityMarketCacheFactory,
                        this._fixedIncomeMarketCacheFactory,
                        this._cacheStrategyFactory,
                        this._logger,
                        this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_ExchangeRateProfitCalculator_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new EquityRuleHighProfitFactory(
                        this._costCalculatorFactory,
                        this._revenueCalculatorFactory,
                        null,
                        this._orderFilterService,
                        this._equityMarketCacheFactory,
                        this._fixedIncomeMarketCacheFactory,
                        this._cacheStrategyFactory,
                        this._logger,
                        this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new EquityRuleHighProfitFactory(
                        null,
                        this._revenueCalculatorFactory,
                        this._exchangeRateProfitCalculator,
                        this._orderFilterService,
                        this._equityMarketCacheFactory,
                        this._fixedIncomeMarketCacheFactory,
                        this._cacheStrategyFactory,
                        this._logger,
                        null));
        }

        [Test]
        public void Constructor_Null_MarketCacheFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new EquityRuleHighProfitFactory(
                        this._costCalculatorFactory,
                        this._revenueCalculatorFactory,
                        this._exchangeRateProfitCalculator,
                        this._orderFilterService,
                        this._equityMarketCacheFactory,
                        this._fixedIncomeMarketCacheFactory,
                        null,
                        this._logger,
                        this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_OrderFilter_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new EquityRuleHighProfitFactory(
                        this._costCalculatorFactory,
                        this._revenueCalculatorFactory,
                        this._exchangeRateProfitCalculator,
                        this._orderFilterService,
                        null,
                        null,
                        this._cacheStrategyFactory,
                        this._logger,
                        this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_PercentageComplete_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new EquityRuleHighProfitFactory(
                        this._costCalculatorFactory,
                        this._revenueCalculatorFactory,
                        this._exchangeRateProfitCalculator,
                        null,
                        this._equityMarketCacheFactory,
                        this._fixedIncomeMarketCacheFactory,
                        this._cacheStrategyFactory,
                        this._logger,
                        this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_RevenueCalculatorFactory_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(
                () =>

                    // ReSharper disable once ObjectCreationAsStatement
                    new EquityRuleHighProfitFactory(
                        this._costCalculatorFactory,
                        null,
                        this._exchangeRateProfitCalculator,
                        this._orderFilterService,
                        this._equityMarketCacheFactory,
                        this._fixedIncomeMarketCacheFactory,
                        this._cacheStrategyFactory,
                        this._logger,
                        this._tradingHistoryLogger));
        }

        [SetUp]
        public void Setup()
        {
            this._orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._costCalculatorFactory = A.Fake<ICostCalculatorFactory>();
            this._revenueCalculatorFactory = A.Fake<IRevenueCalculatorFactory>();
            this._exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            this._equityMarketCacheFactory = A.Fake<IUniverseEquityMarketCacheFactory>();
            this._fixedIncomeMarketCacheFactory = A.Fake<IUniverseFixedIncomeMarketCacheFactory>();
            this._cacheStrategyFactory = A.Fake<IEquityMarketDataCacheStrategyFactory>();
            this._judgementService = A.Fake<IJudgementService>();
            this._logger = new NullLogger<HighProfitsRule>();
            this._tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            this._equitiesParameters = A.Fake<IHighProfitsRuleEquitiesParameters>();
            this._ruleCtxStream = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._ruleCtxMarket = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            this._scheduledExecution = new ScheduledExecution();
        }
    }
}