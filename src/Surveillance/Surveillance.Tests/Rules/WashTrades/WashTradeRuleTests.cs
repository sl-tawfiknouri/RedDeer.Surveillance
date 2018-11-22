using System.Collections.Generic;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.WashTrade;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Tests.Helpers;

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
        private ILogger _logger;

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
                _logger);

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
                _logger);

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
                _logger);

            var trades = new List<TradeOrderFrame> {new TradeOrderFrame().Random()};
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
                _logger);

            var tr1 = new TradeOrderFrame().Random(19);
            var tr2 = new TradeOrderFrame().Random(21);

            tr1.Position = OrderPosition.Buy;
            tr2.Position = OrderPosition.Sell;

            tr1.ExecutedPrice = tr2.ExecutedPrice;
            tr1.FulfilledVolume = 2000;
            tr2.FulfilledVolume = 1000;

            var trades = new List<TradeOrderFrame> { tr1, tr2 };
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
                _logger);

            var tr1 = new TradeOrderFrame().Random(19);
            var tr2 = new TradeOrderFrame().Random(21);

            tr1.Position = OrderPosition.Buy;
            tr2.Position = OrderPosition.Sell;

            tr1.ExecutedPrice = tr2.ExecutedPrice;
            tr1.FulfilledVolume = 950;
            tr2.FulfilledVolume = 1000;

            var trades = new List<TradeOrderFrame> { tr1, tr2 };
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
                _logger);

            var tr1 = new TradeOrderFrame().Random(21);
            var tr2 = new TradeOrderFrame().Random(21);

            var tr3 = new TradeOrderFrame().Random(10);
            var tr4 = new TradeOrderFrame().Random(9);

            tr1.Position = OrderPosition.Buy;
            tr2.Position = OrderPosition.Sell;
            tr1.FulfilledVolume = 950;
            tr2.FulfilledVolume = 1000;

            tr3.Position = OrderPosition.Buy;
            tr4.Position = OrderPosition.Sell;
            tr4.FulfilledVolume = 950;
            tr3.FulfilledVolume = 1500;

            var trades = new List<TradeOrderFrame> { tr1, tr2, tr3, tr4 };
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
                _logger);

            var tr1 = new TradeOrderFrame().Random(21);
            var tr2 = new TradeOrderFrame().Random(21);

            var tr3 = new TradeOrderFrame().Random(100);
            var tr4 = new TradeOrderFrame().Random(101);

            tr1.Position = OrderPosition.Buy;
            tr2.Position = OrderPosition.Sell;
            tr1.FulfilledVolume = 950;
            tr2.FulfilledVolume = 1000;

            tr3.Position = OrderPosition.Buy;
            tr4.Position = OrderPosition.Sell;
            tr4.FulfilledVolume = 1500;
            tr3.FulfilledVolume = 1500;

            var trades = new List<TradeOrderFrame> { tr1, tr2, tr3, tr4 };
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
                _logger);

            var tr1 = new TradeOrderFrame().Random(21);
            var tr2 = new TradeOrderFrame().Random(21);

            var tr3 = new TradeOrderFrame().Random(100);
            var tr4 = new TradeOrderFrame().Random(101);

            var tr5 = new TradeOrderFrame().Random(50);
            var tr6 = new TradeOrderFrame().Random(70);

            tr1.Position = OrderPosition.Buy;
            tr2.Position = OrderPosition.Sell;
            tr1.FulfilledVolume = 950;
            tr2.FulfilledVolume = 1000;

            tr3.Position = OrderPosition.Buy;
            tr4.Position = OrderPosition.Sell;
            tr4.FulfilledVolume = 1500;
            tr3.FulfilledVolume = 1500;

            var trades = new List<TradeOrderFrame> { tr1, tr2, tr3, tr4, tr5, tr6 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.IsTrue(result.AmountOfBreachingClusters < 3);
        }

        [Test]
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
                _logger);

            var tr1 = new TradeOrderFrame().Random(21);
            var tr2 = new TradeOrderFrame().Random(21);
            var tr11 = new TradeOrderFrame().Random(21);
            var tr22 = new TradeOrderFrame().Random(21);


            var tr3 = new TradeOrderFrame().Random(100);
            var tr4 = new TradeOrderFrame().Random(101);

            var tr5 = new TradeOrderFrame().Random(50);
            var tr6 = new TradeOrderFrame().Random(70);

            tr1.Position = OrderPosition.Buy;
            tr2.Position = OrderPosition.Sell;
            tr1.FulfilledVolume = 950;
            tr2.FulfilledVolume = 1000;
            tr11.Position = OrderPosition.Buy;
            tr22.Position = OrderPosition.Sell;
            tr11.FulfilledVolume = 950;
            tr22.FulfilledVolume = 1000;

            tr3.Position = OrderPosition.Buy;
            tr4.Position = OrderPosition.Sell;
            tr4.FulfilledVolume = 1500;
            tr3.FulfilledVolume = 1500;

            var trades = new List<TradeOrderFrame> { tr1, tr2, tr3, tr4, tr5, tr6, tr11, tr22 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.AreEqual(result.AmountOfBreachingClusters, 1);
        }
    }
}
