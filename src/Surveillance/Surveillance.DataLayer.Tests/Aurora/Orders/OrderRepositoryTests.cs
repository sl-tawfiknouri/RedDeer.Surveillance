namespace Surveillance.DataLayer.Tests.Aurora.Orders
{
    using System;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Cfis;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;
    using Domain.Core.Trading.Orders.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Market;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    [TestFixture]
    public class OrderRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private IConnectionStringFactory _connectionStringFactory;

        private ILogger<OrdersRepository> _logger;

        private IReddeerMarketRepository _marketRepository;

        private ISystemProcessOperationContext _opCtx;

        private IOrderBrokerRepository _orderBrokerRepository;

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Create()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new OrdersRepository(factory, this._marketRepository, this._orderBrokerRepository, this._logger);
            var frame = this.Frame();

            await repo.Create(frame);

            Assert.IsTrue(true);
        }

        [Test]
        public void Create_NullOrder_DoesNotThrow()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new OrdersRepository(factory, this._marketRepository, this._orderBrokerRepository, this._logger);

            Assert.DoesNotThrowAsync(() => repo.Create(null));
        }

        [Test]
        [Explicit]
        public async Task Create_OrderMultipleInsert_DoesNotThrow()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var marketRepository = new ReddeerMarketRepository(
                factory,
                new CfiInstrumentTypeMapper(),
                new NullLogger<ReddeerMarketRepository>());
            var repo = new OrdersRepository(factory, marketRepository, this._orderBrokerRepository, this._logger);

            var securityIdentifiers1 = new InstrumentIdentifiers(
                null,
                null,
                null,
                "6657789",
                "6657789",
                null,
                null,
                null,
                null,
                null,
                "STAN1");

            var security1 = new FinancialInstrument(
                InstrumentTypes.Equity,
                securityIdentifiers1,
                "Standard Chartered",
                "CFI",
                "USD",
                "Standard Chartered Bank");

            var securityIdentifiers2 = new InstrumentIdentifiers(
                null,
                null,
                null,
                "B00KT68",
                "B00KT68",
                null,
                null,
                null,
                null,
                null,
                "STAN1");

            var security2 = new FinancialInstrument(
                InstrumentTypes.Equity,
                securityIdentifiers2,
                "Standard Chartered",
                "CFI",
                "USD",
                "Standard Chartered Bank");

            var securityIdentifiers3 = new InstrumentIdentifiers(
                null,
                null,
                null,
                "B00KT68",
                "B00KT68",
                null,
                null,
                null,
                null,
                null,
                "STAN1");

            var security3 = new FinancialInstrument(
                InstrumentTypes.Equity,
                securityIdentifiers3,
                "Standard Chartered",
                "CFI",
                "USD",
                "Standard Chartered Bank");

            var exch = new Market(null, "NA", "NA", MarketTypes.STOCKEXCHANGE);

            var order1 = this.OrderMultiple(security1, exch);
            var order2 = this.OrderMultiple(security2, exch);
            var order3 = this.OrderMultiple(security3, exch);
            var order4 = this.OrderMultiple(security1, exch);

            await repo.Create(order1);
            await repo.Create(order2);
            await repo.Create(order3);
            await repo.Create(order4);

            var result1 = await repo.Get(order1.PlacedDate.Value, order3.PlacedDate.Value, this._opCtx);

            Assert.AreEqual(result1.Count, 3);
        }

        [Test]
        public void Create_OrderNoBrokerInsert_DoesNotThrow()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new OrdersRepository(factory, this._marketRepository, this._orderBrokerRepository, this._logger);
            var frame = this.Frame();
            frame.OrderBroker = null;

            Assert.DoesNotThrowAsync(() => repo.Create(frame));

            A.CallTo(() => this._orderBrokerRepository.InsertOrUpdateBroker(A<IOrderBroker>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Creates_LiveUnscheduledOrders()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new OrdersRepository(factory, this._marketRepository, this._orderBrokerRepository, this._logger);

            var result = await repo.LiveUnscheduledOrders();

            Assert.IsTrue(true);
        }

        [Test]
        public void Ctor_ConnectionStringFactoryNull_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new OrdersRepository(null, this._marketRepository, this._orderBrokerRepository, this._logger));
        }

        [Test]
        public void Ctor_LoggerNull_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new OrdersRepository(
                    this._connectionStringFactory,
                    this._marketRepository,
                    this._orderBrokerRepository,
                    null));
        }

        [Test]
        public void Ctor_MarketRepositoryNull_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new OrdersRepository(
                    this._connectionStringFactory,
                    null,
                    this._orderBrokerRepository,
                    this._logger));
        }

        [Test]
        public void Ctor_OrderBrokerRepositoryNull_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new OrdersRepository(this._connectionStringFactory, this._marketRepository, null, this._logger));
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Get()
        {
            var factory = new ConnectionStringFactory(this._configuration);

            var brokerRepository = new OrderBrokerRepository(factory, NullLogger<OrderBrokerRepository>.Instance);

            var repo = new OrdersRepository(factory, this._marketRepository, brokerRepository, this._logger);
            var row1 = this.Frame();
            var row2 = this.Frame();
            var start = row1.MostRecentDateEvent().Date;
            var end = row2.MostRecentDateEvent().AddDays(1).Date;

            await repo.Create(row1);
            await repo.Create(row2);

            var result = await repo.Get(start, end, this._opCtx);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task SetScheduledOrder_SetsOrderToAutoScheduled()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new OrdersRepository(factory, this._marketRepository, this._orderBrokerRepository, this._logger);
            var frame = this.Frame();
            var frames = new[] { frame };

            await repo.SetOrdersScheduled(frames);

            Assert.IsTrue(true);
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._logger = A.Fake<ILogger<OrdersRepository>>();
            this._opCtx = A.Fake<ISystemProcessOperationContext>();
            this._marketRepository = A.Fake<IReddeerMarketRepository>();
            this._orderBrokerRepository = A.Fake<IOrderBrokerRepository>();
            this._connectionStringFactory = A.Fake<IConnectionStringFactory>();
        }

        private Order Frame()
        {
            var exch = new Market("1", "XLON", "LSE", MarketTypes.STOCKEXCHANGE);
            var orderDates = DateTime.UtcNow;
            var tradeDates = DateTime.UtcNow;

            var securityIdentifiers = new InstrumentIdentifiers(
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
                DateTime.UtcNow,
                "trader-1",
                "trader-one",
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
                new Money(100, "GBP"),
                new Money(100, "GBP"),
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
                DateTime.UtcNow,
                "trader-2",
                "trader-two",
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
                new Money(100, "GBP"),
                new Money(100, "GBP"),
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
                DateTime.UtcNow,
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
                new Money(100, "GBP"),
                new Money(100, "GBP"),
                1000,
                1000,
                "trader-1",
                "trader one",
                "clearing-agent",
                "deal asap",
                new OrderBroker(null, string.Empty, "Mr Broker", DateTime.Now, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new[] { trade1, trade2 });

            return order2;
        }

        private Order OrderMultiple(FinancialInstrument financialInstrument, Market exch)
        {
            var orderDates = DateTime.UtcNow;

            var order = new Order(
                financialInstrument,
                exch,
                null,
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
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
                new Money(100, "GBP"),
                new Money(100, "GBP"),
                1000,
                1000,
                "trader-1",
                "trader one",
                "clearing-agent",
                "deal asap",
                null,
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);

            return order;
        }
    }
}