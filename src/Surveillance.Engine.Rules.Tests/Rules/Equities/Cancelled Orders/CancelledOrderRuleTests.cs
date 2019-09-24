namespace Surveillance.Engine.Rules.Tests.Rules.Equities.Cancelled_Orders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
    using Surveillance.Engine.Rules.Tests.Helpers;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    [TestFixture]
    public class CancelledOrderRuleTests
    {
        private IUniverseAlertStream _alertStream;

        private IUniverseMarketCacheFactory _cacheFactory;

        private ILogger<CancelledOrderRule> _logger;

        private ILogger<UniverseMarketCacheFactory> _loggerCache;

        private IUniverseOrderFilter _orderFilter;

        private ICancelledOrderRuleEquitiesParameters _parameters;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private IRuleRunDataRequestRepository _ruleRunRepository;

        private IStubRuleRunDataRequestRepository _stubRuleRunRepository;

        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

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
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new CancelledOrderRule(
                    this._parameters,
                    this._ruleCtx,
                    this._alertStream,
                    this._orderFilter,
                    this._cacheFactory,
                    RuleRunMode.ValidationRun,
                    null,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_ConsidersNullParameters_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new CancelledOrderRule(
                    null,
                    this._ruleCtx,
                    this._alertStream,
                    this._orderFilter,
                    this._cacheFactory,
                    RuleRunMode.ValidationRun,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void OnNext_DoesNotSendMessage_ForNoPositionSizeRuleBreach()
        {
            var cancelledOrdersByTradeSize = new List<Order>
                                                 {
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled)
                                                 };

            var parameters = new CancelledOrderRuleEquitiesParameters(
                "id",
                TimeSpan.FromMinutes(30),
                0.5m,
                null,
                10,
                10,
                null,
                false,
                true);

            var orderRule = new CancelledOrderRule(
                parameters,
                this._ruleCtx,
                this._alertStream,
                this._orderFilter,
                this.BuildFactory(),
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize.Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents) orderRule.OnNext(order);

            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_DoesNotSendsMessage_ForNoTradeCountRuleBreach()
        {
            var trade = this.OrderFactory(OrderStatus.Cancelled);
            trade.OrderFilledVolume = trade.OrderFilledVolume * 100;

            var cancelledOrdersByTradeSize = new List<Order>
                                                 {
                                                     trade,
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled)
                                                 };

            var parameters = new CancelledOrderRuleEquitiesParameters(
                "id",
                TimeSpan.FromMinutes(30),
                null,
                0.70m,
                3,
                10,
                null,
                false,
                true);

            var orderRule = new CancelledOrderRule(
                parameters,
                this._ruleCtx,
                this._alertStream,
                this._orderFilter,
                this.BuildFactory(),
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize.Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents) orderRule.OnNext(order);

            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForPositionSizeRuleBreach()
        {
            var trade = this.OrderFactory(OrderStatus.Cancelled);
            trade.OrderFilledVolume = trade.OrderFilledVolume * 100;
            trade.OrderOrderedVolume = trade.OrderOrderedVolume * 100;

            var cancelledOrdersByTradeSize = new List<Order>
                                                 {
                                                     trade,
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled)
                                                 };

            var parameters = new CancelledOrderRuleEquitiesParameters(
                "id",
                TimeSpan.FromMinutes(30),
                0.8m,
                null,
                3,
                10,
                null,
                false,
                true);

            var orderRule = new CancelledOrderRule(
                parameters,
                this._ruleCtx,
                this._alertStream,
                this._orderFilter,
                this.BuildFactory(),
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize.Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents) orderRule.OnNext(order);

            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 8);
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForTradeCountRuleBreach()
        {
            var cancelledOrdersByTradeSize = new List<Order>
                                                 {
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled)
                                                 };

            var parameters = new CancelledOrderRuleEquitiesParameters(
                "id",
                TimeSpan.FromMinutes(30),
                null,
                0.3m,
                3,
                20,
                null,
                false,
                true);
            var orderRule = new CancelledOrderRule(
                parameters,
                this._ruleCtx,
                this._alertStream,
                this._orderFilter,
                this.BuildFactory(),
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize.Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents) orderRule.OnNext(order);

            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 11);
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForTradeCountRuleBreach_WithNoMax()
        {
            var cancelledOrdersByTradeSize = new List<Order>
                                                 {
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Cancelled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled),
                                                     this.OrderFactory(OrderStatus.Filled)
                                                 };

            var parameters = new CancelledOrderRuleEquitiesParameters(
                "id",
                TimeSpan.FromMinutes(30),
                null,
                0.3m,
                3,
                null,
                null,
                false,
                true);

            var orderRule = new CancelledOrderRule(
                parameters,
                this._ruleCtx,
                this._alertStream,
                this._orderFilter,
                this.BuildFactory(),
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize.Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents) orderRule.OnNext(order);

            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 11);
        }

        [SetUp]
        public void Setup()
        {
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._parameters = A.Fake<ICancelledOrderRuleEquitiesParameters>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._cacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            this._ruleRunRepository = A.Fake<IRuleRunDataRequestRepository>();
            this._stubRuleRunRepository = A.Fake<IStubRuleRunDataRequestRepository>();
            this._loggerCache = A.Fake<ILogger<UniverseMarketCacheFactory>>();
            this._logger = A.Fake<ILogger<CancelledOrderRule>>();
            this._tradingHistoryLogger = A.Fake<ILogger<TradingHistoryStack>>();

            this._orderFilter = A.Fake<IUniverseOrderFilter>();
            A.CallTo(() => this._orderFilter.Filter(A<IUniverseEvent>.Ignored))
                .ReturnsLazily(i => (IUniverseEvent)i.Arguments[0]);
        }

        private UniverseMarketCacheFactory BuildFactory()
        {
            return new UniverseMarketCacheFactory(
                this._stubRuleRunRepository,
                this._ruleRunRepository,
                this._loggerCache);
        }

        private CancelledOrderRule BuildRule(CancelledOrderRuleEquitiesParameters parameters = null)
        {
            if (parameters == null)
                parameters = new CancelledOrderRuleEquitiesParameters(
                    "id",
                    TimeSpan.FromMinutes(30),
                    0.5m,
                    null,
                    10,
                    10,
                    null,
                    false,
                    true);

            var orderRule = new CancelledOrderRule(
                parameters,
                this._ruleCtx,
                this._alertStream,
                this._orderFilter,
                this.BuildFactory(),
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingHistoryLogger);

            return orderRule;
        }

        private Order OrderFactory(OrderStatus status)
        {
            var order = ((Order)null).Random();

            switch (status)
            {
                case OrderStatus.Cancelled:
                    order.CancelledDate = DateTime.UtcNow;
                    return order;
                case OrderStatus.Filled:
                    order.FilledDate = DateTime.UtcNow;
                    return order;
            }

            return order;
        }
    }
}