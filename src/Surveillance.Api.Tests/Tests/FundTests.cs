namespace Surveillance.Api.Tests.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Queries;

    using Surveillance.Api.DataAccess.Entities;

    public class FundTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Fund_AllFields()
        {
            // arrange
            this._dbContext.DbOrderAllocations.Add(new OrdersAllocation { Id = 1, Live = true, Fund = "Fund 1" });
            this._dbContext.DbOrderAllocations.Add(new OrdersAllocation { Id = 2, Live = true, Fund = "Fund 1" });

            await this._dbContext.SaveChangesAsync();

            var query = new FundQuery();
            query.Filter.Node.FieldName();

            // act
            var funds = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(funds, Has.Count.EqualTo(1));
            var fund = funds[0];
            Assert.That(fund.Name, Is.EqualTo("Fund 1"));
        }
    }
}