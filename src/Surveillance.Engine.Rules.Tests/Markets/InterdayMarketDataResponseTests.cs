using System;
using Domain.Equity.TimeBars;
using Domain.Markets;
using NUnit.Framework;
using Surveillance.Engine.Rules.Markets;

namespace Surveillance.Engine.Rules.Tests.Markets
{
    [TestFixture]
    public class InterdayMarketDataResponseTests
    {
        private static MarketDataResponse<EquityInstrumentInterDayTimeBar> _responseMissingData;
        private static MarketDataResponse<EquityInstrumentInterDayTimeBar> _responseNoMissingData;

        [SetUp]
        public void Setup()
        {
            _responseMissingData = new MarketDataResponse<EquityInstrumentInterDayTimeBar>(null, true, false);
            _responseNoMissingData = new MarketDataResponse<EquityInstrumentInterDayTimeBar>(null, false, false);
        }

        [Test]
        public void Constructor_Response_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new InterdayMarketDataResponse(null));
        }

        [Test]
        public void HadMissingData_Passes_Through_Response_Value_Missing()
        {
            var response = new InterdayMarketDataResponse(_responseMissingData);

            var missing = response.HadMissingData();

            Assert.IsTrue(missing);
        }

        [Test]
        public void HadMissingData_Passes_Through_Response_Value_Not_Missing()
        {
            var response = new InterdayMarketDataResponse(_responseNoMissingData);

            var missing = response.HadMissingData();

            Assert.IsFalse(missing);
        }

    }
}
