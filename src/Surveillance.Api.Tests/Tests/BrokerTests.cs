namespace Surveillance.Api.Tests.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Queries;

    using Surveillance.Api.DataAccess.Entities;

    /// <summary>
    /// The broker tests.
    /// </summary>
    [TestFixture]
    public class BrokerTests : BaseTest
    {
        /// <summary>
        /// The can request broker all fields.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task CanRequestBrokerAllFields()
        {
            // arrange
            this._dbContext.DbBrokers.Add(
                new Broker
                    {
                        Id = 1,
                        ExternalId = string.Empty,
                        Name = "TOURMALINE",
                        CreatedOn = new DateTime(2019, 07, 15, 8, 56, 56)
                });

            this._dbContext.DbBrokers.Add(
                new Broker
                    {
                        Id = 4751,
                        ExternalId = string.Empty,
                        Name = string.Empty,
                        CreatedOn = new DateTime(2019, 07, 15, 15, 0, 0)
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new BrokerQuery();
            query.Filter.Node.FieldId().FieldName().FieldCreatedOn().FieldExternalId();

            // act
            var brokers = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            var databaseBrokers = this._dbContext.DbBrokers.ToList();
            Assert.That(brokers, Has.Count.EqualTo(databaseBrokers.Count));

            for (var i = 0; i < this._dbContext.DbMarkets.Count(); i++)
            {
                var expected = databaseBrokers[i];
                var actual = brokers[i];

                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.ExternalId, Is.EqualTo(expected.ExternalId));
                Assert.That(actual.Name, Is.EqualTo(expected.Name));
                Assert.That(actual.CreatedOn, Is.EqualTo(expected.CreatedOn));
            }
        }
    }
}