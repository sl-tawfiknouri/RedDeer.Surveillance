namespace Surveillance.Api.Tests.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Queries;

    using Surveillance.Api.DataAccess.Entities;

    public class StrategyTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Fund_AllFields()
        {
            // arrange
            this._dbContext.DbOrderAllocations.Add(
                new OrdersAllocation { Id = 1, Live = true, Strategy = "Strategy 1" });

            await this._dbContext.SaveChangesAsync();

            var query = new StrategyQuery();
            query.Filter.Node.FieldName();

            // act
            var strategies = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(strategies, Has.Count.EqualTo(1));
            var stategy = strategies[0];
            Assert.That(stategy.Name, Is.EqualTo("Strategy 1"));
        }
    }
}