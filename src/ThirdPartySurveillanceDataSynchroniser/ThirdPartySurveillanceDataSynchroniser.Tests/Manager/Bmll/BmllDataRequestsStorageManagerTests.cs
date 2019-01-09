using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Manager.Bmll
{
    [TestFixture]
    public class BmllDataRequestsStorageManagerTests
    {
        private IReddeerMarketTimeBarRepository _repository;
        private ILogger<BmllDataRequestsStorageManager> _logger;

        [SetUp]
        public void Setup()
        {
            _repository = A.Fake<IReddeerMarketTimeBarRepository>();
            _logger = A.Fake<ILogger<BmllDataRequestsStorageManager>>();
        }


        [Test]
        public void Ctor_Throws_For_Null_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsStorageManager(null, null));
        }

        [Test]
        public async Task Store_DeDuplicatesAsExpected_ForRepository()
        {
            var storageManager = new BmllDataRequestsStorageManager(_repository, _logger);

            var response = new GetMinuteBarsResponse
            {
                MinuteBars = new List<MinuteBarDto>
                {
                    new MinuteBarDto
                    {
                        Figi = "test1",
                        DateTime = DateTime.Today
                    }
                }
            };

            var response2 = new GetMinuteBarsResponse
            {
                MinuteBars = new List<MinuteBarDto>
                {
                    new MinuteBarDto
                    {
                        Figi = "test2",
                        DateTime = DateTime.Today
                    },
                    new MinuteBarDto
                    {
                        Figi = "test1",
                        DateTime = DateTime.Today
                    }
                }
            };


            var timeBarPairs = new List<GetTimeBarPair>
            {
                new GetTimeBarPair(null, response),
                new GetTimeBarPair(null, response2)
            };

            await storageManager.Store(timeBarPairs);

            A.CallTo(() => _repository.Save(A<List<MinuteBarDto>>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
