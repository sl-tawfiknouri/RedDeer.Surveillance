using System;
using DataSynchroniser.Api.Bmll.Bmll;
using FakeItEasy;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces;

namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    [TestFixture]
    public class BmllDataRequestsGetTimeBarsTests
    {
        private IBmllTimeBarApi _timeBarRepository;
        private ILogger<BmllDataRequestsGetTimeBars> _logger;

        [SetUp]
        public void Setup()
        {
            _timeBarRepository = A.Fake<IBmllTimeBarApi>();
            _logger = new NullLogger<BmllDataRequestsGetTimeBars>();
        }

        [Test]
        public void Constructor_TimeBarRepository_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsGetTimeBars(null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsGetTimeBars(_timeBarRepository, null));
        }

        [Test]
        public void GetTimeBars_Null_Keys_Returns_Empty_Collection()
        {
            var getTimeBars = BuildGetTimeBars();

            var result = getTimeBars.GetTimeBars(null);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetTimeBars_Empty_Keys_Returns_Empty_Collection()
        {
            var getTimeBars = BuildGetTimeBars();

            var result = getTimeBars.GetTimeBars(new MinuteBarRequestKeyDto[0]);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetTimeBars_OneKey_Calls_TimeBarRepository_Once()
        {
            var getTimeBars = BuildGetTimeBars();
            var keyDtos = new MinuteBarRequestKeyDto[]
            {
                new MinuteBarRequestKeyDto("a-figi", "1min", new DateTime(2019, 01, 01))
            };

            var result = getTimeBars.GetTimeBars(keyDtos);

            A
                .CallTo(() => _timeBarRepository.GetMinuteBars(A<GetMinuteBarsRequest>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetTimeBars_TwoKey_SameKey_Calls_TimeBarRepository_Once()
        {
            var getTimeBars = BuildGetTimeBars();
            var keyDtos = new MinuteBarRequestKeyDto[]
            {
                new MinuteBarRequestKeyDto("a-figi", "1min", new DateTime(2019, 01, 01)),
                new MinuteBarRequestKeyDto("a-figi", "1min", new DateTime(2019, 01, 01)),
            };

            var result = getTimeBars.GetTimeBars(keyDtos);

            A
                .CallTo(() => _timeBarRepository.GetMinuteBars(A<GetMinuteBarsRequest>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetTimeBars_TwoKey_DifferentKey_Calls_TimeBarRepository_Twice()
        {
            var getTimeBars = BuildGetTimeBars();
            var keyDtos = new MinuteBarRequestKeyDto[]
            {
                new MinuteBarRequestKeyDto("a-figi-1", "1min", new DateTime(2019, 01, 01)),
                new MinuteBarRequestKeyDto("a-figi-2", "1min", new DateTime(2019, 01, 01)),
            };

            var result = getTimeBars.GetTimeBars(keyDtos);

            A
                .CallTo(() => _timeBarRepository.GetMinuteBars(A<GetMinuteBarsRequest>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void GetTimeBars_ThreeKey_DifferentKey_Calls_TimeBarRepository_Twice()
        {
            var getTimeBars = BuildGetTimeBars();
            var keyDtos = new MinuteBarRequestKeyDto[]
            {
                new MinuteBarRequestKeyDto("a-figi-1", "1min", new DateTime(2019, 01, 01)),
                new MinuteBarRequestKeyDto("a-figi-1", "1min", new DateTime(2019, 01, 01)),
                new MinuteBarRequestKeyDto("a-figi-2", "1min", new DateTime(2019, 01, 01)),
            };

            var result = getTimeBars.GetTimeBars(keyDtos);

            A
                .CallTo(() => _timeBarRepository.GetMinuteBars(A<GetMinuteBarsRequest>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        private BmllDataRequestsGetTimeBars BuildGetTimeBars()
        {
            return new BmllDataRequestsGetTimeBars(_timeBarRepository, _logger);
        }

    }
}
