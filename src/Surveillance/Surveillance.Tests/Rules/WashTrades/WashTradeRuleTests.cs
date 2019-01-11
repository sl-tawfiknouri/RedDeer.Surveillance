using System;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using NUnit.Framework;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.WashTrade;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Tests.Helpers;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Tests.Rules.WashTrades
{
    [TestFixture]
    public class WashTradeRuleTests
    {

        private ICurrencyConverter _currencyConverter;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;
        private IWashTradeClustering _clustering;
        private IWashTradePositionPairer _positionPairer;
        private IWashTradeRuleParameters _parameters;
        private IUniverseOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private ILogger _logger;

        private IRuleRunDataRequestRepository _ruleRunRepository;
        private IStubRuleRunDataRequestRepository _stubRuleRunRepository;
        private ILogger<UniverseMarketCacheFactory> _loggerCache;
        private ILogger<TradingHistoryStack> _tradingLogger;

        [SetUp]
        public void Setup()
        {
            _currencyConverter = A.Fake<ICurrencyConverter>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _clustering = new WashTradeClustering();
            _positionPairer = A.Fake<IWashTradePositionPairer>();
            _parameters = A.Fake<IWashTradeRuleParameters>();
            _logger = A.Fake<ILogger>();
            _ruleRunRepository = A.Fake<IRuleRunDataRequestRepository>();
            _stubRuleRunRepository = A.Fake<IStubRuleRunDataRequestRepository>();
            _loggerCache = A.Fake<ILogger<UniverseMarketCacheFactory>>();
            _tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _orderFilter = A.Fake<IUniverseOrderFilter>();
            _factory = new UniverseMarketCacheFactory(_stubRuleRunRepository, _ruleRunRepository, _loggerCache);
            A.CallTo(() => _orderFilter.Filter(A<IUniverseEvent>.Ignored)).ReturnsLazily(i => (IUniverseEvent)i.Arguments[0]);

            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);
            A.CallTo(() => _parameters.ClusteringPercentageValueDifferenceThreshold).Returns(0.05m);
        }

        [Test]
        public void Clustering_DontPerformAnalysis_ReturnsNoBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(false);


            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

            var result = rule.ClusteringTrades(null);

            Assert.AreEqual(result.ClusteringPositionBreach, false);
        }

        [Test]
        public void Clustering_NullActiveTrades_ReturnsNoBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

           var result = rule.ClusteringTrades(null);

            Assert.AreEqual(result.ClusteringPositionBreach, false);
        }

        [Test]
        public void Clustering_NullClusterResponse_ReturnsNoBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

            var trades = new List<Order> {new Order().Random()};
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, false);
        }

        [Test]
        public void Clustering_OneClusterExpected_ReturnsNoBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);
            
            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

            var tr1 = new Order().Random(19);
            var tr2 = new Order().Random(21);

            tr1.OrderPosition = OrderPositions.BUY;
            tr2.OrderPosition = OrderPositions.SELL;

            tr1.OrderAveragePrice = tr2.OrderAveragePrice;
            tr1.OrderFilledVolume = 2000;
            tr2.OrderFilledVolume = 1000;

            var trades = new List<Order> { tr1, tr2 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, false);
        }

        [Test]
        public void Clustering_OneClusterExpectedWithinValueRange_ReturnsOneBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

            var tr1 = new Order().Random(19);
            var tr2 = new Order().Random(21);

            tr1.OrderPosition = OrderPositions.BUY;
            tr2.OrderPosition = OrderPositions.SELL;

            tr1.OrderAveragePrice = tr2.OrderAveragePrice;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;

            var trades = new List<Order> { tr1, tr2 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.AreEqual(result.AmountOfBreachingClusters, 1);
        }

        [Test]
        public void Clustering_TwoClusterExpectedWithOneWithinValueRange_ReturnsOneBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

            var tr1 = new Order().Random(21);
            var tr2 = new Order().Random(21);

            var tr3 = new Order().Random(10);
            var tr4 = new Order().Random(9);

            tr1.OrderPosition = OrderPositions.BUY;
            tr2.OrderPosition = OrderPositions.SELL;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;
            tr1.OrderFilledDate = DateTime.UtcNow;
            tr2.OrderFilledDate = DateTime.UtcNow;

            tr3.OrderPosition = OrderPositions.BUY;
            tr4.OrderPosition = OrderPositions.SELL;
            tr4.OrderFilledVolume = 950;
            tr3.OrderFilledVolume = 1500;
            tr3.OrderFilledDate = DateTime.UtcNow.AddMinutes(5);
            tr4.OrderFilledDate = DateTime.UtcNow.AddMinutes(5);

            var trades = new List<Order> { tr1, tr2, tr3, tr4 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.AreEqual(result.AmountOfBreachingClusters, 1);
        }

        [Test]
        public void Clustering_TwoClusterExpectedWithTwoWithinValueRange_ReturnsOneBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

            var tr1 = new Order().Random(21);
            var tr2 = new Order().Random(21);

            var tr3 = new Order().Random(100);
            var tr4 = new Order().Random(101);

            tr1.OrderPosition = OrderPositions.BUY;
            tr2.OrderPosition = OrderPositions.SELL;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;

            tr3.OrderPosition = OrderPositions.BUY;
            tr4.OrderPosition = OrderPositions.SELL;
            tr4.OrderFilledVolume = 1500;
            tr3.OrderFilledVolume = 1500;

            var trades = new List<Order> { tr1, tr2, tr3, tr4 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.AreEqual(result.AmountOfBreachingClusters, 2);
        }

        [Test]
        public void Clustering_FourClusterExpectedWithTwoWithinValueRange_ReturnsOneBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

            var tr1 = new Order().Random(21);
            var tr2 = new Order().Random(21);

            var tr3 = new Order().Random(100);
            var tr4 = new Order().Random(101);

            var tr5 = new Order().Random(50);
            var tr6 = new Order().Random(70);

            tr1.OrderPosition = OrderPositions.BUY;
            tr2.OrderPosition = OrderPositions.SELL;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;
            tr1.OrderFilledDate = DateTime.UtcNow.AddMinutes(5);
            tr2.OrderFilledDate = DateTime.UtcNow.AddMinutes(5);

            tr3.OrderPosition = OrderPositions.BUY;
            tr4.OrderPosition = OrderPositions.SELL;
            tr4.OrderFilledVolume = 1500;
            tr3.OrderFilledVolume = 1500;
            tr3.OrderFilledDate = DateTime.UtcNow.AddMinutes(10);
            tr4.OrderFilledDate = DateTime.UtcNow.AddMinutes(10);

            tr5.OrderPosition = OrderPositions.BUY;
            tr6.OrderPosition = OrderPositions.SELL;
            tr6.OrderFilledVolume = 1500;
            tr5.OrderFilledVolume = 1500;
            tr5.OrderFilledDate = DateTime.UtcNow.AddMinutes(15);
            tr6.OrderFilledDate = DateTime.UtcNow.AddMinutes(15);

            var trades = new List<Order> { tr1, tr2, tr3, tr4, tr5, tr6 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.IsTrue(result.AmountOfBreachingClusters < 3);
        }

        [Test]
        [Explicit]
        public void Clustering_FourClusterExpectedWithOnInValueAndNumberOfTradesRange_ReturnsOneBreach()
        {
            A.CallTo(() => _parameters.PerformClusteringPositionAnalysis).Returns(true);
            A.CallTo(() => _parameters.ClusteringPositionMinimumNumberOfTrades).Returns(4);

            var rule = new WashTradeRule(
                _parameters,
                _ruleCtx,
                _positionPairer,
                _clustering,
                _alertStream,
                _currencyConverter,
                _orderFilter,
                _factory,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);

            var tr1 = new Order().Random(21);
            var tr2 = new Order().Random(21);
            var tr11 = new Order().Random(21);
            var tr22 = new Order().Random(21);


            var tr3 = new Order().Random(100);
            var tr4 = new Order().Random(101);

            var tr5 = new Order().Random(50);
            var tr6 = new Order().Random(70);

            tr1.OrderPosition = OrderPositions.BUY;
            tr2.OrderPosition = OrderPositions.SELL;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;
            tr1.OrderFilledDate = DateTime.UtcNow.AddMinutes(5);
            tr2.OrderFilledDate = DateTime.UtcNow.AddMinutes(5);

            tr11.OrderPosition = OrderPositions.BUY;
            tr22.OrderPosition = OrderPositions.SELL;
            tr11.OrderFilledVolume = 950;
            tr22.OrderFilledVolume = 1000;
            tr11.OrderFilledDate = DateTime.UtcNow.AddMinutes(6);
            tr22.OrderFilledDate = DateTime.UtcNow.AddMinutes(6);

            tr3.OrderPosition = OrderPositions.BUY;
            tr4.OrderPosition = OrderPositions.SELL;
            tr4.OrderFilledVolume = 1500;
            tr3.OrderFilledVolume = 1500;
            tr3.OrderFilledDate = DateTime.UtcNow.AddMinutes(10);
            tr4.OrderFilledDate = DateTime.UtcNow.AddMinutes(10);

            tr5.OrderPosition = OrderPositions.BUY;
            tr6.OrderPosition = OrderPositions.SELL;
            tr6.OrderFilledVolume = 1500;
            tr5.OrderFilledVolume = 1500;
            tr5.OrderFilledDate = DateTime.UtcNow.AddMinutes(15);
            tr6.OrderFilledDate = DateTime.UtcNow.AddMinutes(20);

            var trades = new List<Order> { tr1, tr2, tr3, tr4, tr5, tr6, tr11, tr22 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.AreEqual(result.AmountOfBreachingClusters, 1);
        }
    }
}
