using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Market;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Rules.Cancelled_Orders;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Tests.Rules.Cancelled_Orders
{
    [TestFixture]
    public class CancelledOrderRuleTests
    {
        private ICancelledOrderMessageSender _messageSender;
        private ICancelledOrderRuleParameters _parameters;
        private ILogger<CancelledOrderRule> _logger;

        [SetUp]
        public void Setup()
        {
            _messageSender = A.Fake<ICancelledOrderMessageSender>();
            _parameters = A.Fake<ICancelledOrderRuleParameters>();
            _logger = A.Fake<ILogger<CancelledOrderRule>>();
        }

        [Test]
        public void Constructor_ConsidersNullMessageSender_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRule(null, _parameters, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullParameters_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRule(_messageSender, null, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderRule(_messageSender, _parameters, null));
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
            };

            var parameters = new CancelledOrderRuleParameters(TimeSpan.FromMinutes(30), null, 0.3m, 3, 10);

            var orderRule = new CancelledOrderRule(_messageSender, parameters, _logger);

            foreach (var order in cancelledOrdersByTradeSize)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 8);
        }

        [Test]
        public void OnNext_DoesNotSendsMessage_ForNoTradeCountRuleBreach()
        {
            var trade = TradeFrame(OrderStatus.Cancelled);
            trade.Volume = trade.Volume * 100;

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

            var orderRule = new CancelledOrderRule(_messageSender, parameters, _logger);

            foreach (var order in cancelledOrdersByTradeSize)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void OnNext_SendsExpectedMessage_ForPositionSizeRuleBreach()
        {
            var trade = TradeFrame(OrderStatus.Cancelled);
            trade.Volume = trade.Volume * 100;

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

            var orderRule = new CancelledOrderRule(_messageSender, parameters, _logger);

            foreach (var order in cancelledOrdersByTradeSize)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
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

            var orderRule = new CancelledOrderRule(_messageSender, parameters, _logger);

            foreach (var order in cancelledOrdersByTradeSize)
            {
                orderRule.OnNext(order);
            }

            A
                .CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustNotHaveHappened();
        }

        private TradeOrderFrame TradeFrame(OrderStatus status)
        {
            return new TradeOrderFrame(
                OrderType.Market,
                new StockExchange(new Market.MarketId("XLON"), "XLON"),
                new Security(
                    new SecurityIdentifiers("client id", "1234567", "12345678912", "figi", "cusip", "test"),
                    "Test Security",
                    "CFI"),
                null,
                1000,
                OrderPosition.BuyLong,
                status,
                DateTime.Now,
                DateTime.Now,
                "trader-1",
                "client-attribution-id",
                "party-broker",
                "counter party");
        }
    }
}
