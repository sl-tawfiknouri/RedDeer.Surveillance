﻿namespace DataSynchroniser.Api.Factset.Tests.Filters
{
    using DataSynchroniser.Api.Factset.Filters;

    using Domain.Core.Financial.Assets;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    [TestFixture]
    public class MarketDataRequestFilterTests
    {
        [Test]
        public void Filter_Empty_Cfi_Returns_False()
        {
            var filter = this.BuildRequestsFilter();
            var instrument = InstrumentIdentifiers.Null();
            var emptyCfiRequest = new MarketDataRequest(
                "id",
                "XLON",
                null,
                instrument,
                null,
                null,
                null,
                false,
                DataSource.Factset);

            var result = filter.ValidAssetType(emptyCfiRequest);

            Assert.IsFalse(result);
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
        public void Filter_Equity_Cfi_Returns_True(string cfi)
        {
            var filter = this.BuildRequestsFilter();
            var instrument = InstrumentIdentifiers.Null();
            var emptyCfiRequest = new MarketDataRequest(
                "id",
                "XLON",
                cfi,
                instrument,
                null,
                null,
                null,
                false,
                DataSource.Factset);

            var result = filter.ValidAssetType(emptyCfiRequest);

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
        public void Filter_NonEquity_Cfi_Returns_False(string cfi)
        {
            var filter = this.BuildRequestsFilter();
            var instrument = InstrumentIdentifiers.Null();
            var emptyCfiRequest = new MarketDataRequest(
                "id",
                "XLON",
                cfi,
                instrument,
                null,
                null,
                null,
                false,
                DataSource.Factset);

            var result = filter.ValidAssetType(emptyCfiRequest);

            Assert.IsFalse(result);
        }

        [Test]
        public void Filter_Null_Returns_False()
        {
            var filter = this.BuildRequestsFilter();

            var result = filter.ValidAssetType(null);

            Assert.IsFalse(result);
        }

        private FactsetDataRequestFilter BuildRequestsFilter()
        {
            return new FactsetDataRequestFilter();
        }
    }
}