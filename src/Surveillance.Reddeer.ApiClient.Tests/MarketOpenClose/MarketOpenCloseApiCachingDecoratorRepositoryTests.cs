using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Reddeer.ApiClient.MarketOpenClose;
using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

namespace Surveillance.Reddeer.ApiClient.Tests.MarketOpenClose
{
    [TestFixture]
    public class MarketOpenCloseApiCachingDecoratorRepositoryTests
    {
        private IMarketOpenCloseApi _repository;
        private ILogger<MarketOpenCloseApiCachingDecorator> _logger;

        [SetUp]
        public void Setup()
        {
            _repository = A.Fake<IMarketOpenCloseApi>();
            _logger = A.Fake<ILogger<MarketOpenCloseApiCachingDecorator>>();
        }

        [Test]
        public void Constructor_ThrowsForNull_Repository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiCachingDecorator(null, _logger));
        }

        [Test]
        public void Constructor_ThrowsForNull_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiCachingDecorator(_repository, null));
        }

        [Test]
        public async Task Get_FetchesFromRepositoryForInitialRequest()
        {
            var repository = new MarketOpenCloseApiCachingDecorator(_repository, _logger);

            await repository.Get();

            A.CallTo(() => _repository.Get()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Get_FetchesOnceFromRepositoryForMultipleRequests()
        {
            var repository = new MarketOpenCloseApiCachingDecorator(_repository, _logger);

            await repository.Get();
            await repository.Get();
            await repository.Get();

            A.CallTo(() => _repository.Get()).MustHaveHappenedOnceExactly();
        }
    }
}
