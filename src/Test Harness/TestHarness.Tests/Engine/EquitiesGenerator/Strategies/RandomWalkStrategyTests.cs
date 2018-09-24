using Newtonsoft.Json;
using NUnit.Framework;
using System;
using Domain.Equity;
using Domain.Equity.Frames;
using TestHarness.Engine.EquitiesGenerator.Strategies;

namespace TestHarness.Tests.Engine.EquitiesGenerator.Strategies
{
    [TestFixture]
    public class RandomWalkStrategyTests
    {
        [TestCase(1)]
        [TestCase(1.1)]
        [TestCase(200)]
        [TestCase(-200)]
        [TestCase(-0.0001)]
        public void Constructor_ConsidersOutOfRangeMaxSpread_ToBeOutOfRange(decimal maxSpread)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentOutOfRangeException>(() => new MarkovEquityStrategy(0, 1, maxSpread));
        }

        [Test]
        public void TickSecurity_ReturnsNullTick_ForNullArgument()
        {
            var strategy = new MarkovEquityStrategy();

            var result = strategy.AdvanceFrame(null);

            Assert.IsNull(result);
        }

        [Test]
        public void TickSecurity_UpdatesWithNewTickData()
        {
            var strategy = new MarkovEquityStrategy();
            var identifiers = new SecurityIdentifiers("MSFT", "MS12345", "MSF123456789", "MSFT", "MSF12341234", "MSFT", "MSFT", "MSFT");
            var security = new Security(identifiers, "Microsoft", "CFI", "Microsoft Company");
            var spread = new Spread(new Price(66, "GBP"), new Price(65, "GBP"), new Price(65, "GBP"));

            var tick =
                new SecurityTick(
                    security,
                    spread,
                    new Volume(200000),
                    DateTime.UtcNow,
                    3000,
                    null,
                    100);

            var result = strategy.AdvanceFrame(tick);

            var printableInitialSecurity = JsonConvert.SerializeObject(security);
            var printableGeneratedSecurity = JsonConvert.SerializeObject(result);

            Console.WriteLine(printableInitialSecurity);
            Console.WriteLine(printableGeneratedSecurity);

            Assert.AreEqual(result.Security.Name, "Microsoft");
            Assert.IsFalse(ReferenceEquals(security, result));
        }

        [Test]
        [Explicit]
        public void TickSecurity_UpdatesWithNewTickData_Printing100IterationWalk()
        {
            var strategy = new MarkovEquityStrategy();
            var identifiers = new SecurityIdentifiers("MSFT", "MS12345", "MSF123456789", "MSFT", "MSF12341234", "MSFT", "MSFT", "MSFT");
            var security = new Security(identifiers, "Microsoft", "CFI", "Microsoft Company");
            var spread = new Spread(new Price(66, "GBP"), new Price(65, "GBP"), new Price(65, "GBP"));

            var tick =
                new SecurityTick(
                    security,
                    spread,
                    new Volume(200000),
                    DateTime.UtcNow,
                    3000,
                    null,
                    100);

            var printableInitialSecurity = JsonConvert.SerializeObject(security);
            Console.WriteLine(printableInitialSecurity);

            for(var i = 0; i < 99; i++)
            {
                tick = strategy.AdvanceFrame(tick);

                var printableGeneratedSecurity = JsonConvert.SerializeObject(security);
                Console.WriteLine(printableGeneratedSecurity);

                Assert.IsTrue(tick.Spread.Bid.Value >= tick.Spread.Ask.Value);
            }
        }
    }
}
