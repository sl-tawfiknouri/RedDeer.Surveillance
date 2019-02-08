using System;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Multiverse;
using Surveillance.Universe.Subscribers.Interfaces;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class HighProfitRuleFactoryTests
    {
        private IUniversePercentageCompletionLoggerFactory _percentageCompleteFactory;
        private IUniverseOrderFilter _orderFilter;
        private IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;
        private ICostCalculatorFactory _costCalculatorFactory;
        private IRevenueCalculatorFactory _revenueCalculatorFactory;
        private IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private IUniverseMarketCacheFactory _marketCacheFactory;
        private IMarketDataCacheStrategyFactory _cacheStrategyFactory;
        private ILogger<HighProfitsRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;
        private ILogger<MarketCloseMultiverseTransformer> _transformerLogger;


        private IHighProfitsRuleParameters _parameters;
        private ISystemProcessOperationRunRuleContext _ruleCtxStream;
        private ISystemProcessOperationRunRuleContext _ruleCtxMarket;
        private IUniverseAlertStream _alertStream;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private DomainV2.Scheduling.ScheduledExecution _scheduledExecution;

        [SetUp]
        public void Setup()
        {
            _percentageCompleteFactory = A.Fake<IUniversePercentageCompletionLoggerFactory>();
            _orderFilter = A.Fake<IUniverseOrderFilter>();
            _unsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
            _costCalculatorFactory = A.Fake<ICostCalculatorFactory>();
            _revenueCalculatorFactory = A.Fake<IRevenueCalculatorFactory>();
            _exchangeRateProfitCalculator = A.Fake<IExchangeRateProfitCalculator>();
            _marketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _cacheStrategyFactory = A.Fake<IMarketDataCacheStrategyFactory>();
            _logger = new NullLogger<HighProfitsRule>();
            _tradingHistoryLogger = new NullLogger<TradingHistoryStack>();
            _transformerLogger = new NullLogger<MarketCloseMultiverseTransformer>();

            _parameters = A.Fake<IHighProfitsRuleParameters>();
            _ruleCtxStream = A.Fake<ISystemProcessOperationRunRuleContext>();
            _ruleCtxMarket = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _scheduledExecution = new ScheduledExecution();
        }

        [Test]
        public void Constructor_Null_UnsubscriberFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() => 
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    null,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_CostCalculatorFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    null,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_RevenueCalculatorFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    null,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_ExchangeRateProfitCalculator_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    null,
                    _percentageCompleteFactory,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_PercentageComplete_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    null,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_OrderFilter_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    null,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_MarketCacheFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    _orderFilter,
                    null,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_CacheStrategyFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    _orderFilter,
                    _marketCacheFactory,
                    null,
                    _logger,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    null,
                    _tradingHistoryLogger,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_TradingHistoryLogger_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    null,
                    _transformerLogger));
        }

        [Test]
        public void Constructor_Null_TransformerLogger_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new HighProfitRuleFactory(
                    _unsubscriberFactory,
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _percentageCompleteFactory,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger,
                    null));
        }

        [Test]
        public void Build_Returns_High_Profits_Rule()
        {
             var factory = new HighProfitRuleFactory(
                _unsubscriberFactory,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _percentageCompleteFactory,
                _orderFilter,
                _marketCacheFactory,
                _cacheStrategyFactory,
                _logger,
                _tradingHistoryLogger,
                _transformerLogger);

            var result = factory.Build(_parameters, _ruleCtxStream, _ruleCtxMarket, _alertStream, _dataRequestSubscriber, _scheduledExecution);

            Assert.IsNotNull(result);
        }
    }
}
