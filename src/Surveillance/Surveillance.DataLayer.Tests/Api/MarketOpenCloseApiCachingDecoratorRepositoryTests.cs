using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Api;
using Surveillance.DataLayer.Api.MarketOpenClose;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;

namespace Surveillance.DataLayer.Tests.Api
{
    [TestFixture]
    public class MarketOpenCloseApiCachingDecoratorRepositoryTests
    {
        private IMarketOpenCloseApiRepository _repository;
        private ILogger<MarketOpenCloseApiCachingDecoratorRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _repository = A.Fake<IMarketOpenCloseApiRepository>();
            _logger = A.Fake<ILogger<MarketOpenCloseApiCachingDecoratorRepository>>();
        }

        [Test]
        public void Constructor_ThrowsForNull_Repository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiCachingDecoratorRepository(null, _logger));
        }

        [Test]
        public void Constructor_ThrowsForNull_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiCachingDecoratorRepository(_repository, null));
        }

        [Test]
        public async Task Get_FetchesFromRepositoryForInitialRequest()
        {
            var repository = new MarketOpenCloseApiCachingDecoratorRepository(_repository, _logger);

            var result = await repository.Get();

            A.CallTo(() => _repository.Get()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Get_FetchesOnceFromRepositoryForMultipleRequests()
        {
            var repository = new MarketOpenCloseApiCachingDecoratorRepository(_repository, _logger);

            var result = await repository.Get();
            await repository.Get();
            await repository.Get();

            A.CallTo(() => _repository.Get()).MustHaveHappenedOnceExactly();
        }
    }
}
