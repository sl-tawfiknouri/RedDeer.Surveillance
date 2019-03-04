using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Equity.TimeBars;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

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
        public void Create()
        {
            var factory = new ConnectionStringFactory(_configuration);
            var repo = new ReddeerMarketRepository(factory, _cfiInstrumentMapper, _logger);

            repo.Create(Frame());

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Get()
        {
            var factory = new ConnectionStringFactory(_configuration);
            var repo = new ReddeerMarketRepository(factory, _cfiInstrumentMapper, _logger);

            repo.Create(Frame());
            repo.Create(Frame());

            var results = await repo.GetEquityIntraday(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow, _opCtx);

            Assert.IsTrue(true);
        }

        private EquityIntraDayTimeBarCollection Frame()
        {
            var stockExchange = new Domain.Financial.Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var securityIdentifiers = new InstrumentIdentifiers(string.Empty, string.Empty, "stan", "stan", "st12345", "sta123456789", "stan", "sta12345", "stan", "stan", "STAN");

            var security = new FinancialInstrument(
                InstrumentTypes.Equity,
                securityIdentifiers,
                "Standard Chartered",
                "CFI",
                "USD",
                "Standard Chartered Bank");

            var securities = new List<EquityInstrumentIntraDayTimeBar>
            {
                new EquityInstrumentIntraDayTimeBar(
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

            return new EquityIntraDayTimeBarCollection(stockExchange, DateTime.UtcNow, securities);
        }
    }
}
