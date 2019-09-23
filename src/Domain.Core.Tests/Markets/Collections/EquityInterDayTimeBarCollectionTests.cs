namespace Domain.Core.Tests.Markets.Collections
{
    using System;

    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;

    using NUnit.Framework;

    [TestFixture]
    public class EquityInterDayTimeBarCollectionTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var market = new Market("1", "xlon", "london stock exchange", MarketTypes.DarkPool);
            var date = DateTime.UtcNow;
            var timeBars = new EquityInstrumentInterDayTimeBar[0];

            var coll = new EquityInterDayTimeBarCollection(market, date, timeBars);

            Assert.AreEqual(market, coll.Exchange);
            Assert.AreEqual(date, coll.Epoch);
            Assert.AreEqual(timeBars, coll.Securities);
        }

        [Test]
        public void Ctor_DoesNotThrow_WithNulls()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new EquityInterDayTimeBarCollection(null, DateTime.UtcNow, null));
        }

        [Test]
        public void ToString_PrintsOutExpected_ExchangeAndSecurities()
        {
            var market = new Market("1", "xlon", "london stock exchange", MarketTypes.DarkPool);
            var date = DateTime.UtcNow;
            var timeBars = new EquityInstrumentInterDayTimeBar[0];
            var coll = new EquityInterDayTimeBarCollection(market, date, timeBars);

            var result = coll.ToString();

            Assert.AreEqual("Exchange (xlon.london stock exchange) Securities(0)", result);
        }
    }
}