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
using Surveillance.Trades;

namespace Surveillance.Tests.Rules.Cancelled_Orders
{
    [TestFixture]
    public class CancelledOrderPositionDeDuplicatorTests
    {
       private IRuleConfiguration _ruleConfiguration;
       private ICancelledOrderMessageSender _messageSender;

        [SetUp]
        public void Setup()
        {
            _ruleConfiguration = A.Fake<IRuleConfiguration>();
            _messageSender = A.Fake<ICancelledOrderMessageSender>();
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
        [Parallelizable]
        public void Send_ForwardsMessageToMessageSender()
        {
            var messageSender = A.Fake<ICancelledOrderMessageSender>();
            var logger = A.Fake<ILogger>();           
            var ruleConfiguration = new RuleConfiguration {CancelledOrderDeduplicationDelaySeconds = 1};
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, messageSender);
            var identifiers = new SecurityIdentifiers("client-1", "sedol-1", "isin-1", "figi-1", "cusip-1", "XCH");
            var tradePosition = new TradePosition(new List<TradeOrderFrame>(), null, null, logger);
            var parameters = new CancelledOrderMessageSenderParameters(identifiers) { TradePosition = tradePosition };

            deduplicator.Send(parameters);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(2);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            A.CallTo(() =>
                    messageSender.Send(A<ICancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [Parallelizable]
        public void Send_DiscardsInitialMessageIfFollowUpWithDuplicateWithinTimePeriod()
        {
            var messageSender = A.Fake<ICancelledOrderMessageSender>();
            var logger = A.Fake<ILogger>();
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, messageSender);
            var identifiers = new SecurityIdentifiers("client-1", "sedol-1", "isin-1", "figi-1", "cusip-1", "XCH");
            var tradePosition = new TradePosition(new List<TradeOrderFrame>(), null, null, logger);
            var parameters = new CancelledOrderMessageSenderParameters(identifiers) { TradePosition = tradePosition };

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);

            var exitLoopAt = DateTime.UtcNow.AddSeconds(3);

            while (DateTime.UtcNow < exitLoopAt)
            {
            }

            A.CallTo(() =>
                    messageSender.Send(A<ICancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [Parallelizable]
        public void Send_DiscardsOlderMessagesIfFollowUpWithManyDuplicatesWithinTimePeriod()
        {
            var messageSender = A.Fake<ICancelledOrderMessageSender>();
            var logger = A.Fake<ILogger>();
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, messageSender);
            var identifiers = new SecurityIdentifiers("client-1", "sedol-1", "isin-1", "figi-1", "cusip-1", "XCH");
            var tradePosition = new TradePosition(new List<TradeOrderFrame>(), null, null, logger);
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
                    messageSender.Send(A<ICancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        [Parallelizable]
        public void Send_DiscardsOlderMessagesIfFollowUpWithManyDuplicatesWithinTwoTimePeriods()
        {
            var messageSender = A.Fake<ICancelledOrderMessageSender>();
            var logger = A.Fake<ILogger>();
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, messageSender);
            var identifiers = new SecurityIdentifiers("client-1", "sedol-1", "isin-1", "figi-1", "cusip-1", "XCH");
            var tradePosition = new TradePosition(new List<TradeOrderFrame>(), null, null, logger);
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
                    messageSender.Send(A<ICancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        [Parallelizable]
        public void Send_ConsidersTwoNonIntersectingPositions_InTheSameSecurityAsSeparateMessages()
        {
            var messageSender = A.Fake<ICancelledOrderMessageSender>();
            var logger = A.Fake<ILogger>();
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, messageSender);

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
                    logger);

            var tradePositionAlertTwo =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame2, tradeFrame3, tradeFrame3, tradeFrame4, tradeFrame5 },
                    null,
                    null,
                    logger);

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
                    messageSender.Send(A<ICancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        [Parallelizable]
        public void Send_ConsidersTwoNonIntersectingPositions_InTheSameSecurityAsSeparateMessages_ResendsBothWithDelay()
        {
            var messageSender = A.Fake<ICancelledOrderMessageSender>();
            var logger = A.Fake<ILogger>();
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, messageSender);

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
                    logger);

            var tradePositionAlertTwo =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame2, tradeFrame3, tradeFrame3, tradeFrame4, tradeFrame5 },
                    null,
                    null,
                    logger);

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
                    messageSender.Send(A<ICancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 4);
        }

        [Test]
        [Parallelizable]
        public void Send_ConsidersTwoNonIntersectingPositionsAndTwoIntersectingPositionsForTotalOfThreePositions_InTheSameSecurityAsTwoSeparateMessages()
        {
            var messageSender = A.Fake<ICancelledOrderMessageSender>();
            var logger = A.Fake<ILogger>();
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, messageSender);

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
                    logger);

            var tradePositionAlertTwo =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame1, tradeFrame2, tradeFrame3, tradeFrame4 },
                    null,
                    null,
                    logger);

            var tradePositionAlertThree =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame2, tradeFrame3, tradeFrame4, tradeFrame5 },
                    null,
                    null,
                    logger);

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
                    messageSender.Send(A<ICancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(x => x == 2);
        }

        [Test]
        [Parallelizable]
        public void Send_ConsidersSubsetPositionArrivingAfterToBeDuplicate()
        {
            var messageSender = A.Fake<ICancelledOrderMessageSender>();
            var logger = A.Fake<ILogger>();
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 2 };
            var deduplicator = new CancelledOrderPositionDeDuplicator(ruleConfiguration, messageSender);

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
                    logger);

            var tradePositionAlertTwo =
                new TradePosition(
                    new List<TradeOrderFrame> { tradeFrame1, tradeFrame2, tradeFrame3, tradeFrame3, tradeFrame4, tradeFrame5 },
                    null,
                    null,
                    logger);

            var parameters = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
            { TradePosition = tradePositionAlertOne };

            var parameters2 = new CancelledOrderMessageSenderParameters(tradeFrame1.Security.Identifiers)
            { TradePosition = tradePositionAlertTwo };

            deduplicator.Send(parameters);
            deduplicator.Send(parameters);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);
            deduplicator.Send(parameters2);
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
                    messageSender.Send(A<ICancelledOrderRuleBreach>.Ignored))
                .MustHaveHappenedTwiceExactly();
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
