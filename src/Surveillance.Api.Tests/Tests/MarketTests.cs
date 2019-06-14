using NUnit.Framework;
using RedDeer.Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class MarketTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Market_AllFields()
        {
            // arrange
            _dbContext.DbMarkets.Add(new Market
            {
                Id = 1,
                MarketId = "M-1",
                MarketName = "Market 1"
            });
            _dbContext.DbMarkets.Add(new Market
            {
                Id = 2,
                MarketId = "M-2",
                MarketName = "Market 2"
            });

            await _dbContext.SaveChangesAsync();

            var query = new MarketQuery();
            query
                .Filter
                    .Node
                        .FieldId()
                        .FieldMarketId()
                        .FieldMarketName();

            // act
            var markets = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert

            var dbMarkets = _dbContext.DbMarkets.ToList();
            Assert.That(markets, Has.Count.EqualTo(dbMarkets.Count));

            for (int i = 0; i < _dbContext.DbMarkets.Count(); i++)
            {
                var expected = dbMarkets[i];
                var actual = markets[i];

                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.MarketId, Is.EqualTo(expected.MarketId));
                Assert.That(actual.MarketName, Is.EqualTo(expected.MarketName));
            }
        }
    }
}
