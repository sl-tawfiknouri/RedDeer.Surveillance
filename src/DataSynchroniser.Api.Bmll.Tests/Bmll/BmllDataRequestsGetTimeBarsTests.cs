namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    using System;

    using DataSynchroniser.Api.Bmll.Bmll;

    using FakeItEasy;

    using Firefly.Service.Data.BMLL.Shared.Dtos;
    using Firefly.Service.Data.BMLL.Shared.Requests;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces;

    [TestFixture]
    public class BmllDataRequestsGetTimeBarsTests
    {
        private ILogger<BmllDataRequestsGetTimeBars> _logger;

        private IBmllTimeBarApi _timeBarRepository;

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsGetTimeBars(this._timeBarRepository, null));
        }

        [Test]
        public void Constructor_TimeBarRepository_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsGetTimeBars(null, this._logger));
        }

        [Test]
        public void GetTimeBars_Empty_Keys_Returns_Empty_Collection()
        {
            var getTimeBars = this.BuildGetTimeBars();

            var result = getTimeBars.GetTimeBars(new MinuteBarRequestKeyDto[0]);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetTimeBars_Null_Keys_Returns_Empty_Collection()
        {
            var getTimeBars = this.BuildGetTimeBars();

            var result = getTimeBars.GetTimeBars(null);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetTimeBars_OneKey_Calls_TimeBarRepository_Once()
        {
            var getTimeBars = this.BuildGetTimeBars();
            var keyDtos = new[] { new MinuteBarRequestKeyDto("a-figi", "1min", new DateTime(2019, 01, 01)) };

            var result = getTimeBars.GetTimeBars(keyDtos);

            A.CallTo(() => this._timeBarRepository.GetMinuteBars(A<GetMinuteBarsRequest>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetTimeBars_ThreeKey_DifferentKey_Calls_TimeBarRepository_Twice()
        {
            var getTimeBars = this.BuildGetTimeBars();
            var keyDtos = new[]
                              {
                                  new MinuteBarRequestKeyDto("a-figi-1", "1min", new DateTime(2019, 01, 01)),
                                  new MinuteBarRequestKeyDto("a-figi-1", "1min", new DateTime(2019, 01, 01)),
                                  new MinuteBarRequestKeyDto("a-figi-2", "1min", new DateTime(2019, 01, 01))
                              };

            var result = getTimeBars.GetTimeBars(keyDtos);

            A.CallTo(() => this._timeBarRepository.GetMinuteBars(A<GetMinuteBarsRequest>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void GetTimeBars_TwoKey_DifferentKey_Calls_TimeBarRepository_Twice()
        {
            var getTimeBars = this.BuildGetTimeBars();
            var keyDtos = new[]
                              {
                                  new MinuteBarRequestKeyDto("a-figi-1", "1min", new DateTime(2019, 01, 01)),
                                  new MinuteBarRequestKeyDto("a-figi-2", "1min", new DateTime(2019, 01, 01))
                              };

            var result = getTimeBars.GetTimeBars(keyDtos);

            A.CallTo(() => this._timeBarRepository.GetMinuteBars(A<GetMinuteBarsRequest>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void GetTimeBars_TwoKey_SameKey_Calls_TimeBarRepository_Once()
        {
            var getTimeBars = this.BuildGetTimeBars();
            var keyDtos = new[]
                              {
                                  new MinuteBarRequestKeyDto("a-figi", "1min", new DateTime(2019, 01, 01)),
                                  new MinuteBarRequestKeyDto("a-figi", "1min", new DateTime(2019, 01, 01))
                              };

            var result = getTimeBars.GetTimeBars(keyDtos);

            A.CallTo(() => this._timeBarRepository.GetMinuteBars(A<GetMinuteBarsRequest>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._timeBarRepository = A.Fake<IBmllTimeBarApi>();
            this._logger = new NullLogger<BmllDataRequestsGetTimeBars>();
        }

        private BmllDataRequestsGetTimeBars BuildGetTimeBars()
        {
            return new BmllDataRequestsGetTimeBars(this._timeBarRepository, this._logger);
        }
    }
}