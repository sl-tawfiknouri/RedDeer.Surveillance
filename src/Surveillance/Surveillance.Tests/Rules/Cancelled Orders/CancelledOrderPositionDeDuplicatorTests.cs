using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Market;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Configuration;
using Surveillance.Configuration.Interfaces;
using Surveillance.Rules.Cancelled_Orders;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Tests.Rules.Cancelled_Orders
{
    [TestFixture]
    public class CancelledOrderPositionDeDuplicatorTests
    {
        private IRuleConfiguration _ruleConfiguration;
        private ICancelledOrderMessageSender _messageSender;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _ruleConfiguration = A.Fake<IRuleConfiguration>();
            _messageSender = A.Fake<ICancelledOrderMessageSender>();
            _logger = A.Fake<ILogger>();
        }

        [Test]
        public void Constructor_RuleConfigurationNull_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderPositionDeDuplicator(null, _messageSender));
        }

        [Test]
        public void Constructor_RuleConfigurationMessageSenderNull_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancelledOrderPositionDeDuplicator(_ruleConfiguration, null));
        }

        [Test]
        public void Send_ForwardsMessageToMessageSender()
        {
            var ruleConfiguration = new RuleConfiguration {CancelledOrderDeduplicationDelaySeconds = 1};
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, _messageSender);
            var identifiers = new SecurityIdentifiers("client-1", "sedol-1", "isin-1", "figi-1", "cusip-1", "XCH");
            var tradePosition = new TradePosition(new List<TradeOrderFrame>(), null, null, _logger);
            var parameters = new CancelledOrderMessageSenderParameters(identifiers) { TradePosition = tradePosition };

            deduplicator.Send(parameters);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            A.CallTo(() =>
                _messageSender.Send(
                    A<ITradePosition>.Ignored,
                    A<ICancelledOrderRuleBreach>.Ignored,
                    A<ICancelledOrderRuleParameters>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Send_DiscardsInitialMessageIfFollowUpWithDuplicateWithinTimePeriod()
        {
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 5 };

            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, _messageSender);
            var identifiers = new SecurityIdentifiers("client-1", "sedol-1", "isin-1", "figi-1", "cusip-1", "XCH");
            var tradePosition = new TradePosition(new List<TradeOrderFrame>(), null, null, _logger);
            var parameters = new CancelledOrderMessageSenderParameters(identifiers) { TradePosition = tradePosition };

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(8);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            A.CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Send_DiscardsOlderMessagesIfFollowUpWithManyDuplicatesWithinTimePeriod()
        {
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };

            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, _messageSender);
            var identifiers = new SecurityIdentifiers("client-1", "sedol-1", "isin-1", "figi-1", "cusip-1", "XCH");
            var tradePosition = new TradePosition(new List<TradeOrderFrame>(), null, null, _logger);
            var parameters = new CancelledOrderMessageSenderParameters(identifiers) { TradePosition = tradePosition };

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            A.CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Send_DiscardsOlderMessagesIfFollowUpWithManyDuplicatesWithinTwoTimePeriods()
        {
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };

            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, _messageSender);
            var identifiers = new SecurityIdentifiers("client-1", "sedol-1", "isin-1", "figi-1", "cusip-1", "XCH");
            var tradePosition = new TradePosition(new List<TradeOrderFrame>(), null, null, _logger);
            var parameters = new CancelledOrderMessageSenderParameters(identifiers) { TradePosition = tradePosition };

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);

            var exitLoopAt2 = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt2)
            {
            }

            A.CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void Send_ConsidersTwoNonIntersectingPositions_InTheSameSecurityAsSeparateMessages()
        {
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };

            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, _messageSender);

            var tradeFrame1 = TradeFrame();
            var tradeFrame2 = TradeFrame();
            var tradeFrame3 = TradeFrame();
            var tradeFrame4 = TradeFrame();
            var tradeFrame5 = TradeFrame();

            var tradePositionAlertOne =
                new TradePosition(
                    new List<TradeOrderFrame> {tradeFrame1, tradeFrame2, tradeFrame3},
                    null, 
                    null,
                    _logger);

            var tradePositionAlertTwo =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame2, tradeFrame3, tradeFrame3, tradeFrame4, tradeFrame5 },
                    null,
                    null,
                    _logger);

            var parameters = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
                { TradePosition = tradePositionAlertOne };

            var parameters2 = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
                { TradePosition = tradePositionAlertTwo };

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            A.CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void Send_ConsidersTwoNonIntersectingPositions_InTheSameSecurityAsSeparateMessages_ResendsBothWithDelay()
        {
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };

            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, _messageSender);

            var tradeFrame1 = TradeFrame();
            var tradeFrame2 = TradeFrame();
            var tradeFrame3 = TradeFrame();
            var tradeFrame4 = TradeFrame();
            var tradeFrame5 = TradeFrame();

            var tradePositionAlertOne =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame1, tradeFrame2, tradeFrame3 },
                    null,
                    null,
                    _logger);

            var tradePositionAlertTwo =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame2, tradeFrame3, tradeFrame3, tradeFrame4, tradeFrame5 },
                    null,
                    null,
                    _logger);

            var parameters = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
            { TradePosition = tradePositionAlertOne };

            var parameters2 = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
            { TradePosition = tradePositionAlertTwo };

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);

            var exitLoopAt2 = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt2)
            {
            }

            A.CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 4);
        }

        [Test]
        public void Send_ConsidersTwoNonIntersectingPositionsAndTwoIntersectingPositionsForTotalOfThreePositions_InTheSameSecurityAsTwoSeparateMessages()
        {
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };

            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, _messageSender);

            var tradeFrame1 = TradeFrame();
            var tradeFrame2 = TradeFrame();
            var tradeFrame3 = TradeFrame();
            var tradeFrame4 = TradeFrame();
            var tradeFrame5 = TradeFrame();

            var tradePositionAlertOne =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame1, tradeFrame2, tradeFrame3 },
                    null,
                    null,
                    _logger);

            var tradePositionAlertTwo =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame1, tradeFrame2, tradeFrame3, tradeFrame4 },
                    null,
                    null,
                    _logger);

            var tradePositionAlertThree =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame2, tradeFrame3, tradeFrame4, tradeFrame5 },
                    null,
                    null,
                    _logger);

            var parameters = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
            { TradePosition = tradePositionAlertOne };

            var parameters2 = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
            { TradePosition = tradePositionAlertTwo };

            var parameters3 = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
            {TradePosition = tradePositionAlertThree};

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters3);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            A.CallTo(() =>
                    _messageSender.Send(
                        A<ITradePosition>.Ignored,
                        A<ICancelledOrderRuleBreach>.Ignored,
                        A<ICancelledOrderRuleParameters>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 2);
        }

        private TradeOrderFrame TradeFrame()
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
                OrderStatus.Fulfilled,
                DateTime.Now,
                DateTime.Now,
                "trader-1",
                "client-attribution-id",
                "party-broker",
                "counter party");
        }
    }
}
