using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.DataLayer.Api.Interfaces;
using Surveillance.DataLayer.Stub;
using Surveillance.Universe;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Tests.Universe.MarketEvents
{
    [TestFixture]
    public class MarketOpenCloseEventManagerTests
    {
        private IMarketOpenCloseApiCachingDecoratorRepository _repository;
        private ExchangeDto _marketOpenClose;

        [SetUp]
        public void Setup()
        {
            _repository = A.Fake<IMarketOpenCloseApiCachingDecoratorRepository>();
            _marketOpenClose =
                new ExchangeDto
                {
                    Code = "XLON",
                    MarketOpenTime = TimeSpan.FromHours(8),
                    MarketCloseTime = TimeSpan.FromHours(16),
                    TimeZone = "Central Standard Time",
                    IsOpenOnMonday = true,
                    IsOpenOnTuesday = true,
                    IsOpenOnWednesday = true,
                    IsOpenOnThursday = true,
                    IsOpenOnFriday = true,
                    IsOpenOnSaturday = true,
                    IsOpenOnSunday = true,
                };

            IReadOnlyCollection<ExchangeDto> testCollection = new List<ExchangeDto>
            {
                _marketOpenClose
            };

            A.CallTo(() => _repository.Get()).Returns(Task.FromResult(testCollection));
        }

        [Test]
        public void Constructor_NullRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseEventManager(null));
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanTooShortForMarketBeforeOpen_ReturnEmpty()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 3, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 4, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);

            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanTooShortForMarketAfterClose_ReturnEmpty()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 19, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 20, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);

            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanOpenOnlyMarket_ReturnOnlyOneOpen()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 13, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 15, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();

            Assert.AreEqual(results.Count, 1);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 14, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketOpen);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanCloseOnlyMarket_ReturnOnlyOneClose()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 16, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 23, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();

            Assert.AreEqual(results.Count, 1);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 22, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketClose);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanOpenCloseOneDayOnlyMarket_ReturnOnlyOneOpenAndClose()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 7, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 23, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();
            var secondResult = results.Skip(1).FirstOrDefault();

            Assert.AreEqual(results.Count, 2);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 14, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketOpen);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 1, 22, 0, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.StockMarketClose);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanOpenCloseOpenOnlyMarket_ReturnTwoOpenAndOneClose()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 10, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 2, 20, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();
            var secondResult = results.Skip(1).FirstOrDefault();
            var thirdResult = results.Skip(2).FirstOrDefault();

            Assert.AreEqual(results.Count, 3);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 14, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketOpen);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 1, 22, 0, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.StockMarketClose);

            Assert.IsNotNull(thirdResult);
            Assert.AreEqual(thirdResult.EventTime, new DateTime(2000, 1, 2, 14, 0, 0));
            Assert.AreEqual(thirdResult.StateChange, UniverseStateEvent.StockMarketOpen);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanCloseOpenCloseOnlyMarket_ReturnOneOpenAndTwoClose()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 15, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 2, 23, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();
            var secondResult = results.Skip(1).FirstOrDefault();
            var thirdResult = results.Skip(2).FirstOrDefault();

            Assert.AreEqual(results.Count, 3);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 22, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketClose);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 2, 14, 0, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.StockMarketOpen);

            Assert.IsNotNull(thirdResult);
            Assert.AreEqual(thirdResult.EventTime, new DateTime(2000, 1, 2, 22, 0, 0));
            Assert.AreEqual(thirdResult.StateChange, UniverseStateEvent.StockMarketClose);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanFiveDays_ReturnsTenEvents()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var start = new DateTime(2000, 1, 1, 5, 0, 0);
            var end = new DateTime(2000, 1, 5, 23, 0, 0);

            var results = await manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 10);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanFiveDaysAfterOpen_ReturnsNineEvents()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var start = new DateTime(2000, 1, 1, 12, 0, 0);
            var end = new DateTime(2000, 1, 5, 20, 0, 0);

            var results = await manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 9);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanFiveDaysBeforeFinalClose_ReturnsNineEvents()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var start = new DateTime(2000, 1, 1, 13, 0, 0);
            var end = new DateTime(2000, 1, 5, 21, 0, 0);

            var results = await manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 9);
        }
    }
}