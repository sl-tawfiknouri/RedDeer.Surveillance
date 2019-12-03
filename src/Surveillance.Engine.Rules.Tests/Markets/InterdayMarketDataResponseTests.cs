namespace Surveillance.Engine.Rules.Tests.Markets
{
    using System;

    using Domain.Core.Markets.Timebars;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets;

    [TestFixture]
    public class InterdayMarketDataResponseTests
    {
        private static MarketDataResponse<EquityInstrumentInterDayTimeBar> _responseMissingData;

        private static MarketDataResponse<EquityInstrumentInterDayTimeBar> _responseNoMissingData;

        [Test]
        public void Constructor_Response_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityInterDayMarketDataResponse(null));
        }

        [Test]
        public void HadMissingData_Passes_Through_Response_Value_Missing()
        {
            var response = new EquityInterDayMarketDataResponse(_responseMissingData);

            var missing = response.HadMissingData();

            Assert.IsTrue(missing);
        }

        [Test]
        public void HadMissingData_Passes_Through_Response_Value_Not_Missing()
        {
            var response = new EquityInterDayMarketDataResponse(_responseNoMissingData);

            var missing = response.HadMissingData();

            Assert.IsFalse(missing);
        }

        [SetUp]
        public void Setup()
        {
            _responseMissingData = new MarketDataResponse<EquityInstrumentInterDayTimeBar>(null, true, false);
            _responseNoMissingData = new MarketDataResponse<EquityInstrumentInterDayTimeBar>(null, false, false);
        }
    }
}