namespace Surveillance.Engine.Rules.Tests.Rules.Equities.WashTrades
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Tests.Helpers;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    [TestFixture]
    public class WashTradeRuleTests
    {
        private IUniverseAlertStream _alertStream;

        private IClusteringService _clustering;

        private ICurrencyConverterService _currencyConverterService;

        private IWashTradeRuleEquitiesParameters _equitiesParameters;

        private IUniverseMarketCacheFactory _factory;

        private ILogger _logger;

        private ILogger<UniverseMarketCacheFactory> _loggerCache;

        private IUniverseOrderFilter _orderFilter;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private IRuleRunDataRequestRepository _ruleRunRepository;

        private IStubRuleRunDataRequestRepository _stubRuleRunRepository;

        private ILogger<TradingHistoryStack> _tradingLogger;

        [Test]
        public void Clone_Copies_FactorValue_To_New_Clone()
        {
            var rule = this.BuildRule();
            var factor = new FactorValue(ClientOrganisationalFactors.Fund, "abcd");

            var clone = rule.Clone(factor);

            Assert.AreEqual(rule.OrganisationFactorValue.OrganisationalFactors, ClientOrganisationalFactors.None);
            Assert.AreEqual(rule.OrganisationFactorValue.Value, string.Empty);

            Assert.AreEqual(clone.OrganisationFactorValue.OrganisationalFactors, ClientOrganisationalFactors.Fund);
            Assert.AreEqual(clone.OrganisationFactorValue.Value, "abcd");
        }

        [Test]
        public void Clustering_DontPerformAnalysis_ReturnsNoBreach()
        {
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(false);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var result = rule.ClusteringTrades(null);

            Assert.AreEqual(result.ClusteringPositionBreach, false);
        }

        [Test]
        [Explicit]
        public void Clustering_FourClusterExpectedWithOnInValueAndNumberOfTradesRange_ReturnsOneBreach()
        {
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);
            A.CallTo(() => this._equitiesParameters.ClusteringPositionMinimumNumberOfTrades).Returns(4);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var tr1 = new Order().Random(21);
            var tr2 = new Order().Random(21);
            var tr11 = new Order().Random(21);
            var tr22 = new Order().Random(21);

            var tr3 = new Order().Random(100);
            var tr4 = new Order().Random(101);

            var tr5 = new Order().Random(50);
            var tr6 = new Order().Random(70);

            tr1.OrderDirection = OrderDirections.BUY;
            tr2.OrderDirection = OrderDirections.SELL;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;
            tr1.FilledDate = DateTime.UtcNow.AddMinutes(5);
            tr2.FilledDate = DateTime.UtcNow.AddMinutes(5);

            tr11.OrderDirection = OrderDirections.BUY;
            tr22.OrderDirection = OrderDirections.SELL;
            tr11.OrderFilledVolume = 950;
            tr22.OrderFilledVolume = 1000;
            tr11.FilledDate = DateTime.UtcNow.AddMinutes(6);
            tr22.FilledDate = DateTime.UtcNow.AddMinutes(6);

            tr3.OrderDirection = OrderDirections.BUY;
            tr4.OrderDirection = OrderDirections.SELL;
            tr4.OrderFilledVolume = 1500;
            tr3.OrderFilledVolume = 1500;
            tr3.FilledDate = DateTime.UtcNow.AddMinutes(10);
            tr4.FilledDate = DateTime.UtcNow.AddMinutes(10);

            tr5.OrderDirection = OrderDirections.BUY;
            tr6.OrderDirection = OrderDirections.SELL;
            tr6.OrderFilledVolume = 1500;
            tr5.OrderFilledVolume = 1500;
            tr5.FilledDate = DateTime.UtcNow.AddMinutes(15);
            tr6.FilledDate = DateTime.UtcNow.AddMinutes(20);

            var trades = new List<Order>
                             {
                                 tr1,
                                 tr2,
                                 tr3,
                                 tr4,
                                 tr5,
                                 tr6,
                                 tr11,
                                 tr22
                             };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.AreEqual(result.AmountOfBreachingClusters, 1);
        }

        [Test]
        public void Clustering_FourClusterExpectedWithTwoWithinValueRange_ReturnsOneBreach()
        {
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var tr1 = new Order().Random(21);
            var tr2 = new Order().Random(21);

            var tr3 = new Order().Random(100);
            var tr4 = new Order().Random(101);

            var tr5 = new Order().Random(50);
            var tr6 = new Order().Random(70);

            tr1.OrderDirection = OrderDirections.BUY;
            tr2.OrderDirection = OrderDirections.SELL;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;
            tr1.FilledDate = DateTime.UtcNow.AddMinutes(5);
            tr2.FilledDate = DateTime.UtcNow.AddMinutes(5);

            tr3.OrderDirection = OrderDirections.BUY;
            tr4.OrderDirection = OrderDirections.SELL;
            tr4.OrderFilledVolume = 1500;
            tr3.OrderFilledVolume = 1500;
            tr3.FilledDate = DateTime.UtcNow.AddMinutes(10);
            tr4.FilledDate = DateTime.UtcNow.AddMinutes(10);

            tr5.OrderDirection = OrderDirections.BUY;
            tr6.OrderDirection = OrderDirections.SELL;
            tr6.OrderFilledVolume = 1500;
            tr5.OrderFilledVolume = 1500;
            tr5.FilledDate = DateTime.UtcNow.AddMinutes(15);
            tr6.FilledDate = DateTime.UtcNow.AddMinutes(15);

            var trades = new List<Order>
                             {
                                 tr1,
                                 tr2,
                                 tr3,
                                 tr4,
                                 tr5,
                                 tr6
                             };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.IsTrue(result.AmountOfBreachingClusters < 3);
        }

        [Test]
        public void Clustering_NullActiveTrades_ReturnsNoBreach()
        {
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var result = rule.ClusteringTrades(null);

            Assert.AreEqual(result.ClusteringPositionBreach, false);
        }

        [Test]
        public void Clustering_NullClusterResponse_ReturnsNoBreach()
        {
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var trades = new List<Order> { new Order().Random() };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, false);
        }

        [Test]
        public void Clustering_OneClusterExpected_ReturnsNoBreach()
        {
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var tr1 = new Order().Random(19);
            var tr2 = new Order().Random(21);

            tr1.OrderDirection = OrderDirections.BUY;
            tr2.OrderDirection = OrderDirections.SELL;

            tr1.OrderAverageFillPrice = tr2.OrderAverageFillPrice;
            tr1.OrderFilledVolume = 2000;
            tr2.OrderFilledVolume = 1000;

            var trades = new List<Order> { tr1, tr2 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, false);
        }

        [Test]
        public void Clustering_OneClusterExpectedWithinValueRange_ReturnsOneBreach()
        {
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var tr1 = new Order().Random(19);
            var tr2 = new Order().Random(21);

            tr1.OrderDirection = OrderDirections.BUY;
            tr2.OrderDirection = OrderDirections.SELL;

            tr1.OrderAverageFillPrice = tr2.OrderAverageFillPrice;
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
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var tr1 = new Order().Random(21);
            var tr2 = new Order().Random(21);

            var tr3 = new Order().Random(10);
            var tr4 = new Order().Random(9);

            tr1.OrderDirection = OrderDirections.BUY;
            tr2.OrderDirection = OrderDirections.SELL;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;
            tr1.FilledDate = DateTime.UtcNow;
            tr2.FilledDate = DateTime.UtcNow;

            tr3.OrderDirection = OrderDirections.BUY;
            tr4.OrderDirection = OrderDirections.SELL;
            tr4.OrderFilledVolume = 950;
            tr3.OrderFilledVolume = 1500;
            tr3.FilledDate = DateTime.UtcNow.AddMinutes(5);
            tr4.FilledDate = DateTime.UtcNow.AddMinutes(5);

            var trades = new List<Order> { tr1, tr2, tr3, tr4 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.AreEqual(result.AmountOfBreachingClusters, 1);
        }

        [Test]
        public void Clustering_TwoClusterExpectedWithTwoWithinValueRange_ReturnsOneBreach()
        {
            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);

            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            var tr1 = new Order().Random(21);
            var tr2 = new Order().Random(21);

            var tr3 = new Order().Random(100);
            var tr4 = new Order().Random(101);

            tr1.OrderDirection = OrderDirections.BUY;
            tr2.OrderDirection = OrderDirections.SELL;
            tr1.OrderFilledVolume = 950;
            tr2.OrderFilledVolume = 1000;

            tr3.OrderDirection = OrderDirections.BUY;
            tr4.OrderDirection = OrderDirections.SELL;
            tr4.OrderFilledVolume = 1500;
            tr3.OrderFilledVolume = 1500;

            var trades = new List<Order> { tr1, tr2, tr3, tr4 };
            var result = rule.ClusteringTrades(trades);

            Assert.AreEqual(result.ClusteringPositionBreach, true);
            Assert.AreEqual(result.AmountOfBreachingClusters, 2);
        }

        [SetUp]
        public void Setup()
        {
            this._currencyConverterService = A.Fake<ICurrencyConverterService>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._clustering = new ClusteringService();
            this._equitiesParameters = A.Fake<IWashTradeRuleEquitiesParameters>();
            this._logger = A.Fake<ILogger>();
            this._ruleRunRepository = A.Fake<IRuleRunDataRequestRepository>();
            this._stubRuleRunRepository = A.Fake<IStubRuleRunDataRequestRepository>();
            this._loggerCache = A.Fake<ILogger<UniverseMarketCacheFactory>>();
            this._tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();

            this._orderFilter = A.Fake<IUniverseOrderFilter>();
            this._factory = new UniverseMarketCacheFactory(
                this._stubRuleRunRepository,
                this._ruleRunRepository,
                this._loggerCache);
            A.CallTo(() => this._orderFilter.Filter(A<IUniverseEvent>.Ignored))
                .ReturnsLazily(i => (IUniverseEvent)i.Arguments[0]);

            A.CallTo(() => this._equitiesParameters.PerformClusteringPositionAnalysis).Returns(true);
            A.CallTo(() => this._equitiesParameters.ClusteringPercentageValueDifferenceThreshold).Returns(0.05m);
        }

        private WashTradeRule BuildRule()
        {
            var rule = new WashTradeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._clustering,
                this._alertStream,
                this._currencyConverterService,
                this._orderFilter,
                this._factory,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);

            return rule;
        }
    }
}