namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Bmll.Bmll;

    using FakeItEasy;

    using Firefly.Service.Data.BMLL.Shared.Dtos;
    using Firefly.Service.Data.BMLL.Shared.Requests;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.Market.Interfaces;

    [TestFixture]
    public class BmllDataRequestsStorageManagerTests
    {
        private ILogger<BmllDataRequestsStorageManager> _logger;

        private IReddeerMarketTimeBarRepository _repository;

        [Test]
        public void Ctor_Throws_For_Null_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsStorageManager(null, null));
        }

        [SetUp]
        public void Setup()
        {
            this._repository = A.Fake<IReddeerMarketTimeBarRepository>();
            this._logger = A.Fake<ILogger<BmllDataRequestsStorageManager>>();
        }

        [Test]
        public async Task Store_DeDuplicatesAsExpected_ForRepository()
        {
            var storageManager = new BmllDataRequestsStorageManager(this._repository, this._logger);

            var response = new GetMinuteBarsResponse
                               {
                                   MinuteBars = new List<MinuteBarDto>
                                                    {
                                                        new MinuteBarDto { Figi = "test1", DateTime = DateTime.Today }
                                                    }
                               };

            var response2 = new GetMinuteBarsResponse
                                {
                                    MinuteBars = new List<MinuteBarDto>
                                                     {
                                                         new MinuteBarDto { Figi = "test2", DateTime = DateTime.Today },
                                                         new MinuteBarDto { Figi = "test1", DateTime = DateTime.Today }
                                                     }
                                };

            var timeBarPairs = new List<GetTimeBarPair>
                                   {
                                       new GetTimeBarPair(null, response), new GetTimeBarPair(null, response2)
                                   };

            await storageManager.Store(timeBarPairs);

            A.CallTo(() => this._repository.Save(A<List<MinuteBarDto>>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}