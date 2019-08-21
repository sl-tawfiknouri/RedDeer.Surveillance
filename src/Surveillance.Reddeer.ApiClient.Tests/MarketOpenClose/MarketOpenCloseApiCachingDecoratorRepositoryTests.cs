namespace Surveillance.Reddeer.ApiClient.Tests.MarketOpenClose
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Reddeer.ApiClient.MarketOpenClose;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    [TestFixture]
    public class MarketOpenCloseApiCachingDecoratorRepositoryTests
    {
        private ILogger<MarketOpenCloseApiCachingDecorator> _logger;

        private IMarketOpenCloseApi _repository;

        [Test]
        public void Constructor_ThrowsForNull_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiCachingDecorator(this._repository, null));
        }

        [Test]
        public void Constructor_ThrowsForNull_Repository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiCachingDecorator(null, this._logger));
        }

        [Test]
        public async Task Get_FetchesFromRepositoryForInitialRequest()
        {
            var repository = new MarketOpenCloseApiCachingDecorator(this._repository, this._logger);

            await repository.Get();

            A.CallTo(() => this._repository.Get()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Get_FetchesOnceFromRepositoryForMultipleRequests()
        {
            var repository = new MarketOpenCloseApiCachingDecorator(this._repository, this._logger);

            await repository.Get();
            await repository.Get();
            await repository.Get();

            A.CallTo(() => this._repository.Get()).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._repository = A.Fake<IMarketOpenCloseApi>();
            this._logger = A.Fake<ILogger<MarketOpenCloseApiCachingDecorator>>();
        }
    }
}