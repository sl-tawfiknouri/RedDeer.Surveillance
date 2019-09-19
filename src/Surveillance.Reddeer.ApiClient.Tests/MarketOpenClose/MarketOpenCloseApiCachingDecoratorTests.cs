namespace Surveillance.Reddeer.ApiClient.Tests.MarketOpenClose
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Reddeer.ApiClient.MarketOpenClose;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The market open close caching decorator tests.
    /// </summary>
    [TestFixture]
    public class MarketOpenCloseApiCachingDecoratorTests
    {
        /// <summary>
        /// The market open close.
        /// </summary>
        private IMarketOpenCloseApi marketOpenClose;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<MarketOpenCloseApiCachingDecorator> logger;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.marketOpenClose = A.Fake<IMarketOpenCloseApi>();
            this.logger = A.Fake<ILogger<MarketOpenCloseApiCachingDecorator>>();
        }

        /// <summary>
        /// The constructor throws for null logger.
        /// </summary>
        [Test]
        public void ConstructorThrowsForNullLogger()
        {
            Assert.Throws<ArgumentNullException>(
                () => new MarketOpenCloseApiCachingDecorator(this.marketOpenClose, null));
        }

        /// <summary>
        /// The constructor throws for null repository.
        /// </summary>
        [Test]
        public void ConstructorThrowsForNullRepository()
        {
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiCachingDecorator(null, this.logger));
        }

        /// <summary>
        /// The get fetches from repository for initial request.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task GetFetchesFromRepositoryForInitialRequest()
        {
            var repository = new MarketOpenCloseApiCachingDecorator(this.marketOpenClose, this.logger);

            await repository.GetAsync();

            A.CallTo(() => this.marketOpenClose.GetAsync()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The get fetches once from repository for multiple requests.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task GetFetchesOnceFromRepositoryForMultipleRequests()
        {
            var repository = new MarketOpenCloseApiCachingDecorator(this.marketOpenClose, this.logger);

            await repository.GetAsync();
            await repository.GetAsync();
            await repository.GetAsync();

            A.CallTo(() => this.marketOpenClose.GetAsync()).MustHaveHappenedOnceExactly();
        }
    }
}