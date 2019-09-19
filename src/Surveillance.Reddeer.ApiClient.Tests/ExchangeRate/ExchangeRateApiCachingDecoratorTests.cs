namespace Surveillance.Reddeer.ApiClient.Tests.ExchangeRate
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Reddeer.ApiClient.ExchangeRate;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The exchange rate caching decorator tests.
    /// </summary>
    [TestFixture]
    public class ExchangeRateApiCachingDecoratorTests
    {
        /// <summary>
        /// The exchange rates.
        /// </summary>
        private IExchangeRateApi exchangeRates;

        /// <summary>
        /// The constructor considers null throws exception.
        /// </summary>
        [Test]
        public void ConstructorConsidersNullApiThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new ExchangeRateApiCachingDecorator(null));
        }

        /// <summary>
        /// The get fetches from exchange rates.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task GetFetchesFromApi()
        {
            var repo = new ExchangeRateApiCachingDecorator(this.exchangeRates);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.GetAsync(start, end);

            A.CallTo(() => this.exchangeRates.GetAsync(start, end)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The get fetches from only once with caching.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task GetFetchesFromApiOnlyOnceWithCaching()
        {
            var repo = new ExchangeRateApiCachingDecorator(this.exchangeRates);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.GetAsync(start, end);
            await repo.GetAsync(start, end);

            A.CallTo(() => this.exchangeRates.GetAsync(start, end)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The get fetches from twice with caching expiring.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task GetFetchesFromApiTwiceWithCachingExpiring()
        {
            var repo = new ExchangeRateApiCachingDecorator(this.exchangeRates) { Expiry = TimeSpan.Zero };

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.GetAsync(start, end);
            await repo.GetAsync(start, end);

            A.CallTo(() => this.exchangeRates.GetAsync(start, end)).MustHaveHappenedTwiceExactly();
        }

        /// <summary>
        /// The get fetches from twice with same date but different times.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task GetFetchesFromApiTwiceWithSameDateButDifferentTimes()
        {
            var repo = new ExchangeRateApiCachingDecorator(this.exchangeRates);

            var start = new DateTime(2018, 1, 1);
            var end = new DateTime(2018, 1, 2);

            await repo.GetAsync(start, end);
            await repo.GetAsync(start.AddMinutes(5), end.AddMinutes(5));

            A.CallTo(() => this.exchangeRates.GetAsync(start, end)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.exchangeRates = A.Fake<IExchangeRateApi>();
        }
    }
}