namespace Surveillance.Api.Tests.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Queries;

    using Surveillance.Api.DataAccess.Entities;

    public class TraderTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Trade_AllFields()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order { Id = 1, Live = true, TraderId = "TraderId-1", TraderName = "TraderName 1" });

            await this._dbContext.SaveChangesAsync();

            var query = new TraderQuery();
            query.Filter.Node.FieldId().FieldName();

            // act
            var traders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(traders, Has.Count.EqualTo(1));
            var trader = traders[0];
            Assert.That(trader.Id, Is.EqualTo("TraderId-1"));
            Assert.That(trader.Name, Is.EqualTo("TraderName 1"));
        }
    }
}