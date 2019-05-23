using NUnit.Framework;
using RedDeer.Surveillance.Api.Client.Enums;
using RedDeer.Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class OrderStatusTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Cancelled_Orders()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order
            {
                Id = 5,
                CancelledDate = DateTime.UtcNow
            });
            _dbContext.DbOrders.Add(new Order // not to be found because is Amended
            {
                Id = 6,
                AmendedDate = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query
                .Filter
                    .ArgumentStatuses(new List<OrderStatus> { OrderStatus.Cancelled })
                    .Node
                        .FieldId();

            // act
            var orders = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(5));
        }

        [Test]
        public async Task CanRequest_Rejected_Orders()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order // not to be found because is Cancelled
            {
                Id = 5,
                CancelledDate = DateTime.UtcNow,
                RejectedDate = DateTime.UtcNow
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 6,
                RejectedDate = DateTime.UtcNow
            });
            _dbContext.DbOrders.Add(new Order // not to be found because is Unknown
            {
                Id = 7
            });

            await _dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query
                .Filter
                    .ArgumentStatuses(new List<OrderStatus> { OrderStatus.Rejected })
                    .Node
                        .FieldId();

            // act
            var orders = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Filled_Orders()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order // not to be found because is Rejected
            {
                Id = 5,
                RejectedDate = DateTime.UtcNow,
                FilledDate = DateTime.UtcNow
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 6,
                FilledDate = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query
                .Filter
                    .ArgumentStatuses(new List<OrderStatus> { OrderStatus.Filled })
                    .Node
                        .FieldId();

            // act
            var orders = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Amended_Orders()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order // not to be found because is Filled
            {
                Id = 5,
                FilledDate = DateTime.UtcNow,
                AmendedDate = DateTime.UtcNow
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 6,
                AmendedDate = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query
                .Filter
                    .ArgumentStatuses(new List<OrderStatus> { OrderStatus.Amended })
                    .Node
                        .FieldId();

            // act
            var orders = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Booked_Orders()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order // not to be found because is Amended
            {
                Id = 5,
                AmendedDate = DateTime.UtcNow,
                BookedDate = DateTime.UtcNow
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 6,
                BookedDate = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query
                .Filter
                    .ArgumentStatuses(new List<OrderStatus> { OrderStatus.Booked })
                    .Node
                        .FieldId();

            // act
            var orders = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Placed_Orders()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order // not to be found because is Booked
            {
                Id = 5,
                BookedDate = DateTime.UtcNow,
                PlacedDate = DateTime.UtcNow
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 6,
                PlacedDate = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query
                .Filter
                    .ArgumentStatuses(new List<OrderStatus> { OrderStatus.Placed })
                    .Node
                        .FieldId();

            // act
            var orders = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Unknown_Orders()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order // not to be found because is Placed
            {
                Id = 5,
                PlacedDate = DateTime.UtcNow
            });
            _dbContext.DbOrders.Add(new Order
            {
                Id = 6
            });

            await _dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query
                .Filter
                    .ArgumentStatuses(new List<OrderStatus> { OrderStatus.Unknown })
                    .Node
                        .FieldId();

            // act
            var orders = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }
    }
}
