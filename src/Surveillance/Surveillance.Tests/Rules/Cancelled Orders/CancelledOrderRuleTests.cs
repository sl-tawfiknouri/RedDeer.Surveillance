using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Market;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Rules.CancelledOrders;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.Rule_Parameters;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe;

namespace Surveillance.Tests.Rules.Cancelled_Orders
{
    [TestFixture]
    public class CancelledOrderRuleTests
    {
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ICancelledOrderRuleCachedMessageSender _dedupeCachedMessageSender;
        private ICancelledOrderRuleParameters _parameters;
        private ILogger<CancelledOrderRule> _logger;

        [SetUp]
        public void Setup()
        {
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _dedupeCachedMessageSender = A.Fake<ICancelledOrderRuleCachedMessageSender>();
            _parameters = A.Fake<ICancelledOrderRuleParameters>();
            _logger = A.Fake<ILogger<CancelledOrderRule>>();
        }

        [Test]
        public void Constructor_ConsidersNullMessageSender_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRule( _parameters, null, _ruleCtx, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullParameters_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRule(null, _dedupeCachedMessageSender, _ruleCtx, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRule(_parameters, _dedupeCachedMessageSender, _ruleCtx, null));
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForTradeCountRuleBreach()
        {
            var cancelledOrdersByTradeSize = new List<TradeOrderFrame>
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
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), null, 0.3m, 3, 20);

            var orderRule = new CancelledOrderRule(parameters, _dedupeCachedMessageSender, _ruleCtx, _logger);


            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));


            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _dedupeCachedMessageSender.Send(A<CancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 11);
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForTradeCountRuleBreach_WithNoMax()
        {
            var cancelledOrdersByTradeSize = new List<TradeOrderFrame>
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
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), null, 0.3m, 3, null);

            var orderRule = new CancelledOrderRule(parameters, _dedupeCachedMessageSender, _ruleCtx, _logger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _dedupeCachedMessageSender.Send(A<CancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 11);
        }

        [Test]
        public void OnNext_DoesNotSendsMessage_ForNoTradeCountRuleBreach()
        {
            var trade = TradeFrame(OrderStatus.Cancelled);
            trade.FulfilledVolume = trade.FulfilledVolume * 100;

            var cancelledOrdersByTradeSize = new List<TradeOrderFrame>
            {
                trade,
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), null, 0.70m, 3, 10);

            var orderRule = new CancelledOrderRule(parameters, _dedupeCachedMessageSender, _ruleCtx, _logger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _dedupeCachedMessageSender.Send(A<CancelledOrderRuleBreach>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForPositionSizeRuleBreach()
        {
            var trade = TradeFrame(OrderStatus.Cancelled);
            trade.FulfilledVolume = trade.FulfilledVolume * 100;
            trade.OrderedVolume = trade.OrderedVolume * 100;

            var cancelledOrdersByTradeSize = new List<TradeOrderFrame>
            {
                trade,
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), 0.8m, null, 3, 10);

            var orderRule = new CancelledOrderRule(parameters, _dedupeCachedMessageSender, _ruleCtx, _logger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _dedupeCachedMessageSender.Send(A<CancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 8);
        }

        [Test]
        public void OnNext_DoesNotSendMessage_ForNoPositionSizeRuleBreach()
        {
            var cancelledOrdersByTradeSize = new List<TradeOrderFrame>
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

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), 0.5m, null, 10, 10);

            var orderRule = new CancelledOrderRule(parameters, _dedupeCachedMessageSender, _ruleCtx, _logger);

            var universeEvents =
                cancelledOrdersByTradeSize
                    .Select(x => new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.Now, x));

            foreach (var order in universeEvents)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() => _dedupeCachedMessageSender.Send(A<CancelledOrderRuleBreach>.Ignored))
                .MustNotHaveHappened();
        }

        private TradeOrderFrame TradeFrame(OrderStatus status)
        {
            var securityIdentifiers =
                new SecurityIdentifiers(
                    string.Empty,
                    "reddeer id",
                    "client id",
                    "1234567",
                    "12345678912",
                    "figi", 
                    "cusip",
                    "test",
                    "testLei",
                    "tick");

            var security = new Security(
                securityIdentifiers,
                "Test Security",
                "CFI",
                "Issuer-Identifier");

            return new TradeOrderFrame(
                null,
                OrderType.Market,
                new StockExchange(new Market.MarketId("XLON"), "XLON"),
                security,
                null,
                new Price(1000, "GBP"),
                1000,
                1000,
                OrderPosition.Buy,
                status,
                DateTime.Now,
                DateTime.Now,
                "trader-1",
                "client-attribution-id",
                "account-1",
                "dealer-instructions",
                "party-broker",
                "counter party",
                "trade rationale",
                "good strategy",
                "GBX");
        }
    }
}