using System;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.Api.ExchangeRate;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;

namespace Surveillance.DataLayer.Tests.Api.ExchangeRate
{
    [TestFixture]
    public class ExchangeRateApiCachingDecoratorRepositoryTests
    {
        private IExchangeRateApiRepository _apiRepository;

        [SetUp]
        public void Setup()
        {
            _apiRepository = A.Fake<IExchangeRateApiRepository>();
        }

        [Test]
        public void Constructor_ConsidersNullApiRepository_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ExchangeRateApiCachingDecoratorRepository(null));
        }

        [Test]
        public async Task Get_FetchesFromApi()
        {
            var repo = new ExchangeRateApiCachingDecoratorRepository(_apiRepository);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.Get(start, end);

            A.CallTo(() => _apiRepository.Get(start, end)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Get_FetchesFromApi_OnlyOnceWithCaching()
        {
            var repo = new ExchangeRateApiCachingDecoratorRepository(_apiRepository);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.Get(start, end);
            await repo.Get(start, end);

            A.CallTo(() => _apiRepository.Get(start, end)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Get_FetchesFromApi_TwiceWithCachingExpiring()
        {
            var repo = new ExchangeRateApiCachingDecoratorRepository(_apiRepository)
            {
                Expiry = TimeSpan.Zero
            };

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.Get(start, end);
            await repo.Get(start, end);

            A.CallTo(() => _apiRepository.Get(start, end)).MustHaveHappenedTwiceExactly();
        }

        [Test]
        public async Task Get_FetchesFromApi_TwiceWithSameDateButDifferentTimes()
        {
            var repo = new ExchangeRateApiCachingDecoratorRepository(_apiRepository);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.Get(start, end);
            await repo.Get(start.AddMinutes(5), end.AddMinutes(5));

            A.CallTo(() => _apiRepository.Get(start, end)).MustHaveHappenedOnceExactly();
        }
    }
}
