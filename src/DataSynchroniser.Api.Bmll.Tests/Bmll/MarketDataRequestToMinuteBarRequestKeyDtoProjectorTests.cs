using System;
using System.Collections.Generic;
using DataSynchroniser.Api.Bmll.Bmll;
using Domain.Core.Financial.Assets;
using NUnit.Framework;
using SharedKernel.Contracts.Markets;

namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    [TestFixture]
    public class MarketDataRequestToMinuteBarRequestKeyDtoProjectorTests
    {
        [Test]
        public void ProjectToRequestKeys_Returns_Expected_MinuteBarKeys()
        {
            var requests = new List<MarketDataRequest>()
            {
                new MarketDataRequest(
                    "XLON",
                    "ENTSPB",
                    new InstrumentIdentifiers
                    {
                        Figi = "BBG000C6K6G9"
                    },
                    new DateTime(2018, 01, 01),
                    new DateTime(2018, 01, 06),
                    "1",
                    DataSource.All),
                new MarketDataRequest(
                    "XLON",
                    "ENTSPB",
                    new InstrumentIdentifiers
                    {
                        Figi = "BBG000C6K6G9"
                    },
                    new DateTime(2018, 01, 01),
                    new DateTime(2018, 01, 06),
                    "1",
                    DataSource.All),
            };

            var keyProjector = new MarketDataRequestToMinuteBarRequestKeyDtoProjector();

            var result = keyProjector.ProjectToRequestKeys(requests);

            Assert.AreEqual(result.Count, 7);
        }
    }
}
