namespace Surveillance.DataLayer.Tests.Aurora.Market
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Firefly.Service.Data.BMLL.Shared.Dtos;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Market;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    [TestFixture]
    public class ReddeerMarketTimeBarRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private ILogger<ReddeerMarketTimeBarRepository> _logger;

        [Test]
        [Explicit]
        public async Task Save()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new ReddeerMarketTimeBarRepository(factory, this._logger);

            var items = new List<MinuteBarDto>
                            {
                                new MinuteBarDto
                                    {
                                        Figi = "BBG000C6K6G9",
                                        DateTime = DateTime.UtcNow,
                                        ExecutionVolume = 12345,
                                        ExecutionClosePrice = 1,
                                        BestBidClosePrice = 2,
                                        BestAskClosePrice = 3
                                    }
                            };

            await repo.Save(items);

            Assert.IsTrue(true);
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._logger = A.Fake<ILogger<ReddeerMarketTimeBarRepository>>();
        }
    }
}