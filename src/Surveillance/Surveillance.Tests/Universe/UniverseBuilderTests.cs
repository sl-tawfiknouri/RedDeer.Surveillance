using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Equity.Frames;
using Domain.Market;
using Domain.Scheduling;
using Domain.Trades.Orders;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.MarketEvents.Interfaces;
using Surveillance.Tests.Helpers;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Tests.Universe
{
    [TestFixture]
    public class UniverseBuilderTests
    {
        private IReddeerTradeRepository _auroraTradeRepository;
        private IReddeerMarketRepository _auroraMarketRepository;
        private IMarketOpenCloseEventManager _marketManager;
        private ISystemProcessOperationContext _opCtx;

        [SetUp]
        public void Setup()
        {
            _auroraTradeRepository = A.Fake<IReddeerTradeRepository>();
            _auroraMarketRepository = A.Fake<IReddeerMarketRepository>();
            _marketManager = A.Fake<IMarketOpenCloseEventManager>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
        }

        [Test]
        public void Summon_DoesNot_ReturnNull()
        {
            var builder =
                new UniverseBuilder(
                    _auroraTradeRepository,
                    _auroraMarketRepository,
                    _marketManager);

            var result = builder.Summon(null, _opCtx);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Summon_InsertsUniverseBeginningAndEndEventData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder =
                new UniverseBuilder(
                    _auroraTradeRepository,
                    _auroraMarketRepository,
                    _marketManager);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier>(),
                TimeSeriesInitiation = timeSeriesInitiation,
                TimeSeriesTermination = timeSeriesTermination
            };

            var result = await builder.Summon(schedule, _opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(result.UniverseEvents.Skip(1).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);
        }

        [Test]
        public async Task Summon_FetchesTradeOrderData()
        { 
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder =
                new UniverseBuilder(
                    _auroraTradeRepository,
                    _auroraMarketRepository,
                    _marketManager);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier>(),
                TimeSeriesInitiation = timeSeriesInitiation,
                TimeSeriesTermination = timeSeriesTermination
            };
            var frame = ((TradeOrderFrame)null).Random();

            A
                .CallTo(() => _auroraTradeRepository.Get(timeSeriesInitiation, timeSeriesTermination, _opCtx))
                .Returns(new[] {frame});

            var result = await builder.Summon(schedule, _opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Trades.Count, 1);
            Assert.AreEqual(result.Trades.FirstOrDefault(), frame);
            Assert.AreEqual(result.UniverseEvents.Skip(1).FirstOrDefault().UnderlyingEvent, frame);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(result.UniverseEvents.Skip(3).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);

            A.CallTo(() => _auroraTradeRepository.Get(timeSeriesInitiation, timeSeriesTermination, _opCtx)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Summon_FetchesMarketOpenCloseData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder =
                new UniverseBuilder(
                    _auroraTradeRepository,
                    _auroraMarketRepository,
                    _marketManager);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier>(),
                TimeSeriesInitiation = timeSeriesInitiation,
                TimeSeriesTermination = timeSeriesTermination
            };

            var marketOpenClose = new[]
            {
                new UniverseEvent(
                    UniverseStateEvent.StockMarketOpen,
                    timeSeriesInitiation,
                    new MarketOpenClose("xlon", timeSeriesInitiation, timeSeriesInitiation)),
                new UniverseEvent(
                    UniverseStateEvent.StockMarketClose,
                    timeSeriesTermination,
                    new MarketOpenClose("xlon", timeSeriesTermination, timeSeriesTermination))
            };

            A
                .CallTo(() => _marketManager.AllOpenCloseEvents(timeSeriesInitiation, timeSeriesTermination))
                .Returns(marketOpenClose);

            var result = await builder.Summon(schedule, _opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(result.UniverseEvents.Skip(1).FirstOrDefault().StateChange, UniverseStateEvent.StockMarketOpen);
            Assert.AreEqual(result.UniverseEvents.Skip(2).FirstOrDefault().StateChange, UniverseStateEvent.StockMarketClose);
            Assert.AreEqual(result.UniverseEvents.Skip(3).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);
        }

        [Test]
        public async Task Summon_FetchesStockExchangeTickUpdateData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder =
                new UniverseBuilder(
                    _auroraTradeRepository,
                    _auroraMarketRepository,
                    _marketManager);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier>(),
                TimeSeriesInitiation = timeSeriesInitiation,
                TimeSeriesTermination = timeSeriesTermination
            };

            var exchangeFrames = new[]
            {
                new ExchangeFrame(
                    new StockExchange(
                        new Market.MarketId("xlon"), "London Stock Exchange"),
                    timeSeriesInitiation,
                    new List<SecurityTick>()),
                new ExchangeFrame(
                    new StockExchange(
                        new Market.MarketId("xlon"), "London Stock Exchange"),
                    timeSeriesTermination,
                    new List<SecurityTick>())
            };

            A
                .CallTo(() => _auroraMarketRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored, _opCtx))
                .Returns(exchangeFrames);

            var result = await builder.Summon(schedule, _opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(result.UniverseEvents.Skip(1).FirstOrDefault().StateChange, UniverseStateEvent.StockTickReddeer);
            Assert.AreEqual(result.UniverseEvents.Skip(2).FirstOrDefault().StateChange, UniverseStateEvent.StockTickReddeer);
            Assert.AreEqual(result.UniverseEvents.Skip(3).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);
        }
    }
}
