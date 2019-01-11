using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Configuration;

namespace Surveillance.DataLayer.Tests.Aurora.Market
{
    [TestFixture]
    public class ReddeerMarketTimeBarRepositoryTests
    {
        private ILogger<ReddeerMarketTimeBarRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ReddeerMarketTimeBarRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Save()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=dev-temporary.cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=hackinguser;pwd='WillDelete3101';database=hackingdb1; Allow User Variables=True"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new ReddeerMarketTimeBarRepository(factory, _logger);

            var items = new List<MinuteBarDto>
            {
                new MinuteBarDto
                {
                    Figi = "BBG000C6K6G9",
                    DateTime = DateTime.Now,
                    ExecutionVolume = 12345,
                    ExecutionClosePrice = 1,
                    BestBidClosePrice = 2,
                    BestAskClosePrice = 3
                }
            };

            await repo.Save(items);

            Assert.IsTrue(true);
        }
    }
}
