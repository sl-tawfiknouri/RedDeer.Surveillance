using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.Markets;

namespace Surveillance.Tests.Markets
{
    [TestFixture]
    public class MarketTradingHoursManagerTests
    {
        private IMarketOpenCloseApiCachingDecoratorRepository _marketOpenCloseRepository;
        private ILogger<MarketTradingHoursManager> _logger;

        [SetUp]
        public void Setup()
        {
            _marketOpenCloseRepository = A.Fake<IMarketOpenCloseApiCachingDecoratorRepository>();
            _logger = new NullLogger<MarketTradingHoursManager>();
        }

        [Test]
        public void GetTradingDaysWithinRangeAdjustedToTime_Empty_Mic_Returns_Empty_List()
        {
            var marketTradingHoursManager = Build();

            var result = marketTradingHoursManager.GetTradingDaysWithinRangeAdjustedToTime(
                DateTime.Parse("2018/01/01"),
                DateTime.Parse("2018/01/03"), 
                null);

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetTradingDaysWithinRangeAdjustedToTime_Start_Exceeds_End_Returns_Empty_List()
        {
            var marketTradingHoursManager = Build();

            var result = marketTradingHoursManager.GetTradingDaysWithinRangeAdjustedToTime(
                DateTime.Parse("2018/01/05"),
                DateTime.Parse("2018/01/03"),
                "XLON");

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetTradingDaysWithinRangeAdjustedToTime_Start_Ok_End_Ok_Cant_Find_Mic_Returns_Empty_List()
        {
            var marketTradingHoursManager = Build();

            var result = marketTradingHoursManager.GetTradingDaysWithinRangeAdjustedToTime(
                DateTime.Parse("2018/01/01"),
                DateTime.Parse("2018/01/03"),
                "TEST");

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetTradingDaysWithinRangeAdjustedToTime_Start_Ok_End_Ok_Can_Find_Mic_Returns_3_Dates()
        {
            var marketTradingHoursManager = Build();

            var exchangeDto = new ExchangeDto
            {
                Code = "XLON",
                MarketOpenTime = TimeSpan.FromHours(8),
                MarketCloseTime = TimeSpan.FromHours(16)
            };

            A
                .CallTo(() => _marketOpenCloseRepository.Get())
                .Returns(new ExchangeDto[] { exchangeDto});

            var result = marketTradingHoursManager.GetTradingDaysWithinRangeAdjustedToTime(
                DateTime.Parse("2018/01/01"),
                DateTime.Parse("2018/01/03"),
                "XLON");

            Assert.AreEqual(result.Count, 3);
        }

        [Test]
        public void GetTradingDaysWithinRangeAdjustedToTime_Start_Ok_End_Ok_Can_Find_Mic_Returns_2_Dates()
        {
            var marketTradingHoursManager = Build();

            var exchangeDto = new ExchangeDto
            {
                Code = "XLON",
                MarketOpenTime = TimeSpan.FromHours(8),
                MarketCloseTime = TimeSpan.FromHours(16),
                Holidays = new [] { new DateTime(2018, 01, 02) }
            };

            A
                .CallTo(() => _marketOpenCloseRepository.Get())
                .Returns(new ExchangeDto[] { exchangeDto });

            var result = marketTradingHoursManager.GetTradingDaysWithinRangeAdjustedToTime(
                DateTime.Parse("2018/01/01"),
                DateTime.Parse("2018/01/03"),
                "XLON");

            Assert.AreEqual(result.Count, 2);
        }

        private MarketTradingHoursManager Build()
        {
            return new MarketTradingHoursManager(_marketOpenCloseRepository, _logger);
        }
    }
}
