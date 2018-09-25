using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Market;
using Surveillance.ElasticSearchDtos.Market;

namespace Surveillance.DataLayer.Tests.ElasticSearch.Market
{
    [TestFixture]
    public class RedDeerMarketExchangeFormatRepositoryTests
    {
        private IElasticSearchDataAccess _dataAccess;
        private IMarketIndexNameBuilder _marketIndexNameBuilder;

        [SetUp]
        public void Setup()
        {
            _dataAccess = A.Fake<IElasticSearchDataAccess>();
            _marketIndexNameBuilder = A.Fake<IMarketIndexNameBuilder>();
        }

        [Test]
        public void Constructor_ConsidersNullDataAccess_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RedDeerMarketExchangeFormatRepository(null, _marketIndexNameBuilder));
        }

        [Test]
        public void Constructor_ConsidersNullMarketIndexNamer_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RedDeerMarketExchangeFormatRepository(_dataAccess, null));
        }

        [Test]
        public async Task Save_NullDocument_DoesNothing()
        {
            var repo = new RedDeerMarketExchangeFormatRepository(_dataAccess, _marketIndexNameBuilder);

            await repo.Save(null);

            A.CallTo(() => 
                _dataAccess.GetOrCreateDateBasedIndexAsync<ReddeerMarketDocument>(
                    A<string>.Ignored,
                    A<DateTime>.Ignored,
                    A<CancellationToken>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(() =>
                _dataAccess.IndexDocumentAsync(
                    A<string>.Ignored,
                    A<ReddeerMarketDocument>.Ignored,
                    A<DateTime>.Ignored,
                    A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Save_CreatesIndexAndIndexesDocument()
        {
            var repo = new RedDeerMarketExchangeFormatRepository(_dataAccess, _marketIndexNameBuilder);
            var document = new ReddeerMarketDocument();

            await repo.Save(document);

            A.CallTo(() =>
                    _dataAccess.GetOrCreateDateBasedIndexAsync<ReddeerMarketDocument>(
                        A<string>.Ignored,
                        A<DateTime>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustHaveHappened();

            A.CallTo(() =>
                    _dataAccess.IndexDocumentAsync(
                        A<string>.Ignored,
                        A<ReddeerMarketDocument>.Ignored,
                        A<DateTime>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustHaveHappened();
        }
    }
}
