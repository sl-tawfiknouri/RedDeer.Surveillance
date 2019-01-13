using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Tests.Aurora.Market
{
    [TestFixture]
    public class ReddeerMarketRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<ReddeerMarketRepository> _logger;
        private ISystemProcessOperationContext _opCtx;
        private ICfiInstrumentTypeMapper _cfiInstrumentMapper;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = A.Fake<ILogger<ReddeerMarketRepository>>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _cfiInstrumentMapper = new CfiInstrumentTypeMapper();
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Create()
        {
            var factory = new ConnectionStringFactory(_configuration);
            var repo = new ReddeerMarketRepository(factory, _cfiInstrumentMapper, _logger);

            await repo.Create(Frame());

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Get()
        {
            var factory = new ConnectionStringFactory(_configuration);
            var repo = new ReddeerMarketRepository(factory, _cfiInstrumentMapper, _logger);

            await repo.Create(Frame());
            await repo.Create(Frame());

            var results = await repo.Get(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow, _opCtx);

            Assert.IsTrue(true);
        }

        private MarketTimeBarCollection Frame()
        {
            var stockExchange = new DomainV2.Financial.Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var securityIdentifiers = new InstrumentIdentifiers(string.Empty, string.Empty, "stan", "stan", "st12345", "sta123456789", "stan", "sta12345", "stan", "stan", "STAN");

            var security = new FinancialInstrument(
                InstrumentTypes.Equity,
                securityIdentifiers,
                "Standard Chartered",
                "CFI",
                "USD",
                "Standard Chartered Bank");

            var securities = new List<FinancialInstrumentTimeBar>
            {
                new FinancialInstrumentTimeBar(
                    security,
                    new SpreadTimeBar(new CurrencyAmount(100, "GBP"), new CurrencyAmount(101, "GBP"), new CurrencyAmount(100.5m, "GBP"), new Volume(1000)),
                    new DailySummaryTimeBar(
                        1000000,
                        new IntradayPrices(new CurrencyAmount(90, "GBP"), new CurrencyAmount(85, "GBP"), new CurrencyAmount(105, "GBP"), new CurrencyAmount(84, "GBP")),
                        1000,
                        new Volume(10000),
                        DateTime.UtcNow), 
                    DateTime.UtcNow,
                    stockExchange)
            };

            return new MarketTimeBarCollection(stockExchange, DateTime.UtcNow, securities);
        }
    }
}
