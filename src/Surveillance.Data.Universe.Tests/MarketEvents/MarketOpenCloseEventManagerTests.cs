namespace Surveillance.Data.Universe.Tests.MarketEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    [TestFixture]
    public class MarketOpenCloseEventManagerTests
    {
        private ILogger<MarketOpenCloseEventService> _logger;

        private ExchangeDto _marketOpenClose;

        private IMarketOpenCloseApiCachingDecorator _repository;

        [Test]
        public async Task AllOpenCloseEvents_TimespanCloseOnlyMarket_ReturnFiveEventsClose()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var earlyStart = new DateTime(2000, 1, 1, 16, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 23, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var closeResult = results.Skip(2).FirstOrDefault();

            Assert.AreEqual(results.Count, 5);
            Assert.IsNotNull(closeResult);
            Assert.AreEqual(closeResult.EventTime, new DateTime(2000, 1, 1, 16, 30, 0));
            Assert.AreEqual(closeResult.StateChange, UniverseStateEvent.ExchangeClose);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanCloseOpenCloseOnlyMarket_ReturnOneOpenAndTwoClose()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var earlyStart = new DateTime(2000, 1, 1, 15, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 2, 23, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.Skip(2).FirstOrDefault();
            var secondResult = results.Skip(3).FirstOrDefault();
            var thirdResult = results.Skip(4).FirstOrDefault();

            Assert.AreEqual(results.Count, 7);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 16, 30, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.ExchangeClose);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 2, 8, 0, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.ExchangeOpen);

            Assert.IsNotNull(thirdResult);
            Assert.AreEqual(thirdResult.EventTime, new DateTime(2000, 1, 2, 16, 30, 0));
            Assert.AreEqual(thirdResult.StateChange, UniverseStateEvent.ExchangeClose);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanFiveDays_ReturnsTenEvents()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var start = new DateTime(2000, 1, 1, 5, 0, 0);
            var end = new DateTime(2000, 1, 5, 23, 0, 0);

            var results = await manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 14);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanFiveDaysAfterOpen_ReturnsNineEvents()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var start = new DateTime(2000, 1, 1, 12, 0, 0);
            var end = new DateTime(2000, 1, 5, 20, 0, 0);

            var results = await manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 13);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanFiveDaysBeforeFinalClose_ReturnsNineEvents()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var start = new DateTime(2000, 1, 1, 13, 0, 0);
            var end = new DateTime(2000, 1, 5, 21, 0, 0);

            var results = await manager.AllOpenCloseEvents(start, end);

            Assert.AreEqual(results.Count, 13);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanOpenCloseOneDayOnlyMarket_ReturnOnlyOneOpenAndClose()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var earlyStart = new DateTime(2000, 1, 1, 7, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 23, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.Skip(2).FirstOrDefault();
            var secondResult = results.Skip(3).FirstOrDefault();

            Assert.AreEqual(results.Count, 6);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 8, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.ExchangeOpen);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 1, 16, 30, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.ExchangeClose);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanOpenCloseOpenOnlyMarket_ReturnTwoOpenAndOneClose()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var earlyStart = new DateTime(2000, 1, 1, 10, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 2, 20, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.Skip(1).FirstOrDefault();
            var secondResult = results.Skip(2).FirstOrDefault();
            var thirdResult = results.Skip(3).FirstOrDefault();

            Assert.AreEqual(results.Count, 7);

            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 8, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.ExchangeOpen);

            Assert.IsNotNull(secondResult);
            Assert.AreEqual(secondResult.EventTime, new DateTime(2000, 1, 1, 16, 30, 0));
            Assert.AreEqual(secondResult.StateChange, UniverseStateEvent.ExchangeClose);

            Assert.IsNotNull(thirdResult);
            Assert.AreEqual(thirdResult.EventTime, new DateTime(2000, 1, 2, 8, 0, 0));
            Assert.AreEqual(thirdResult.StateChange, UniverseStateEvent.ExchangeOpen);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanOpenOnlyMarket_ReturnOnlyOneOpen()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var earlyStart = new DateTime(2000, 1, 1, 13, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 15, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);
            var firstResult = results.Skip(1).FirstOrDefault();

            Assert.AreEqual(results.Count, 4);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(firstResult.EventTime, new DateTime(2000, 1, 1, 8, 0, 0));
            Assert.AreEqual(firstResult.StateChange, UniverseStateEvent.ExchangeOpen);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanTooShortForMarketAfterClose_ReturnEmpty()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var earlyStart = new DateTime(2000, 1, 1, 19, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 20, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);

            Assert.AreEqual(results.Count, 4);
        }

        [Test]
        public async Task AllOpenCloseEvents_TimespanTooShortForMarketBeforeOpen_ReturnEmpty()
        {
            var manager = new MarketOpenCloseEventService(this._repository, this._logger);
            var earlyStart = new DateTime(2000, 1, 1, 3, 0, 0);
            var earlyEnd = new DateTime(2000, 1, 1, 4, 0, 0);

            var results = await manager.AllOpenCloseEvents(earlyStart, earlyEnd);

            Assert.AreEqual(results.Count, 4);
        }

        [Test]
        public void Constructor_NullRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseEventService(null, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger<MarketOpenCloseEventService>>();
            this._repository = A.Fake<IMarketOpenCloseApiCachingDecorator>();
            this._marketOpenClose = new ExchangeDto
                                        {
                                            Code = "XLON",
                                            MarketOpenTime = TimeSpan.FromHours(8),
                                            MarketCloseTime = TimeSpan.FromHours(16).Add(TimeSpan.FromMinutes(30)),
                                            TimeZone = "UTC",
                                            IsOpenOnMonday = true,
                                            IsOpenOnTuesday = true,
                                            IsOpenOnWednesday = true,
                                            IsOpenOnThursday = true,
                                            IsOpenOnFriday = true,
                                            IsOpenOnSaturday = true,
                                            IsOpenOnSunday = true
                                        };

            IReadOnlyCollection<ExchangeDto> testCollection = new List<ExchangeDto> { this._marketOpenClose };

            A.CallTo(() => this._repository.GetAsync()).Returns(Task.FromResult(testCollection));
        }
    }
}