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
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Tests.Aurora.Trade
{
    [TestFixture]
    public class ReddeerTradeRepositoryTests
    {
        private ILogger<ReddeerTradeRepository> _logger;
        private ISystemProcessOperationContext _opCtx;
        private IReddeerMarketRepository _marketRepository;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ReddeerTradeRepository>>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _marketRepository = A.Fake<IReddeerMarketRepository>();
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Create()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new ReddeerTradeRepository(factory, _marketRepository, _logger);
            var frame = Frame();

            await repo.Create(frame);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Get()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString =
                    "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new ReddeerTradeRepository(factory, _marketRepository, _logger);
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
            var exch = new DomainV2.Financial.Market("1","id", "LSE", MarketTypes.STOCKEXCHANGE);
            var orderDates = DateTime.Now;

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    "1",
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
                "Standard Chartered Bank");

            var order2 = new Order(security, exch, null, "order-1", orderDates, orderDates, orderDates, orderDates,
                orderDates, orderDates, OrderTypes.MARKET, OrderPositions.BUY, new Currency("GBP"),
                new CurrencyAmount(100, "GBP"), new CurrencyAmount(100, "GBP"), 1000, 1000, "Mr Portfolio Manager",
                "Ry-1", "Goldman Sachs", "Clear Bank", "Process ASAP", "Clever Strategy", "Order rationale",
                "Order fund", "client-account-x", new DomainV2.Trading.Trade[0]);

            return order2;
        }
    }
}
