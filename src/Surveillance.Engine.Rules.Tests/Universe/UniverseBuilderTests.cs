using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Scheduling;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.Engine.Rules.Tests.Helpers;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;
using Surveillance.Engine.Rules.Universe.MarketEvents.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Universe
{
    [TestFixture]
    public class UniverseBuilderTests
    {
        private IOrdersRepository _auroraOrdersRepository;
        private IOrdersToAllocatedOrdersProjector _orderAllocationProjector;
        private IReddeerMarketRepository _auroraMarketRepository;
        private IMarketOpenCloseEventManager _marketManager;
        private ISystemProcessOperationContext _opCtx;
        private IUniverseSortComparer _sortComparer;
        private ILogger<UniverseBuilder> _logger;

        [SetUp]
        public void Setup()
        {
            _auroraOrdersRepository = A.Fake<IOrdersRepository>();
            _orderAllocationProjector = A.Fake<IOrdersToAllocatedOrdersProjector>();
            _auroraMarketRepository = A.Fake<IReddeerMarketRepository>();
            _marketManager = A.Fake<IMarketOpenCloseEventManager>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _sortComparer = A.Fake<IUniverseSortComparer>();
            _logger = A.Fake<ILogger<UniverseBuilder>>();
        }

        [Test]
        public void Summon_DoesNot_ReturnNull()
        {
            var builder =
                new UniverseBuilder(
                    _auroraOrdersRepository,
                    _orderAllocationProjector,
                    _auroraMarketRepository,
                    _marketManager,
                    _sortComparer,
                    _logger);

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
                    _auroraOrdersRepository,
                    _orderAllocationProjector,
                    _auroraMarketRepository,
                    _marketManager,
                    _sortComparer,
                    _logger);

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
        [Ignore("Might be cause of the build server issues")]
        public async Task Summon_FetchesTradeOrderData()
        { 
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder =
                new UniverseBuilder(
                    _auroraOrdersRepository,
                    _orderAllocationProjector,
                    _auroraMarketRepository,
                    _marketManager,
                    _sortComparer,
                    _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier>(),
                TimeSeriesInitiation = timeSeriesInitiation,
                TimeSeriesTermination = timeSeriesTermination
            };
            var frame = ((Order)null).Random();

            A.CallTo(() => _auroraOrdersRepository.Get(timeSeriesInitiation, timeSeriesTermination, _opCtx))
                .Returns(new[] {frame});
            
            A.CallTo(() => _orderAllocationProjector.DecorateOrders(A<IReadOnlyCollection<Order>>.Ignored))
                .Returns(new[] {frame});

            var result = await builder.Summon(schedule, _opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Trades.Count, 1);
            Assert.AreEqual(result.Trades.FirstOrDefault(), frame);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(result.UniverseEvents.Skip(1).FirstOrDefault().UnderlyingEvent, frame);
            Assert.AreEqual(result.UniverseEvents.Skip(2).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);

            A.CallTo(() => _auroraOrdersRepository.Get(timeSeriesInitiation, timeSeriesTermination, _opCtx)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Summon_FetchesMarketOpenCloseData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder =
                new UniverseBuilder(
                    _auroraOrdersRepository,
                    _orderAllocationProjector,
                    _auroraMarketRepository,
                    _marketManager,
                    _sortComparer,
                    _logger);

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

            A.CallTo(() => _marketManager.AllOpenCloseEvents(timeSeriesInitiation, timeSeriesTermination))
                .Returns(marketOpenClose);

            var result = await builder.Summon(schedule, _opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(result.UniverseEvents.Skip(1).FirstOrDefault().StateChange, UniverseStateEvent.ExchangeOpen);
            Assert.AreEqual(result.UniverseEvents.Skip(2).FirstOrDefault().StateChange, UniverseStateEvent.ExchangeClose);
            Assert.AreEqual(result.UniverseEvents.Skip(3).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);
        }

        [Test]
        public async Task Summon_FetchesStockExchangeTickUpdateData()
        {
            var timeSeriesInitiation = new DateTime(2018, 01, 01);
            var timeSeriesTermination = new DateTime(2018, 01, 02);
            var builder =
                new UniverseBuilder(
                    _auroraOrdersRepository,
                    _orderAllocationProjector,
                    _auroraMarketRepository,
                    _marketManager,
                    _sortComparer,
                    _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier>(),
                TimeSeriesInitiation = timeSeriesInitiation,
                TimeSeriesTermination = timeSeriesTermination
            };

            var exchangeFrames = new[]
            {
                new EquityIntraDayTimeBarCollection(
                    new Market("1", "xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                    timeSeriesInitiation,
                    new List<EquityInstrumentIntraDayTimeBar>()),
                new EquityIntraDayTimeBarCollection(
                    new Market(
                        "1","xlon", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                    timeSeriesTermination,
                    new List<EquityInstrumentIntraDayTimeBar>())
            };

            A.CallTo(() => _auroraMarketRepository.GetEquityIntraday(A<DateTime>.Ignored, A<DateTime>.Ignored, _opCtx))
                .Returns(exchangeFrames);

            var result = await builder.Summon(schedule, _opCtx);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.UniverseEvents.FirstOrDefault().StateChange, UniverseStateEvent.Genesis);
            Assert.AreEqual(result.UniverseEvents.Skip(1).FirstOrDefault().StateChange, UniverseStateEvent.EquityIntradayTick);
            Assert.AreEqual(result.UniverseEvents.Skip(2).FirstOrDefault().StateChange, UniverseStateEvent.EquityIntradayTick);
            Assert.AreEqual(result.UniverseEvents.Skip(3).FirstOrDefault().StateChange, UniverseStateEvent.Eschaton);
        }
    }
}
