using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.Stub;
using Surveillance.DataLayer.Stub.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Tests.Universe.MarketEvents
{
    [TestFixture]
    public class MarketOpenCloseEventManagerTests
    {
        private IMarketOpenCloseRepository _repository;
        private MarketOpenClose _marketOpenClose;

        [SetUp]
        public void Setup()
        {
            _repository = A.Fake<IMarketOpenCloseRepository>();
            _marketOpenClose =
                new MarketOpenClose(
                    "XLON",
                    new DateTime(2000, 1, 1, 9, 0, 0),
                    new DateTime(2000, 1, 1, 17, 0, 0));

            var testCollection = new List<MarketOpenClose>
            {
                _marketOpenClose
            };

            A.CallTo(() => _repository.GetAll()).Returns(testCollection);
        }

        [Test]
        public void Constructor_NullRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseEventManager(null));
        }

        [Test]
        public void AllOpenCloseEvents_TimespanTooShortForMarketBeforeOpen_ReturnEmpty()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 3, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 4, 0, 0);

            var results = manager.AllOpenCloseEvents(earlyStart, earlyEnd);

            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanTooShortForMarketAfterClose_ReturnEmpty()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 19, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 20, 0, 0);

            var results = manager.AllOpenCloseEvents(earlyStart, earlyEnd);

            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanOpenOnlyMarket_ReturnOnlyOneOpen()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 8, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 10, 0, 0);

            var results = manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();

            Assert.AreEqual(results.Count, 1);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 9, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketOpen);
            Assert.AreEqual(firstResult.UnderlyingEvent, _marketOpenClose);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanCloseOnlyMarket_ReturnOnlyOneClose()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 10, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 20, 0, 0);

            var results = manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();

            Assert.AreEqual(results.Count, 1);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 17, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketClose);
            Assert.AreEqual(firstResult.UnderlyingEvent, _marketOpenClose);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanOpenCloseOneDayOnlyMarket_ReturnOnlyOneOpenAndClose()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 7, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 20, 0, 0);

            var results = manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();
            var secondResult = results.Skip(1).FirstOrDefault();

            Assert.AreEqual(results.Count, 2);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 9, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketOpen);
            Assert.AreEqual(firstResult.UnderlyingEvent, _marketOpenClose);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 1, 17, 0, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.StockMarketClose);
            Assert.AreEqual(secondResult.UnderlyingEvent, _marketOpenClose);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanOpenCloseOpenOnlyMarket_ReturnTwoOpenAndOneClose()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 7, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 2, 12, 0, 0);
            var closeMarket = new MarketOpenClose(_marketOpenClose.MarketId, _marketOpenClose.MarketOpen.AddDays(1), _marketOpenClose.MarketClose.AddDays(1));

            var results = manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();
            var secondResult = results.Skip(1).FirstOrDefault();
            var thirdResult = results.Skip(2).FirstOrDefault();

            Assert.AreEqual(results.Count, 3);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 9, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketOpen);
            Assert.AreEqual(firstResult.UnderlyingEvent, _marketOpenClose);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 1, 17, 0, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.StockMarketClose);
            Assert.AreEqual(secondResult.UnderlyingEvent, _marketOpenClose);

            Assert.IsNotNull(thirdResult);
            Assert.AreEqual(thirdResult.EventTime, new DateTime(2000, 1, 2, 9, 0, 0));
            Assert.AreEqual(thirdResult.StateChange, UniverseStateEvent.StockMarketOpen);
            Assert.AreEqual(thirdResult.UnderlyingEvent, closeMarket);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanCloseOpenCloseOnlyMarket_ReturnOneOpenAndTwoClose()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var earlyStart = new DateTime(2000, 1, 1, 12, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 2, 20, 0, 0);
            var closeMarket = new MarketOpenClose(_marketOpenClose.MarketId, _marketOpenClose.MarketOpen.AddDays(1), _marketOpenClose.MarketClose.AddDays(1));

            var results = manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.FirstOrDefault();
            var secondResult = results.Skip(1).FirstOrDefault();
            var thirdResult = results.Skip(2).FirstOrDefault();

            Assert.AreEqual(results.Count, 3);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 17, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.StockMarketClose);
            Assert.AreEqual(firstResult.UnderlyingEvent, _marketOpenClose);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 2, 9, 0, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.StockMarketOpen);
            Assert.AreEqual(secondResult.UnderlyingEvent, closeMarket);

            Assert.IsNotNull(thirdResult);
            Assert.AreEqual(thirdResult.EventTime, new DateTime(2000, 1, 2, 17, 0, 0));
            Assert.AreEqual(thirdResult.StateChange, UniverseStateEvent.StockMarketClose);
            Assert.AreEqual(thirdResult.UnderlyingEvent, closeMarket);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanFiveDays_ReturnsTenEvents()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var start = new DateTime(2000, 1, 1, 5, 0, 0);
            var end = new DateTime(2000, 1, 5, 20, 0, 0);

            var results = manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 10);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanFiveDaysAfterOpen_ReturnsNineEvents()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var start = new DateTime(2000, 1, 1, 12, 0, 0);
            var end = new DateTime(2000, 1, 5, 20, 0, 0);

            var results = manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 9);
        }

        [Test]
        public void AllOpenCloseEvents_TimespanFiveDaysBeforeFinalClose_ReturnsNineEvents()
        {
            var manager = new MarketOpenCloseEventManager(_repository);
            var start = new DateTime(2000, 1, 1, 5, 0, 0);
            var end = new DateTime(2000, 1, 5, 12, 0, 0);

            var results = manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 9);
        }
    }
}