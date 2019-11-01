namespace Surveillance.Engine.Rules.Tests.Markets
{
    using System;

    using Domain.Core.Markets.Timebars;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets;

    [TestFixture]
    public class IntradayMarketDataResponseTests
    {
        private MarketDataResponse<EquityInstrumentIntraDayTimeBar> _missingMarketDataResponse;

        private MarketDataResponse<EquityInstrumentIntraDayTimeBar> _notMissingMarketDataResponse;

        [Test]
        public void Constructor_Response_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityIntraDayMarketDataResponse(null));
        }

        [Test]
        public void HadMissingData_Returns_Underlying_Response_Missing()
        {
            var response = new EquityIntraDayMarketDataResponse(this._missingMarketDataResponse);

            var result = response.HadMissingData();

            Assert.IsTrue(result);
        }

        [Test]
        public void HadMissingData_Returns_Underlying_Response_NotMissing()
        {
            var response = new EquityIntraDayMarketDataResponse(this._notMissingMarketDataResponse);

            var result = response.HadMissingData();

            Assert.IsFalse(result);
        }

        [SetUp]
        public void Setup()
        {
            this._missingMarketDataResponse =
                new MarketDataResponse<EquityInstrumentIntraDayTimeBar>(null, true, false);
            this._notMissingMarketDataResponse =
                new MarketDataResponse<EquityInstrumentIntraDayTimeBar>(null, false, false);
        }
    }
}