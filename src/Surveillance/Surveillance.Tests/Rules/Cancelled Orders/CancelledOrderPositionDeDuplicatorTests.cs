using System;
using System.Collections.Generic;
using Domain.Equity;
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
            var ruleConfiguration = new RuleConfiguration { CancelledOrderDeduplicationDelaySeconds = 5 };

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


    }
}
