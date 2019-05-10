using NUnit.Framework;
using Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class OrderAggregationTests : BaseTest
    {
        [Test]
        public async Task CanRequest_OrderAggregation()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order
            {
                Id = 4,
                PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc)
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 5,
                PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc)
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 6,
                PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc)
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 7,
                PlacedDate = new DateTime(2019, 05, 13, 07, 50, 05, DateTimeKind.Utc)
            });

            await _dbContext.SaveChangesAsync();

            var query = new OrderAggregationQuery();
            query
                .Filter
                    .Node
                        .FieldKey()
                        .FieldCount();

            // act
            var aggregations = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(aggregations, Has.Count.EqualTo(3));
            Assert.That(aggregations[0].Key, Is.EqualTo("2019-05-13"));
            Assert.That(aggregations[0].Count, Is.EqualTo(1));
            Assert.That(aggregations[1].Key, Is.EqualTo("2019-05-11"));
            Assert.That(aggregations[1].Count, Is.EqualTo(2));
            Assert.That(aggregations[2].Key, Is.EqualTo("2019-05-10"));
            Assert.That(aggregations[2].Count, Is.EqualTo(1));
        }
    }
}
