namespace Surveillance.DataLayer.Tests.Aurora.Market
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Market;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    [TestFixture]
    public class ReddeerMarketDailySummaryRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private ILogger<ReddeerMarketDailySummaryRepository> _logger;

        [Test]
        [Explicit]
        public async Task Save()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new ReddeerMarketDailySummaryRepository(factory, this._logger);

            var items = new List<FactsetSecurityDailyResponseItem>
                            {
                                new FactsetSecurityDailyResponseItem
                                    {
                                        Figi = "BBG000C6K6G9",
                                        ClosePrice = 0m,
                                        Currency = "usd",
                                        DailyVolume = 2,
                                        Epoch = DateTime.UtcNow.AddDays(1),
                                        HighIntradayPrice = 3,
                                        LowIntradayPrice = 4,
                                        MarketCapitalisationUsd = 5,
                                        OpenPrice = 6,
                                        MarketCapitalisation = 7
                                    }
                            };

            await repo.Save(items);

            Assert.IsTrue(true);
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._logger = A.Fake<ILogger<ReddeerMarketDailySummaryRepository>>();
        }
    }
}