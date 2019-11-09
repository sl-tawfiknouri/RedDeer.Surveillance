namespace Surveillance.Data.Universe.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Data.Universe.MarketEvents.Interfaces;
    using Surveillance.Data.Universe.Trades.Interfaces;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    using TestHelpers;

    [TestFixture]
    public class UniverseBuilderTests
    {
        private IReddeerMarketRepository _auroraMarketRepository;

        private IOrdersRepository _auroraOrdersRepository;

        private ILogger<UniverseBuilder> _logger;

        private IMarketOpenCloseEventService _marketService;

        private ISystemProcessOperationContext _opCtx;

        private IOrdersToAllocatedOrdersProjector _orderAllocationProjector;

        private IUniverseSortComparer _sortComparer;

        [SetUp]
        public void Setup()
        {
            this._auroraOrdersRepository = A.Fake<IOrdersRepository>();
            this._orderAllocationProjector = A.Fake<IOrdersToAllocatedOrdersProjector>();
            this._auroraMarketRepository = A.Fake<IReddeerMarketRepository>();
            this._marketService = A.Fake<IMarketOpenCloseEventService>();
            this._opCtx = A.Fake<ISystemProcessOperationContext>();
            this._sortComparer = A.Fake<IUniverseSortComparer>();
            this._logger = A.Fake<ILogger<UniverseBuilder>>();
        }

        [Test]
        public void Summon_DoesNot_ReturnNull()
        {
            var builder = new UniverseBuilder(
                this._auroraOrdersRepository,
                this._orderAllocationProjector,
                this._auroraMarketRepository,
                this._marketService,
                this._sortComparer,
                this._logger);

            var result = builder.Summon(null, this._opCtx);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Summon_FetchesMarketOpenCloseData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder = new UniverseBuilder(
                this._auroraOrdersRepository,
                this._orderAllocationProjector,
                this._auroraMarketRepository,
                this._marketService,
                this._sortComparer,
                this._logger);

            var schedule = new ScheduledExecution
                               {
                                   Rules = new List<RuleIdentifier>(),
                                   TimeSeriesInitiation = timeSeriesInitiation,
                                   TimeSeriesTermination = timeSeriesTermination
                               };

            var marketOpenClose = new[]
                                      {
                                          new UniverseEvent(
                                              UniverseStateEvent.ExchangeOpen,
                                              timeSeriesInitiation,
                                              new MarketOpenClose("xlon", timeSeriesInitiation, timeSeriesInitiation)),
                                          new UniverseEvent(
                                              UniverseStateEvent.ExchangeClose,
                                              timeSeriesTermination,
                                              new MarketOpenClose("xlon", timeSeriesTermination, timeSeriesTermination))
                                      };

            A.CallTo(() => this._marketService.AllOpenCloseEvents(timeSeriesInitiation, timeSeriesTermination))
                .Returns(marketOpenClose);

            var result = await builder.Summon(schedule, this._opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(
                result.UniverseEvents.Skip(1).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochPrimordialUniverse);
            Assert.AreEqual(
                result.UniverseEvents.Skip(2).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochRealUniverse);
            Assert.AreEqual(
                result.UniverseEvents.Skip(3).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochFutureUniverse);
            Assert.AreEqual(
                result.UniverseEvents.Skip(4).FirstOrDefault().StateChange,
                UniverseStateEvent.ExchangeOpen);
            Assert.AreEqual(
                result.UniverseEvents.Skip(5).FirstOrDefault().StateChange,
                UniverseStateEvent.ExchangeClose);
            Assert.AreEqual(result.UniverseEvents.Skip(6).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);
        }

        [Test]
        public async Task Summon_FetchesStockExchangeTickUpdateData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder = new UniverseBuilder(
                this._auroraOrdersRepository,
                this._orderAllocationProjector,
                this._auroraMarketRepository,
                this._marketService,
                this._sortComparer,
                this._logger);

            var schedule = new ScheduledExecution
                               {
                                   Rules = new List<RuleIdentifier>(),
                                   TimeSeriesInitiation = timeSeriesInitiation,
                                   TimeSeriesTermination = timeSeriesTermination
                               };

            var exchangeFrames = new[]
                                     {
                                         new EquityIntraDayTimeBarCollection(
                                             new Market(
                                                 "1",
                                                 "xlon",
                                                 "London Stock Exchange",
                                                 MarketTypes.STOCKEXCHANGE),
                                             timeSeriesInitiation,
                                             new List<EquityInstrumentIntraDayTimeBar>()),
                                         new EquityIntraDayTimeBarCollection(
                                             new Market(
                                                 "1",
                                                 "xlon",
                                                 "London Stock Exchange",
                                                 MarketTypes.STOCKEXCHANGE),
                                             timeSeriesTermination,
                                             new List<EquityInstrumentIntraDayTimeBar>())
                                     };

            A.CallTo(
                () => this._auroraMarketRepository.GetEquityIntraday(
                    A<DateTime>.Ignored,
                    A<DateTime>.Ignored,
                    this._opCtx)).Returns(exchangeFrames);

            var result = await builder.Summon(schedule, this._opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(
                result.UniverseEvents.Skip(1).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochPrimordialUniverse);
            Assert.AreEqual(
                result.UniverseEvents.Skip(2).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochRealUniverse);
            Assert.AreEqual(
                result.UniverseEvents.Skip(3).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochFutureUniverse);
            Assert.AreEqual(
                result.UniverseEvents.Skip(4).FirstOrDefault().StateChange,
                UniverseStateEvent.EquityIntraDayTick);
            Assert.AreEqual(
                result.UniverseEvents.Skip(5).FirstOrDefault().StateChange,
                UniverseStateEvent.EquityIntraDayTick);
            Assert.AreEqual(result.UniverseEvents.Skip(6).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);
        }

        [Test]
        [Ignore("Might be cause of the build server issues")]
        public async Task Summon_FetchesTradeOrderData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder = new UniverseBuilder(
                this._auroraOrdersRepository,
                this._orderAllocationProjector,
                this._auroraMarketRepository,
                this._marketService,
                this._sortComparer,
                this._logger);

            var schedule = new ScheduledExecution
                               {
                                   Rules = new List<RuleIdentifier>(),
                                   TimeSeriesInitiation = timeSeriesInitiation,
                                   TimeSeriesTermination = timeSeriesTermination
                               };
            var frame = ((Order)null).Random();

            A.CallTo(() => this._auroraOrdersRepository.Get(timeSeriesInitiation, timeSeriesTermination, this._opCtx))
                .Returns(new[] { frame });

            A.CallTo(() => this._orderAllocationProjector.DecorateOrders(A<IReadOnlyCollection<Order>>.Ignored))
                .Returns(new[] { frame });

            var result = await builder.Summon(schedule, this._opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(result.UniverseEvents.Skip(1).FirstOrDefault().UnderlyingEvent, frame);
            Assert.AreEqual(result.UniverseEvents.Skip(2).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);

            A.CallTo(() => this._auroraOrdersRepository.Get(timeSeriesInitiation, timeSeriesTermination, this._opCtx))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Summon_InsertsUniverseBeginningAndEndEventData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder = new UniverseBuilder(
                this._auroraOrdersRepository,
                this._orderAllocationProjector,
                this._auroraMarketRepository,
                this._marketService,
                this._sortComparer,
                this._logger);

            var schedule = new ScheduledExecution
                               {
                                   Rules = new List<RuleIdentifier>(),
                                   TimeSeriesInitiation = timeSeriesInitiation,
                                   TimeSeriesTermination = timeSeriesTermination
                               };

            var result = await builder.Summon(schedule, this._opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(
                result.UniverseEvents.Skip(1).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochPrimordialUniverse);
            Assert.AreEqual(
                result.UniverseEvents.Skip(2).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochRealUniverse);
            Assert.AreEqual(
                result.UniverseEvents.Skip(3).FirstOrDefault().StateChange,
                UniverseStateEvent.EpochFutureUniverse);
            Assert.AreEqual(result.UniverseEvents.Skip(4).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);
        }
    }
}