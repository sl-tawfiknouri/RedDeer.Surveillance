﻿namespace Surveillance.Reddeer.ApiClient.Tests.ExchangeRate
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Reddeer.ApiClient.ExchangeRate;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;

    [TestFixture]
    public class ExchangeRateApiCachingDecoratorRepositoryTests
    {
        private IExchangeRateApi _api;

        [Test]
        public void Constructor_ConsidersNullApiRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ExchangeRateApiCachingDecorator(null));
        }

        [Test]
        public async Task Get_FetchesFromApi()
        {
            var repo = new ExchangeRateApiCachingDecorator(this._api);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.Get(start, end);

            A.CallTo(() => this._api.Get(start, end)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Get_FetchesFromApi_OnlyOnceWithCaching()
        {
            var repo = new ExchangeRateApiCachingDecorator(this._api);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.Get(start, end);
            await repo.Get(start, end);

            A.CallTo(() => this._api.Get(start, end)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Get_FetchesFromApi_TwiceWithCachingExpiring()
        {
            var repo = new ExchangeRateApiCachingDecorator(this._api) { Expiry = TimeSpan.Zero };

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.Get(start, end);
            await repo.Get(start, end);

            A.CallTo(() => this._api.Get(start, end)).MustHaveHappenedTwiceExactly();
        }

        [Test]
        public async Task Get_FetchesFromApi_TwiceWithSameDateButDifferentTimes()
        {
            var repo = new ExchangeRateApiCachingDecorator(this._api);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.Get(start, end);
            await repo.Get(start.AddMinutes(5), end.AddMinutes(5));

            A.CallTo(() => this._api.Get(start, end)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._api = A.Fake<IExchangeRateApi>();
        }
    }
}