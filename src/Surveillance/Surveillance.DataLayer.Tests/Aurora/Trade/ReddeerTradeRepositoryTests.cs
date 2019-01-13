﻿using System;
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
        private ILogger<ReddeerOrdersRepository> _logger;
        private ISystemProcessOperationContext _opCtx;
        private IReddeerMarketRepository _marketRepository;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ReddeerOrdersRepository>>();
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
            var repo = new ReddeerOrdersRepository(factory, _marketRepository, _logger);
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
            var repo = new ReddeerOrdersRepository(factory, _marketRepository, _logger);
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
            var exch = new DomainV2.Financial.Market("3","id", "LSE", MarketTypes.STOCKEXCHANGE);
            var orderDates = DateTime.Now;
            var tradeDates = DateTime.Now;

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    "1",
                    "7",
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
                OrderCleanDirty.Clean,
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
                OptionEuropeanAmerican.None);

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
                OrderCleanDirty.Clean,
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
                OptionEuropeanAmerican.None);

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
                OrderCleanDirty.Clean,
                null,
                new CurrencyAmount(100, "GBP"), 
                new CurrencyAmount(100, "GBP"),
                1000, 
                1000,
                "trader-1",
                "clearing-agent",
                "deal asap",
                null,
                null,
                OptionEuropeanAmerican.None,
                new [] { trade1, trade2 });

            return order2;
        }
    }
}
