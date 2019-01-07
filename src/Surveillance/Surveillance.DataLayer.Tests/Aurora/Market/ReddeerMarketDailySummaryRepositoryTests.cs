using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Configuration;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Tests.Aurora.Market
{
    [TestFixture]
    public class ReddeerMarketDailySummaryRepositoryTests
    {
        private ILogger<ReddeerMarketDailySummaryRepository> _logger;
        private ISystemProcessOperationContext _opCtx;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ReddeerMarketDailySummaryRepository>>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
        }

        [Test]
        public async Task Save()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=dev-temporary.cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=hackinguser;pwd='WillDelete3101';database=hackingdb1; Allow User Variables=True"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new ReddeerMarketDailySummaryRepository(factory, _logger);
            
            var items = new List<FactsetSecurityDailyResponseItem>
            {
                new FactsetSecurityDailyResponseItem
                {
                    Figi = "BBG000C6K6G9",
                    ClosePrice = 0m,
                    Currency = "usd",
                    OpenPriceUsd = 1m,
                    DailyVolume = 2,
                    Epoch = DateTime.Now.AddDays(1),
                    HighIntradayPrice = 3,
                    LowIntradayPrice = 4,
                    MarketCapitalisationUsd = 5,
                    OpenPrice = 6
                }
            };

            await repo.Save(items);

            Assert.IsTrue(true);
        }
    }
}
