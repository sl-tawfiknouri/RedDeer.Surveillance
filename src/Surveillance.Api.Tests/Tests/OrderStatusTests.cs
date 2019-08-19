namespace Surveillance.Api.Tests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Enums;
    using RedDeer.Surveillance.Api.Client.Queries;

    using Surveillance.Api.DataAccess.Entities;

    public class OrderStatusTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Amended_Orders()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found because is Filled
                        Id = 5, FilledDate = DateTime.UtcNow, AmendedDate = DateTime.UtcNow
                    });
            this._dbContext.DbOrders.Add(new Order { Id = 6, AmendedDate = DateTime.UtcNow });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentStatuses(new List<OrderStatus> { OrderStatus.Amended }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Booked_Orders()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found because is Amended
                        Id = 5, AmendedDate = DateTime.UtcNow, BookedDate = DateTime.UtcNow
                    });
            this._dbContext.DbOrders.Add(new Order { Id = 6, BookedDate = DateTime.UtcNow });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentStatuses(new List<OrderStatus> { OrderStatus.Booked }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Cancelled_Orders()
        {
            // arrange
            this._dbContext.DbOrders.Add(new Order { Id = 5, CancelledDate = DateTime.UtcNow });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found because is Amended
                        Id = 6, AmendedDate = DateTime.UtcNow
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentStatuses(new List<OrderStatus> { OrderStatus.Cancelled }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(5));
        }

        [Test]
        public async Task CanRequest_Filled_Orders()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found because is Rejected
                        Id = 5, RejectedDate = DateTime.UtcNow, FilledDate = DateTime.UtcNow
                    });
            this._dbContext.DbOrders.Add(new Order { Id = 6, FilledDate = DateTime.UtcNow });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentStatuses(new List<OrderStatus> { OrderStatus.Filled }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Placed_Orders()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found because is Booked
                        Id = 5, BookedDate = DateTime.UtcNow, PlacedDate = DateTime.UtcNow
                    });
            this._dbContext.DbOrders.Add(new Order { Id = 6, PlacedDate = DateTime.UtcNow });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentStatuses(new List<OrderStatus> { OrderStatus.Placed }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Rejected_Orders()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found because is Cancelled
                        Id = 5, CancelledDate = DateTime.UtcNow, RejectedDate = DateTime.UtcNow
                    });
            this._dbContext.DbOrders.Add(new Order { Id = 6, RejectedDate = DateTime.UtcNow });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found because is Unknown
                        Id = 7
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentStatuses(new List<OrderStatus> { OrderStatus.Rejected }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }

        [Test]
        public async Task CanRequest_Unknown_Orders()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found because is Placed
                        Id = 5, PlacedDate = DateTime.UtcNow
                    });
            this._dbContext.DbOrders.Add(new Order { Id = 6 });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentStatuses(new List<OrderStatus> { OrderStatus.Unknown }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(6));
        }
    }
}