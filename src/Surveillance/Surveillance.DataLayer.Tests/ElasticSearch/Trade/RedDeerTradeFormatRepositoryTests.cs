using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.Tests.ElasticSearch.Trade
{
    [TestFixture]
    public class RedDeerTradeFormatRepositoryTests
    {
        private IElasticSearchDataAccess _dataAccess;

        [SetUp]
        public void Setup()
        {
            _dataAccess = A.Fake<IElasticSearchDataAccess>();
        }

        [Test]
        public void Constructor_NullDataAccessItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new RedDeerTradeFormatRepository(null));
        }

        [Test]
        public async Task Save_NullDocument_DoesNothing()
        {
            var repo = new RedDeerTradeFormatRepository(_dataAccess);

            await repo.Save(null);

            A.CallTo(() =>
                    _dataAccess.GetOrCreateDateBasedIndexAsync<ReddeerTradeDocument>(
                        A<string>.Ignored,
                        A<DateTime>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(() =>
                    _dataAccess.IndexDocumentAsync(
                        A<string>.Ignored,
                        A<ReddeerTradeDocument>.Ignored,
                        A<DateTime>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Save_DocumentCreatesIndexAnd_IndexesDocument()
        {
            var repo = new RedDeerTradeFormatRepository(_dataAccess);
            var document = new ReddeerTradeDocument();

            await repo.Save(document);

            A.CallTo(() =>
                    _dataAccess.GetOrCreateDateBasedIndexAsync<ReddeerTradeDocument>(
                        A<string>.Ignored,
                        A<DateTime>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustHaveHappened();

            A.CallTo(() =>
                    _dataAccess.IndexDocumentAsync(
                        A<string>.Ignored,
                        A<ReddeerTradeDocument>.Ignored,
                        A<DateTime>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustHaveHappened();
        }
    }
}