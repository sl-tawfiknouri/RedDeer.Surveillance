﻿using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.CancelledOrders;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe;

namespace Surveillance.Tests.Rules.Cancelled_Orders
{
    [TestFixture]
    public class CancelledOrderRuleTests
    {
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ICancelledOrderRuleParameters _parameters;
        private IUniverseAlertStream _alertStream;
        private ILogger<CancelledOrderRule> _logger;

        [SetUp]
        public void Setup()
        {
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _parameters = A.Fake<ICancelledOrderRuleParameters>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _logger = A.Fake<ILogger<CancelledOrderRule>>();
        }

        [Test]
        public void Constructor_ConsidersNullParameters_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRule(null, _ruleCtx, _alertStream, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRule(_parameters, _ruleCtx, _alertStream, null));
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForTradeCountRuleBreach()
        {
            var cancelledOrdersByTradeSize = new List<Order>
            {
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), null, 0.3m, 3, 20, null, false);

            var orderRule = new CancelledOrderRule(parameters,  _ruleCtx, _alertStream, _logger);


            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));


            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 11);
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForTradeCountRuleBreach_WithNoMax()
        {
            var cancelledOrdersByTradeSize = new List<Order>
            {
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), null, 0.3m, 3, null, null, false);

            var orderRule = new CancelledOrderRule(parameters, _ruleCtx, _alertStream, _logger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 11);
        }

        [Test]
        public void OnNext_DoesNotSendsMessage_ForNoTradeCountRuleBreach()
        {
            var trade = TradeFrame(OrderStatus.Cancelled);
            trade.FulfilledVolume = trade.FulfilledVolume * 100;

            var cancelledOrdersByTradeSize = new List<Order>
            {
                trade,
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), null, 0.70m, 3, 10, null, false);

            var orderRule = new CancelledOrderRule(parameters, _ruleCtx, _alertStream, _logger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForPositionSizeRuleBreach()
        {
            var trade = TradeFrame(OrderStatus.Cancelled);
            trade.FulfilledVolume = trade.FulfilledVolume * 100;
            trade.OrderedVolume = trade.OrderedVolume * 100;

            var cancelledOrdersByTradeSize = new List<Order>
            {
                trade,
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
                TradeFrame(OrderStatus.Filled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), 0.8m, null, 3, 10, null, false);

            var orderRule = new CancelledOrderRule(parameters, _ruleCtx, _alertStream, _logger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 8);
        }

        [Test]
        public void OnNext_DoesNotSendMessage_ForNoPositionSizeRuleBreach()
        {
            var cancelledOrdersByTradeSize = new List<Order>
            {
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), 0.5m, null, 10, 10, null, false);

            var orderRule = new CancelledOrderRule(parameters,  _ruleCtx, _alertStream, _logger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored))
                .MustNotHaveHappened();
        }
    }
}