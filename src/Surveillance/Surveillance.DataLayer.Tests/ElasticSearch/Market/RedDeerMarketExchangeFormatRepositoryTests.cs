using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Market;
using Surveillance.ElasticSearchDtos.Market;

namespace Surveillance.DataLayer.Tests.ElasticSearch.Market
{
    [TestFixture]
    public class RedDeerMarketExchangeFormatRepositoryTests
    {
        private IElasticSearchDataAccess _dataAccess;

        [SetUp]
        public void Setup()
        {
            _dataAccess = A.Fake<IElasticSearchDataAccess>();
        }

        [Test]
        public void Constructor_ConsidersNullDataAccess_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new RedDeerMarketExchangeFormatRepository(null));
        }

        [Test]
        public async Task Save_NullDocument_DoesNothing()
        {
            var repo = new RedDeerMarketExchangeFormatRepository(_dataAccess);

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
            var repo = new RedDeerMarketExchangeFormatRepository(_dataAccess);
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
