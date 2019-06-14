using NUnit.Framework;
using RedDeer.Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class FundTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Fund_AllFields()
        {
            // arrange
            _dbContext.DbOrderAllocations.Add(new OrdersAllocation
            {
                Id = 1,
                Live = true,
                Fund = "Fund 1"
            });
            _dbContext.DbOrderAllocations.Add(new OrdersAllocation
            {
                Id = 2,
                Live = true,
                Fund = "Fund 1"
            });

            await _dbContext.SaveChangesAsync();

            var query = new FundQuery();
            query
                .Filter
                    .Node
                        .FieldName();

            // act
            var funds = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(funds, Has.Count.EqualTo(1));
            var fund = funds[0];
            Assert.That(fund.Name, Is.EqualTo("Fund 1"));
        }
    }
}
