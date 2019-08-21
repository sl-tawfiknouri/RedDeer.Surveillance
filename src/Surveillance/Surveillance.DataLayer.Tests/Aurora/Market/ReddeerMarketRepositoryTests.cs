namespace Surveillance.DataLayer.Tests.Aurora.Market
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Cfis;
    using Domain.Core.Financial.Cfis.Interfaces;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Market;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    [TestFixture]
    public class ReddeerMarketRepositoryTests
    {
        private ICfiInstrumentTypeMapper _cfiInstrumentMapper;

        private IDataLayerConfiguration _configuration;

        private ILogger<ReddeerMarketRepository> _logger;

        private ISystemProcessOperationContext _opCtx;

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public void Create()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new ReddeerMarketRepository(factory, this._cfiInstrumentMapper, this._logger);

            repo.Create(this.Frame());

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Get()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new ReddeerMarketRepository(factory, this._cfiInstrumentMapper, this._logger);

            repo.Create(this.Frame());
            repo.Create(this.Frame());

            var results = await repo.GetEquityIntraday(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow, this._opCtx);

            Assert.IsTrue(true);
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._logger = A.Fake<ILogger<ReddeerMarketRepository>>();
            this._opCtx = A.Fake<ISystemProcessOperationContext>();
            this._cfiInstrumentMapper = new CfiInstrumentTypeMapper();
        }

        private EquityIntraDayTimeBarCollection Frame()
        {
            var stockExchange = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var securityIdentifiers = new InstrumentIdentifiers(
                string.Empty,
                string.Empty,
                "stan",
                "stan",
                "st12345",
                "sta123456789",
                "stan",
                "sta12345",
                "stan",
                "stan",
                "STAN");

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
                                         new SpreadTimeBar(
                                             new Money(100, "GBP"),
                                             new Money(101, "GBP"),
                                             new Money(100.5m, "GBP"),
                                             new Volume(1000)),
                                         new DailySummaryTimeBar(
                                             1000000,
                                             new IntradayPrices(
                                                 new Money(90, "GBP"),
                                                 new Money(85, "GBP"),
                                                 new Money(105, "GBP"),
                                                 new Money(84, "GBP")),
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