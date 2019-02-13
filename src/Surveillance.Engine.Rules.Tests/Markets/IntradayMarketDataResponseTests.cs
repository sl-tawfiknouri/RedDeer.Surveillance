using System;
using DomainV2.Equity.TimeBars;
using DomainV2.Markets;
using NUnit.Framework;
using Surveillance.Engine.Rules.Markets;

namespace Surveillance.Engine.Rules.Tests.Markets
{
    [TestFixture]
    public class IntradayMarketDataResponseTests
    {
        private MarketDataResponse<EquityInstrumentIntraDayTimeBar> _missingMarketDataResponse;
        private MarketDataResponse<EquityInstrumentIntraDayTimeBar> _notMissingMarketDataResponse;

        [SetUp]
        public void Setup()
        {
            _missingMarketDataResponse = new MarketDataResponse<EquityInstrumentIntraDayTimeBar>(null, true, false);
            _notMissingMarketDataResponse = new MarketDataResponse<EquityInstrumentIntraDayTimeBar>(null, false, false);
        }

        [Test]
        public void Constructor_Response_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new IntradayMarketDataResponse(null));
        }

        [Test]
        public void HadMissingData_Returns_Underlying_Response_Missing()
        {
            var response = new IntradayMarketDataResponse(_missingMarketDataResponse);

            var result = response.HadMissingData();

            Assert.IsTrue(result);
        }

        [Test]
        public void HadMissingData_Returns_Underlying_Response_NotMissing()
        {
            var response = new IntradayMarketDataResponse(_notMissingMarketDataResponse);

            var result = response.HadMissingData();

            Assert.IsFalse(result);
        }
    }
}
