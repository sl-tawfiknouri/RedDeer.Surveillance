namespace Surveillance.Api.Tests.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Queries;

    using Surveillance.Api.DataAccess.Entities;

    public class ClientAccountTests : BaseTest
    {
        [Test]
        public async Task CanRequest_ClientAccount_AllFields()
        {
            // arrange
            this._dbContext.DbOrderAllocations.Add(
                new OrdersAllocation { Id = 1, Live = true, ClientAccountId = "client account id 1" });
            this._dbContext.DbOrderAllocations.Add(
                new OrdersAllocation { Id = 2, Live = true, ClientAccountId = "client account id 1" });

            await this._dbContext.SaveChangesAsync();

            var query = new ClientAccountQuery();
            query.Filter.Node.FieldId();

            // act
            var clientAccounts = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(clientAccounts, Has.Count.EqualTo(1));
            var clientAccount = clientAccounts[0];
            Assert.That(clientAccount.Id, Is.EqualTo("client account id 1"));
        }
    }
}