using Newtonsoft.Json;
using NUnit.Framework;
using System;
using DomainV2.Equity;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
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

            var result = strategy.AdvanceFrame(null, DateTime.UtcNow, true);

            Assert.IsNull(result);
        }

        [Test]
        public void TickSecurity_UpdatesWithNewTickData()
        {
            var strategy = new MarkovEquityStrategy();
            var identifiers = new InstrumentIdentifiers(string.Empty, string.Empty, "MSFT","MSFT", "MS12345", "MSF123456789", "MSFT", "MSF12341234", "MSFT", "MSFT", "MSFT");
            var security = new FinancialInstrument(InstrumentTypes.Equity, identifiers, "Microsoft", "CFI", "USD", "Microsoft Company");
            var spread = new SpreadTimeBar(new CurrencyAmount(66, "GBP"), new CurrencyAmount(65, "GBP"), new CurrencyAmount(65, "GBP"), new Volume(20000));

            var tick =
                new EquityInstrumentIntraDayTimeBar(
                    security,
                    spread,
                    new DailySummaryTimeBar(
                        1000,
                        null,
                        1000,
                        new Volume(20000),
                        DateTime.UtcNow),
                    DateTime.UtcNow,
                    new Market("1", "NASDAQ", "NASDAQ", MarketTypes.STOCKEXCHANGE));

            var result = strategy.AdvanceFrame(tick, DateTime.UtcNow, true);

            var printableInitialSecurity = JsonConvert.SerializeObject(security);
            var printableGeneratedSecurity = JsonConvert.SerializeObject(result);

            Console.WriteLine(printableInitialSecurity);
            Console.WriteLine(printableGeneratedSecurity);

            Assert.AreEqual(result.Security.Name, "Microsoft");
        }

        [Test]
        [Explicit]
        public void TickSecurity_UpdatesWithNewTickData_Printing100IterationWalk()
        {
            var strategy = new MarkovEquityStrategy();
            var identifiers = new InstrumentIdentifiers(string.Empty, string.Empty, string.Empty, "MSFT", "MS12345", "MSF123456789", "MSFT", "MSF12341234", "MSFT", "MSFT", "MSFT");
            var security = new FinancialInstrument(InstrumentTypes.Equity, identifiers, "Microsoft", "CFI", "USD", "Microsoft Company");
            var spread = new SpreadTimeBar(new CurrencyAmount(66, "GBP"), new CurrencyAmount(65, "GBP"), new CurrencyAmount(65, "GBP"), new Volume(200000));

            var tick =
                new EquityInstrumentIntraDayTimeBar(
                    security,
                    spread,
                    new DailySummaryTimeBar(
                        1000,
                        null,
                        1000,
                        new Volume(200000),
                        DateTime.UtcNow),
                    DateTime.UtcNow,
                    new Market("1", "NASDAQ", "NASDAQ", MarketTypes.STOCKEXCHANGE));

            var printableInitialSecurity = JsonConvert.SerializeObject(security);
            Console.WriteLine(printableInitialSecurity);

            for(var i = 0; i < 99; i++)
            {
                tick = strategy.AdvanceFrame(tick, DateTime.UtcNow, true);

                var printableGeneratedSecurity = JsonConvert.SerializeObject(security);
                Console.WriteLine(printableGeneratedSecurity);

                Assert.IsTrue(tick.SpreadTimeBar.Bid.Value >= tick.SpreadTimeBar.Ask.Value);
            }
        }
    }
}
