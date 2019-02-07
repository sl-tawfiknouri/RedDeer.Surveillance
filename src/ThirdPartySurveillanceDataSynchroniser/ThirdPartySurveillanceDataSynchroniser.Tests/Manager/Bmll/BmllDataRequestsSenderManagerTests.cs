﻿using System;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.DataSources;
using ThirdPartySurveillanceDataSynchroniser.Manager;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Manager.Bmll
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
            var requests = new List<MarketDataRequestDataSource>()
            {
                new MarketDataRequestDataSource(
                    DataSource.Bmll,
                    new MarketDataRequest(
                        "XLON",
                        "ENTSPB",
                        new InstrumentIdentifiers
                        {
                            Figi = "BBG000C6K6G9"
                        },
                        new DateTime(2018, 01, 01),
                        new DateTime(2018, 01, 06),
                        "1")),
                new MarketDataRequestDataSource(
                    DataSource.Bmll,
                    new MarketDataRequest(
                        "XLON",
                        "ENTSPB",
                        new InstrumentIdentifiers
                        {
                            Figi = "BBG000C6K6G9"
                        },
                        new DateTime(2018, 01, 01),
                        new DateTime(2018, 01, 06),
                        "1")),
            };

            var senderManager = new BmllDataRequestsSenderManager(_timeBarRepository, _logger);

            var result = senderManager.ProjectToRequestKeys(requests);

            Assert.AreEqual(result.Count, 7);
        }
    }
}
