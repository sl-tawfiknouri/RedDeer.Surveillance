using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Multiverse;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    [TestFixture]
    public class HighProfitRuleFactoryTests
    {
        private IUniversePercentageCompletionLoggerFactory _percentageCompleteFactory;
        private IUniverseEquityOrderFilter _orderFilter;
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
        private ScheduledExecution _scheduledExecution;

        [SetUp]
        public void Setup()
        {
            _percentageCompleteFactory = A.Fake<IUniversePercentageCompletionLoggerFactory>();
            _orderFilter = A.Fake<IUniverseEquityOrderFilter>();
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
        public void Constructor_Null_CostCalculatorFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    null,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_RevenueCalculatorFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    null,
                    _exchangeRateProfitCalculator,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_ExchangeRateProfitCalculator_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    null,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_PercentageComplete_Is_Exceptional()
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
        public void Constructor_Null_OrderFilter_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilter,
                    null,
                    _cacheStrategyFactory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_MarketCacheFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilter,
                    _marketCacheFactory,
                    null,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_CacheStrategyFactory_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    _costCalculatorFactory,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilter,
                    _marketCacheFactory,
                    _cacheStrategyFactory,
                    null,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new EquityRuleHighProfitFactory(
                    null,
                    _revenueCalculatorFactory,
                    _exchangeRateProfitCalculator,
                    _orderFilter,
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
                _orderFilter,
                _marketCacheFactory,
                _cacheStrategyFactory,
                _logger,
                _tradingHistoryLogger);

            var result = factory.Build(_parameters, _ruleCtxStream, _ruleCtxMarket, _alertStream, _dataRequestSubscriber, _scheduledExecution);

            Assert.IsNotNull(result);
        }
    }
}
