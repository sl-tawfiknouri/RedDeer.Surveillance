using System;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Tests.Aurora.Trade
{
    [TestFixture]
    public class ReddeerTradeRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<OrdersRepository> _logger;
        private ISystemProcessOperationContext _opCtx;
        private IReddeerMarketRepository _marketRepository;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = A.Fake<ILogger<OrdersRepository>>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _marketRepository = A.Fake<IReddeerMarketRepository>();
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Create()
        {
            var factory = new ConnectionStringFactory(_configuration);
            var repo = new OrdersRepository(factory, _marketRepository, _logger);
            var frame = Frame();

            await repo.Create(frame);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Get()
        {
            var factory = new ConnectionStringFactory(_configuration);
            var repo = new OrdersRepository(factory, _marketRepository, _logger);
            var row1 = Frame();
            var row2 = Frame();
            var start = row1.MostRecentDateEvent().Date;
            var end = row2.MostRecentDateEvent().AddDays(1).Date;

            await repo.Create(row1);
            await repo.Create(row2);

            var result = await repo.Get(start, end, _opCtx);

            Assert.IsTrue(true);
        }

        private Order Frame()
        {
            var exch = new DomainV2.Financial.Market("1","XLON", "LSE", MarketTypes.STOCKEXCHANGE);
            var orderDates = DateTime.UtcNow;
            var tradeDates = DateTime.UtcNow;

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    "1",
                    "1",
                    null,
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

            var trade1 = new DealerOrder(
                security,
                null, 
                "my-trade",
                tradeDates,
                tradeDates,
                tradeDates,
                tradeDates,
                tradeDates,
                tradeDates,
                "trader-1",
                "sum-notes",
                "counter-party",
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency(null),
                OrderCleanDirty.CLEAN,
                12,
                "v1",
                "link-12345",
                "grp-1",
                new CurrencyAmount(100, "GBP"), 
                new CurrencyAmount(100, "GBP"),
                1000, 
                1000, 
                null,
                null,
                OptionEuropeanAmerican.NONE);

            var trade2 = new DealerOrder(
                security,
                null,
                "my-trade-2",
                tradeDates,
                tradeDates,
                tradeDates,
                tradeDates,
                tradeDates, 
                tradeDates, 
                "trader-2", 
                "sum-notes",
                "counter-party",
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("GBP"),
                OrderCleanDirty.CLEAN,
                15,
                "v1",
                "link-12345",
                "grp-1",
                new CurrencyAmount(100, "GBP"),
                new CurrencyAmount(100, "GBP"),
                1000,
                1000,
                null,
                null,
                OptionEuropeanAmerican.NONE);

            var order2 = new Order(
                security,
                exch, 
                null,
                "order-1",
                "order-v1",
                "order-v1-link",
                "order-group-v1",
                orderDates,
                orderDates,
                orderDates, 
                orderDates,
                orderDates,
                orderDates,               
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("USD"),
                OrderCleanDirty.CLEAN,
                null,
                new CurrencyAmount(100, "GBP"), 
                new CurrencyAmount(100, "GBP"),
                1000, 
                1000,
                "trader-1",
                "trader one",
                "clearing-agent",
                "deal asap",
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new [] { trade1, trade2 });

            return order2;
        }
    }
}
