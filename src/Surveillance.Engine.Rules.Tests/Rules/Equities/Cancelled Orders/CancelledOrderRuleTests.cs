using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Financial;
using Domain.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
using Surveillance.Engine.Rules.Tests.Helpers;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Rules.Equities.Cancelled_Orders
{
    [TestFixture]
    public class CancelledOrderRuleTests
    {
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ICancelledOrderRuleEquitiesParameters _parameters;
        private IUniverseAlertStream _alertStream;
        private IUniverseOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _cacheFactory;
        private IRuleRunDataRequestRepository _ruleRunRepository;
        private IStubRuleRunDataRequestRepository _stubRuleRunRepository;
        private ILogger<CancelledOrderRule> _logger;
        private ILogger<UniverseMarketCacheFactory> _loggerCache;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        [SetUp]
        public void Setup()
        {
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _parameters = A.Fake<ICancelledOrderRuleEquitiesParameters>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _cacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _ruleRunRepository = A.Fake<IRuleRunDataRequestRepository>();
            _stubRuleRunRepository = A.Fake<IStubRuleRunDataRequestRepository>();
            _loggerCache = A.Fake<ILogger<UniverseMarketCacheFactory>>();
            _logger = A.Fake<ILogger<CancelledOrderRule>>();
            _tradingHistoryLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _orderFilter = A.Fake<IUniverseOrderFilter>();
            A.CallTo(() => _orderFilter.Filter(A<IUniverseEvent>.Ignored)).ReturnsLazily(i => (IUniverseEvent)i.Arguments[0]);
        }

        [Test]
        public void Constructor_ConsidersNullParameters_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => 
                new CancelledOrderRule(
                    null,
                    _ruleCtx, 
                    _alertStream, 
                    _orderFilter, 
                    _cacheFactory, 
                    RuleRunMode.ValidationRun,
                    _logger, 
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => 
                new CancelledOrderRule(
                    _parameters,
                    _ruleCtx,
                    _alertStream,
                    _orderFilter,
                    _cacheFactory,
                    RuleRunMode.ValidationRun,
                    null,
                    _tradingHistoryLogger));
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForTradeCountRuleBreach()
        {
            var cancelledOrdersByTradeSize = new List<Order>
            {
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleEquitiesParameters("id", TimeSpan.FromMinutes(30), null, 0.3m, 3, 20, null, false);
            var orderRule = new CancelledOrderRule(parameters,  _ruleCtx, _alertStream, _orderFilter, BuildFactory(), RuleRunMode.ValidationRun,
                _logger, _tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));


            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 11);
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForTradeCountRuleBreach_WithNoMax()
        {
            var cancelledOrdersByTradeSize = new List<Order>
            {
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleEquitiesParameters("id", TimeSpan.FromMinutes(30), null, 0.3m, 3, null, null, false);

            var orderRule = new CancelledOrderRule(parameters, _ruleCtx, _alertStream, _orderFilter, BuildFactory(), RuleRunMode.ValidationRun,
                _logger, _tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 11);
        }

        [Test]
        public void OnNext_DoesNotSendsMessage_ForNoTradeCountRuleBreach()
        {
            var trade = OrderFactory(OrderStatus.Cancelled);
            trade.OrderFilledVolume = trade.OrderFilledVolume * 100;

            var cancelledOrdersByTradeSize = new List<Order>
            {
                trade,
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleEquitiesParameters("id", TimeSpan.FromMinutes(30), null, 0.70m, 3, 10, null, false);

            var orderRule = new CancelledOrderRule(parameters, _ruleCtx, _alertStream, _orderFilter, BuildFactory(), RuleRunMode.ValidationRun,
                _logger, _tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForPositionSizeRuleBreach()
        {
            var trade = OrderFactory(OrderStatus.Cancelled);
            trade.OrderFilledVolume = trade.OrderFilledVolume * 100;
            trade.OrderOrderedVolume = trade.OrderOrderedVolume * 100;

            var cancelledOrdersByTradeSize = new List<Order>
            {
                trade,
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleEquitiesParameters("id", TimeSpan.FromMinutes(30), 0.8m, null, 3, 10, null, false);

            var orderRule = new CancelledOrderRule(parameters, _ruleCtx, _alertStream, _orderFilter, BuildFactory(), RuleRunMode.ValidationRun,
                _logger, _tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 8);
        }

        [Test]
        public void OnNext_DoesNotSendMessage_ForNoPositionSizeRuleBreach()
        {
            var cancelledOrdersByTradeSize = new List<Order>
            {
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Cancelled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
                OrderFactory(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleEquitiesParameters("id", TimeSpan.FromMinutes(30), 0.5m, null, 10, 10, null, false);

            var orderRule = new CancelledOrderRule(parameters,  _ruleCtx, _alertStream, _orderFilter, BuildFactory(), RuleRunMode.ValidationRun,
                _logger, _tradingHistoryLogger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustNotHaveHappened();
        }

        private Order OrderFactory(OrderStatus status)
        {
            var order = ((Order) (null)).Random();

            switch (status)
            {
                case Domain.Financial.OrderStatus.Cancelled:
                    order.CancelledDate = DateTime.UtcNow;
                    return order;
                case Domain.Financial.OrderStatus.Filled:
                    order.FilledDate = DateTime.UtcNow;
                    return order;
            }

            return order;
        }

        private UniverseMarketCacheFactory BuildFactory()
        {
            return new UniverseMarketCacheFactory(_stubRuleRunRepository, _ruleRunRepository, _loggerCache);
        }
    }
}