namespace Surveillance.Api.Tests.Tests
{

    using NUnit.Framework;
    using RedDeer.Surveillance.Api.Client.Queries;
    using Surveillance.Api.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class OrderAggregationTests : BaseTest
    {
        [Test]
        public async Task CanRequest_OrderAggregation()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order { Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5,
                        PlacedDate = new DateTime(
                            2019,
                            05,
                            10,
                            23,
                            30,
                            00,
                            DateTimeKind.Utc) // BST conversion will end up on 11th
                    });
            this._dbContext.DbOrders.Add(
                new Order { Id = 6, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 7,
                        PlacedDate = new DateTime(
                            2019,
                            10,
                            27,
                            01,
                            00,
                            00,
                            DateTimeKind.Utc) // datetime of BST -> GMT switch
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderAggregationQuery();
            query.Filter.ArgumentPlacedDateFrom(new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc))
                .ArgumentPlacedDateTo(new DateTime(2020, 01, 01, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentTzName("Europe/London").Node.FieldKey().FieldCount();

            // act
            var aggregations = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(aggregations, Has.Count.EqualTo(3));
            Assert.That(aggregations[0].Key, Is.EqualTo("2019-05-10"));
            Assert.That(aggregations[0].Count, Is.EqualTo(1));
            Assert.That(aggregations[1].Key, Is.EqualTo("2019-05-11"));
            Assert.That(aggregations[1].Count, Is.EqualTo(2));
            Assert.That(aggregations[2].Key, Is.EqualTo("2019-10-27"));
            Assert.That(aggregations[2].Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CanRequest_OrderAggregation_ById()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order { Id = 4, PlacedDate = new DateTime(2019, 01, 09, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5,
                        PlacedDate = new DateTime(2019, 02, 11, 07, 50, 05, DateTimeKind.Utc) // not in daylight savings
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 6,
                        PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc) // in daylight savings
                    });
            this._dbContext.DbOrders.Add(
                new Order { Id = 7, PlacedDate = new DateTime(2019, 05, 13, 07, 50, 05, DateTimeKind.Utc) });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderAggregationQuery();
            query.Filter.ArgumentIds(new List<int> { 5, 6 })
                .ArgumentPlacedDateFrom(new DateTime(2019, 01, 01, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentPlacedDateTo(new DateTime(2020, 01, 01, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentTzName("America/Los_Angeles").Node.FieldKey().FieldCount();

            // act
            var aggregations = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(aggregations, Has.Count.EqualTo(2));
            Assert.That(aggregations[0].Key, Is.EqualTo("2019-02-10"));
            Assert.That(aggregations[0].Count, Is.EqualTo(1));
            Assert.That(aggregations[1].Key, Is.EqualTo("2019-05-11"));
            Assert.That(aggregations[1].Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CanRequest_OrderAggregation_ByReddeerId()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc), SecurityId = 22
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc), SecurityId = 24
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 6, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc), SecurityId = 27
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 7, PlacedDate = new DateTime(2019, 05, 13, 07, 50, 05, DateTimeKind.Utc), SecurityId = 24
                    });

            this._dbContext.DbDFinancialInstruments.Add(new FinancialInstrument { Id = 22, ReddeerId = "abc" });
            this._dbContext.DbDFinancialInstruments.Add(new FinancialInstrument { Id = 24, ReddeerId = "qwe" });
            this._dbContext.DbDFinancialInstruments.Add(new FinancialInstrument { Id = 27, ReddeerId = "xyz" });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderAggregationQuery();
            query.Filter.ArgumentReddeerIds(new List<string> { "abc", "xyz" })
                .ArgumentPlacedDateFrom(new DateTime(2019, 01, 01, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentPlacedDateTo(new DateTime(2020, 01, 01, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentTzName("Europe/Paris").Node.FieldKey().FieldCount();

            // act
            var aggregations = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(aggregations, Has.Count.EqualTo(2));
            Assert.That(aggregations[0].Key, Is.EqualTo("2019-05-10"));
            Assert.That(aggregations[0].Count, Is.EqualTo(1));
            Assert.That(aggregations[1].Key, Is.EqualTo("2019-05-11"));
            Assert.That(aggregations[1].Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CanRequest_OrderAggregation_ByTraderId()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc), TraderId = "vic"
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc), TraderId = "jim"
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 6, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc), TraderId = "jim"
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 7, PlacedDate = new DateTime(2019, 05, 13, 07, 50, 05, DateTimeKind.Utc), TraderId = "bob"
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderAggregationQuery();
            query.Filter.ArgumentTraderIds(new HashSet<string> { "vic", "bob" })
                .ArgumentPlacedDateFrom(new DateTime(2019, 01, 01, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentPlacedDateTo(new DateTime(2020, 01, 01, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentTzName("Europe/Paris").Node.FieldKey().FieldCount();

            // act
            var aggregations = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(aggregations, Has.Count.EqualTo(2));
            Assert.That(aggregations[0].Key, Is.EqualTo("2019-05-10"));
            Assert.That(aggregations[0].Count, Is.EqualTo(1));
            Assert.That(aggregations[1].Key, Is.EqualTo("2019-05-13"));
            Assert.That(aggregations[1].Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CanRequest_OrderAggregation_WithDateRange()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order { Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order { Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order { Id = 6, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order { Id = 7, PlacedDate = new DateTime(2019, 05, 13, 07, 50, 05, DateTimeKind.Utc) });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderAggregationQuery();
            query.Filter.ArgumentPlacedDateFrom(new DateTime(2019, 05, 11, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentPlacedDateTo(new DateTime(2019, 05, 12, 00, 00, 00, DateTimeKind.Utc))
                .ArgumentTzName("America/Phoenix").Node.FieldKey().FieldCount();

            // act
            var aggregations = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(aggregations, Has.Count.EqualTo(1));
            Assert.That(aggregations[0].Key, Is.EqualTo("2019-05-11"));
            Assert.That(aggregations[0].Count, Is.EqualTo(2));
        }
    }
}