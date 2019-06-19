using System;
using System.Linq;
using Domain.Core.Dates;
using Domain.Core.Financial.Assets;
using Domain.Core.Markets;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SharedKernel.Contracts.Markets;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Tests.Markets
{
    [TestFixture]
    public class UniverseInterdayMarketCacheTests
    {
        private IRuleRunDataRequestRepository _dataRequestRepository;
        private ILogger _logger;

        private MarketDataRequest _mdr1;
        private MarketDataRequest _mdr2;
        private InstrumentIdentifiers _instrument1;
        private InstrumentIdentifiers _instrument2;

        private EquityInterDayTimeBarCollection _interdayTimeBarCollectionNasdaq;
        private EquityInterDayTimeBarCollection _interdayTimeBarCollectionXlon;
        private EquityInterDayTimeBarCollection _interdayTimeBarCollectionXlon2;

        [SetUp]
        public void Setup()
        {
            _dataRequestRepository = A.Fake<IRuleRunDataRequestRepository>();
            _logger = A.Fake<ILogger>();

            var xlon = new Market("1", "XLON", "XLON", MarketTypes.STOCKEXCHANGE);

            _instrument1 = new InstrumentIdentifiers(
                "1",
                "1",
                "1",
                "client-id-1",
                "abcd123",
                "abcd12345678",
                "abcd12345678",
                "abc123", 
                "TEST",
                "TEST INC",
                "TSTY");

            _instrument2 = new InstrumentIdentifiers(
                "2",
                "2",
                "2",
                "client-id-2",
                "abcd122",
                "abcd12345672",
                "abcd12345672",
                "abc122",
                "TES2",
                "TEST2 INC",
                "TST2Y");

            _mdr1 = new MarketDataRequest(
                "1",
                "XLON", 
                "entspb",
                _instrument1, 
                DateTime.UtcNow, 
                DateTime.UtcNow.AddDays(1),
                "1",
                false,
                DataSource.AllInterday);

            _mdr2 = new MarketDataRequest(
                "2",
                "XLON",
                "entspb",
                _instrument1,
                DateTime.UtcNow.AddDays(-5),
                DateTime.UtcNow.AddDays(-4),
                "1",
                false,
                DataSource.AllInterday);

            _interdayTimeBarCollectionNasdaq = new EquityInterDayTimeBarCollection(
                new Market("1", "NASDAQ", "NASDAQ", MarketTypes.STOCKEXCHANGE), 
                DateTime.UtcNow,
                new EquityInstrumentInterDayTimeBar[0]);

            _interdayTimeBarCollectionXlon = new EquityInterDayTimeBarCollection(
                xlon,
                DateTime.UtcNow,
                new EquityInstrumentInterDayTimeBar[]
                {
                    new EquityInstrumentInterDayTimeBar(
                        new FinancialInstrument(
                            InstrumentTypes.Equity,
                            _instrument1,
                            "test",
                            "entspb",
                            "GBX",
                            "TEST"),
                            new DailySummaryTimeBar(
                                null,
                                null,
                                null,
                                new Volume(1),
                                DateTime.Now),
                        DateTime.UtcNow, 
                        xlon)
                });

            _interdayTimeBarCollectionXlon2 = new EquityInterDayTimeBarCollection(
                xlon,
                DateTime.UtcNow,
                new EquityInstrumentInterDayTimeBar[]
                {
                    new EquityInstrumentInterDayTimeBar(
                        new FinancialInstrument(
                            InstrumentTypes.Equity,
                            _instrument2,
                            "test",
                            "entspb",
                            "GBX",
                            "TEST"),
                        new DailySummaryTimeBar(
                            null,
                            null,
                            null,
                            new Volume(1),
                            DateTime.Now),
                        DateTime.UtcNow,
                        xlon)
                });
        }

        [Test]
        public void Constructor_Null_Repository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseEquityInterDayCache(null, _logger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseEquityInterDayCache(_dataRequestRepository, null));
        }

        [Test]
        public void Add_Null_Does_Not_Throw_Exception()
        {
            var cache = Build();

            Assert.DoesNotThrow(() => cache.Add(null));
        }

        [Test]
        public void GetForLatestDayOnly_Null_Value_Returns_MissingData()
        {
            var cache = Build();

            var response = cache.Get(null);
            
            Assert.IsTrue(response.HadMissingData);
            A.CallTo(() => _dataRequestRepository.CreateDataRequest(A<MarketDataRequest>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void GetForLatestDayOnly_Value_Not_Added_Returns_MissingData_And_MarketDataRequest_Made()
        {
            var cache = Build();

            var response = cache.Get(_mdr1);

            Assert.IsTrue(response.HadMissingData);
            A.CallTo(() => _dataRequestRepository.CreateDataRequest(A<MarketDataRequest>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void GetForLatestDayOnly_Value_Added_ButForDifferentMarket_Returns_MissingData_And_MarketDataRequest_Made()
        {
            var cache = Build();

            cache.Add(_interdayTimeBarCollectionNasdaq);
            var response = cache.Get(_mdr1);

            Assert.IsTrue(response.HadMissingData);
            A.CallTo(() => _dataRequestRepository.CreateDataRequest(A<MarketDataRequest>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void GetForLatestDayOnly_Value_Added_ForSameMarket_Returns_Data_And_NoMarketDataRequestMade()
        {
            var cache = Build();

            cache.Add(_interdayTimeBarCollectionXlon);
            var response = cache.Get(_mdr1);

            A.CallTo(() => _dataRequestRepository.CreateDataRequest(A<MarketDataRequest>.Ignored))
                .MustNotHaveHappened();
            Assert.IsFalse(response.HadMissingData);
            Assert.AreEqual(response.Response, _interdayTimeBarCollectionXlon.Securities.FirstOrDefault());
        }

        [Test]
        public void GetForLatestDayOnly_Value_Added_ForSameMarket_Returns_MissingData_And_MarketDataRequestMade_When_Out_Of_Date_Range()
        {
            var cache = Build();

            cache.Add(_interdayTimeBarCollectionXlon);



            var response = cache.Get(_mdr1);

            A.CallTo(() => _dataRequestRepository.CreateDataRequest(A<MarketDataRequest>.Ignored))
                .MustNotHaveHappened();
            Assert.IsFalse(response.HadMissingData);
            Assert.AreEqual(response.Response, _interdayTimeBarCollectionXlon.Securities.FirstOrDefault());
        }

        [Test]
        public void GetForLatestDayOnly_Value_Added_ForSameMarket_But_DifferentSecurity_Returns_MissingData_And_MarketDataRequestMade()
        {
            var cache = Build();

            cache.Add(_interdayTimeBarCollectionXlon2);
            var response = cache.Get(_mdr2);

            A.CallTo(() => _dataRequestRepository.CreateDataRequest(A<MarketDataRequest>.Ignored))
                .MustHaveHappened();
            Assert.IsTrue(response.HadMissingData);
            Assert.AreNotEqual(response.Response, _interdayTimeBarCollectionXlon2.Securities.FirstOrDefault());
        }

        [Test]
        public void GetMarketsForRange_NullDatesCollection_ReturnsMissingData()
        {
            var cache = Build();

            cache.Add(_interdayTimeBarCollectionXlon);
            var response = cache.GetMarketsForRange(_mdr1, null, RuleRunMode.ForceRun);

            Assert.IsTrue(response.HadMissingData);
        }

        [Test]
        public void GetMarketsForRange_DatesCollection_No_Request_ReturnsMissingData()
        {
            var cache = Build();

            cache.Add(_interdayTimeBarCollectionXlon);
            var response = cache.GetMarketsForRange(
                null,
                new []
                {
                    new DateRange(
                        DateTime.UtcNow, 
                        DateTime.UtcNow.AddDays(1)),
                }, RuleRunMode.ForceRun);

            Assert.IsTrue(response.HadMissingData);
        }

        [Test]
        public void GetMarketsForRange_DatesCollection_Request_ReturnsMissingData()
        {
            var cache = Build();

            cache.Add(_interdayTimeBarCollectionXlon);
            var response = cache.GetMarketsForRange(
                _mdr1,
                new[]
                {
                    new DateRange(
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddDays(1)),
                }, RuleRunMode.ForceRun);

            Assert.IsTrue(response.HadMissingData);
        }




        private UniverseEquityInterDayCache Build()
        {
            return new UniverseEquityInterDayCache(_dataRequestRepository, _logger);
        }
    }
}
