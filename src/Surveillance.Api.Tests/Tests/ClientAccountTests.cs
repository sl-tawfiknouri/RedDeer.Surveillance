using NUnit.Framework;
using RedDeer.Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class ClientAccountTests : BaseTest
    {
        [Test]
        public async Task CanRequest_ClientAccount_AllFields()
        {
            // arrange
            _dbContext.DbOrderAllocations.Add(new OrdersAllocation
            {
                Id = 1,
                Live = true,
                ClientAccountId = "client account id 1",
            });
            _dbContext.DbOrderAllocations.Add(new OrdersAllocation
            {
                Id = 2,
                Live = true,
                ClientAccountId = "client account id 1",
            });

            await _dbContext.SaveChangesAsync();

            var query = new ClientAccountQuery();
            query
                .Filter
                    .Node
                        .FieldId();

            // act
            var clientAccounts = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(clientAccounts, Has.Count.EqualTo(1));
            var clientAccount = clientAccounts[0];
            Assert.That(clientAccount.Id, Is.EqualTo("client account id 1"));
        }
    }
}
