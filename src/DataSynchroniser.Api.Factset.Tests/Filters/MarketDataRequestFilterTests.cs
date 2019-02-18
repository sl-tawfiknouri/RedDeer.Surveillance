using DataSynchroniser.Api.Factset.Filters;
using Domain.Financial;
using Domain.Markets;
using NUnit.Framework;

namespace DataSynchroniser.Api.Factset.Tests.Filters
{
    [TestFixture]
    public class MarketDataRequestFilterTests
    {
        [Test]
        public void Filter_Null_Returns_False()
        {
            var filter = Build();

            var result = filter.Filter(null);

            Assert.IsTrue(result);
        }

        [Test]
        public void Filter_Empty_Cfi_Returns_True()
        {
            var filter = Build();
            var instrument = InstrumentIdentifiers.Null();
            var emptyCfiRequest = new MarketDataRequest("id", "XLON", null, instrument, null, null, null, false);

            var result = filter.Filter(emptyCfiRequest);

            Assert.IsTrue(result);
        }

        [TestCase("D")]
        [TestCase("C")]
        [TestCase("R")]
        [TestCase("O")]
        [TestCase("F")]
        [TestCase("S")]
        [TestCase("H")]
        [TestCase("I")]
        [TestCase("J")]
        [TestCase("K")]
        [TestCase("L")]
        [TestCase("T")]
        [TestCase("M")]
        public void Filter_NonEquity_Cfi_Returns_True(string cfi)
        {
            var filter = Build();
            var instrument = InstrumentIdentifiers.Null();
            var emptyCfiRequest = new MarketDataRequest("id", "XLON", cfi, instrument, null, null, null, false);

            var result = filter.Filter(emptyCfiRequest);

            Assert.IsTrue(result);
        }

        [TestCase("e")]
        [TestCase("E")]
        [TestCase("ES")]
        [TestCase("eS")]
        [TestCase("EP")]
        [TestCase("EC")]
        [TestCase("EF")]
        [TestCase("EL")]
        [TestCase("ED")]
        [TestCase("EY")]
        [TestCase("EM")]
        public void Filter_Equity_Cfi_Returns_False(string cfi)
        {
            var filter = Build();
            var instrument = InstrumentIdentifiers.Null();
            var emptyCfiRequest = new MarketDataRequest("id", "XLON", cfi, instrument, null, null, null, false);

            var result = filter.Filter(emptyCfiRequest);

            Assert.IsFalse(result);
        }

        private MarketDataRequestFilter Build()
        {
            return new MarketDataRequestFilter();
        }
    }
}
