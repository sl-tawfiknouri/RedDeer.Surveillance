using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Equity;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Configuration;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Tests.Aurora.Market
{
    [TestFixture]
    public class ReddeerMarketRepositoryTests
    {
        private ILogger<ReddeerMarketRepository> _logger;
        private ISystemProcessOperationContext _opCtx;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ReddeerMarketRepository>>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Create()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=dev-surveillance.cluster-cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=reddeer;pwd='=6CCkoJb2b+HtKg9';database=dev_surveillance; Allow User Variables=True"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new ReddeerMarketRepository(factory, _logger);

            await repo.Create(Frame());

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Get()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=dev-surveillance.cluster-cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=reddeer;pwd='=6CCkoJb2b+HtKg9';database=dev_surveillance; Allow User Variables=True"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new ReddeerMarketRepository(factory, _logger);

            await repo.Create(Frame());
            await repo.Create(Frame());

            var results = await repo.Get(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow, _opCtx);

            Assert.IsTrue(true);
        }

        private ExchangeFrame Frame()
        {
            var stockExchange = new Market(new Domain.Market.Market.MarketId("XLON"), "London Stock Exchange");

            var securityIdentifiers = new SecurityIdentifiers(string.Empty, "stan", "stan", "st12345", "sta123456789", "stan", "sta12345", "stan", "stan", "STAN");

            var security = new Security(
                securityIdentifiers,
                "Standard Chartered",
                "CFI",
                "Standard Chartered Bank");

            var securities = new List<SecurityTick>
            {
                new SecurityTick(
                    security,
                    new Spread(new CurrencyAmount(100, "GBP"), new CurrencyAmount(101, "GBP"), new CurrencyAmount(100.5m, "GBP")),
                    new Volume(1000),
                    new Volume(10000),
                    DateTime.UtcNow,
                    1000000,
                    new IntradayPrices(new CurrencyAmount(90, "GBP"), new CurrencyAmount(85, "GBP"), new CurrencyAmount(105, "GBP"), new CurrencyAmount(84, "GBP")),
                    1000,
                    stockExchange)
            };

            return new ExchangeFrame(stockExchange, DateTime.UtcNow, securities);
        }
    }
}
