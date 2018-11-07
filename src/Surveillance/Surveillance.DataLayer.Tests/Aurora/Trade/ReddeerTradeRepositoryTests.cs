using System;
using System.Threading.Tasks;
using Domain.Equity;
using Domain.Market;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
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
                AuroraConnectionString = "server=dev-surveillance.cluster-cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=reddeer;pwd='=6CCkoJb2b+HtKg9';database=dev_surveillance"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new ReddeerTradeRepository(factory, _marketRepository, _logger);

            await repo.Create(Frame());

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
            var start = row1.StatusChangedOn.Date;
            var end = row2.StatusChangedOn.AddDays(1).Date;

            await repo.Create(row1);
            await repo.Create(row2);
            var result = await repo.Get(start, end, _opCtx);

            Assert.IsTrue(true);
        }

        private TradeOrderFrame Frame()
        {
            var exch = new StockExchange(new Domain.Market.Market.MarketId("id"), "LSE");
            var orderDates = DateTime.Now;
            const string traderId = "Trader Joe";
            const string partyBrokerId = "Broker-1";
            const string accountId = "Account-1";
            const string dealerInstruction = "Trade ASAP";
            const string tradeRationale = "Market is not pricing well";
            const string tradeStrategy = "Unknown";
            const string counterPartyBrokerId = "Broker-2";
            var securityIdentifiers = new SecurityIdentifiers(string.Empty, "stan", "stan", "st12345", "sta123456789", "stan", "sta12345", "stan", "stan", "STAN");

            var security = new Security(
                securityIdentifiers,
                "Standard Chartered",
                "CFI",
                "Standard Chartered Bank");

            var order1 = new TradeOrderFrame(
                null,
                OrderType.Limit,
                exch,
                security,
                new Price(100, "GBX"),
                new Price(100, "GBX"),
                1000,
                1000,
                OrderPosition.Buy,
                OrderStatus.Booked,
                orderDates,
                orderDates,
                traderId,
                string.Empty,
                accountId,
                dealerInstruction,
                partyBrokerId,
                counterPartyBrokerId,
                tradeRationale,
                tradeStrategy,
                "GBX");

            return order1;
        }
    }
}
