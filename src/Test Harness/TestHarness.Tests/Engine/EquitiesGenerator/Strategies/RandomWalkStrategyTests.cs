using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
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
            var security = new SecurityFrame(
                new Domain.Equity.Security(
                    new Domain.Equity.Security.SecurityId("MSFT"), "Microsoft", "MSFT"),
                    new Spread(new Price(66), new Price(65)),
                    new Volume(200000));

            var result = strategy.AdvanceFrame(security);

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
            var security = new SecurityFrame(
                new Domain.Equity.Security(
                    new Domain.Equity.Security.SecurityId("MSFT"), "Microsoft", "MSFT"),
                    new Spread(new Price(66), new Price(65)),
                    new Volume(200000));

            var printableInitialSecurity = JsonConvert.SerializeObject(security);
            Console.WriteLine(printableInitialSecurity);

            for(var i = 0; i < 99; i++)
            {
                security = strategy.AdvanceFrame(security);

                var printableGeneratedSecurity = JsonConvert.SerializeObject(security);
                Console.WriteLine(printableGeneratedSecurity);

                Assert.IsTrue(security.Spread.Buy.Value >= security.Spread.Sell.Value);
            }
        }
    }
}
