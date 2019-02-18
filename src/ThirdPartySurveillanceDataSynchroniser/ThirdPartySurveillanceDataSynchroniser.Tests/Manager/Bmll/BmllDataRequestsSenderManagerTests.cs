using System;
using System.Collections.Generic;
using DataSynchroniser.Manager.Bmll;
using Domain.Financial;
using Domain.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;

namespace DataSynchroniser.Tests.Manager.Bmll
{
    [TestFixture]
    public class BmllDataRequestsSenderManagerTests
    {
        private IBmllTimeBarApiRepository _timeBarRepository;
        private ILogger<BmllDataRequestsSenderManager> _logger;

        [SetUp]
        public void Setup()
        {
            _timeBarRepository = A.Fake<IBmllTimeBarApiRepository>();
            _logger = A.Fake<ILogger<BmllDataRequestsSenderManager>>();
        }

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
                        "1"),
                    new MarketDataRequest(
                        "XLON",
                        "ENTSPB",
                        new InstrumentIdentifiers
                        {
                            Figi = "BBG000C6K6G9"
                        },
                        new DateTime(2018, 01, 01),
                        new DateTime(2018, 01, 06),
                        "1"),
            };

            var senderManager = new BmllDataRequestsSenderManager(_timeBarRepository, _logger);

            var result = senderManager.ProjectToRequestKeys(requests);

            Assert.AreEqual(result.Count, 7);
        }
    }
}
