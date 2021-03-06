﻿namespace Surveillance.Api.Tests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders;

    using NUnit.Framework;

    using RedDeer.Surveillance.Api.Client.Enums;
    using RedDeer.Surveillance.Api.Client.Nodes;
    using RedDeer.Surveillance.Api.Client.Queries;

    using Surveillance.Api.DataAccess.Entities;

    using Order = Surveillance.Api.DataAccess.Entities.Order;

    public class OrderTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Order_AllFields()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5,
                        ClientOrderId = "order id",
                        OrderVersion = "order version",
                        OrderVersionLinkId = "order version link id",
                        OrderGroupId = "order group id",
                        OrderType = (int)OrderTypes.LIMIT,
                        Direction = (int)OrderDirections.SHORT,
                        Currency = "GBP",
                        SettlementCurrency = "USD",
                        CleanDirty = "clean dirty",
                        AccumulatedInterest = 0.6m,
                        LimitPrice = 4.3m,
                        AverageFillPrice = 7.6m,
                        OrderedVolume = 870,
                        FilledVolume = 643,
                        ClearingAgent = "clearing agent",
                        DealingInstructions = "dealing instructions",
                        OptionStrikePrice = 8.8m,
                        OptionExpirationDate = new DateTime(2019, 05, 09, 14, 42, 11, DateTimeKind.Utc),
                        OptionEuropeanAmerican = "european american",
                        PlacedDate = new DateTime(2019, 05, 09, 01, 00, 00, DateTimeKind.Utc),
                        BookedDate = new DateTime(2019, 05, 09, 02, 00, 00, DateTimeKind.Utc),
                        AmendedDate = new DateTime(2019, 05, 09, 03, 00, 00, DateTimeKind.Utc),
                        RejectedDate = new DateTime(2019, 05, 09, 04, 00, 00, DateTimeKind.Utc),
                        CancelledDate = new DateTime(2019, 05, 09, 05, 00, 00, DateTimeKind.Utc),
                        FilledDate = new DateTime(2019, 05, 09, 06, 00, 00, DateTimeKind.Utc),
                        StatusChangedDate = new DateTime(2019, 05, 09, 07, 00, 00, DateTimeKind.Utc),
                        TraderId = "trader id",
                        TraderName = "trader name",
                        MarketId = 9,
                        SecurityId = 14,
                        BrokerId = 3
                    });
            this._dbContext.DbMarkets.Add(new Market { Id = 9, MarketId = "market id", MarketName = "market name" });
            this._dbContext.DbBrokers.Add(
                new Broker
                    {
                        Id = 3, ExternalId = "ExternalBrokerId-3", Name = "Broker name", CreatedOn = DateTime.UtcNow
                    });
            this._dbContext.DbDFinancialInstruments.Add(
                new FinancialInstrument
                    {
                        Id = 14,
                        ClientIdentifier = "client identifier",
                        Sedol = "sedol value",
                        Isin = "isin value",
                        Figi = "figi value",
                        Cusip = "cusip value",
                        Lei = "lei value",
                        ExchangeSymbol = "exchange symbol",
                        BloombergTicker = "bloomber ticker",
                        SecurityName = "security name",
                        Cfi = "cfi value",
                        IssuerIdentifier = "issuer identifier",
                        ReddeerId = "reddeer id"
                    });

            var expectedOrderAllocations = new List<OrdersAllocation>
                                               {
                                                   new OrdersAllocation
                                                       {
                                                           Id = 1,
                                                           OrderId = "order id",
                                                           Fund =
                                                               "fund 1", // Cannot return null for non-null type. Field: fund, Type: String!.
                                                           Strategy = "strategy 1",
                                                           Live = true,
                                                           AutoScheduled = true,
                                                           ClientAccountId = "client account id 1",
                                                           CreatedDate = DateTime.UtcNow,
                                                           OrderFilledVolume = 21213
                                                       }
                                               };

            var nonLiveOrderAllocations = new OrdersAllocation
                                              {
                                                  Id = 2,
                                                  OrderId = "order id 2",
                                                  Fund = "fund 1",
                                                  Strategy = "strategy 1",
                                                  Live = false
                                              };

            var nonRelatedOrderAllocations = new OrdersAllocation
                                                 {
                                                     Id = 2,
                                                     OrderId = "non related order id 2",
                                                     Fund = "fund 1",
                                                     Strategy = "strategy 1",
                                                     Live = true
                                                 };

            var allOrderAllocations = new List<OrdersAllocation>();
            allOrderAllocations.AddRange(expectedOrderAllocations);
            allOrderAllocations.Add(nonLiveOrderAllocations);

            await this._dbContext.DbOrderAllocations.AddRangeAsync(expectedOrderAllocations);

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.Node.FieldId().FieldClientOrderId().FieldOrderVersion().FieldOrderVersionLinkId()
                .FieldOrderGroupId().FieldOrderType().FieldDirection().FieldCurrency().FieldSettlementCurrency()
                .FieldCleanDirty().FieldAccumulatedInterest().FieldLimitPrice().FieldAverageFillPrice()
                .FieldOrderedVolume().FieldFilledVolume().FieldClearingAgent().FieldDealingInstructions()
                .FieldOptionStrikePrice().FieldOptionExpirationDate().FieldOptionEuropeanAmerican().FieldOrderDates()
                .FieldPlaced().FieldBooked().FieldAmended().FieldRejected().FieldCancelled().FieldFilled()
                .FieldStatusChanged().Parent<OrderNode>().FieldTrader().FieldId().FieldName().Parent<OrderNode>()
                .FieldMarket().FieldId().FieldMarketId().FieldMarketName().Parent<OrderNode>()
                .FieldFinancialInstrument().FieldId().FieldClientIdentifier().FieldSedol().FieldIsin().FieldFigi()
                .FieldCusip().FieldLei().FieldExchangeSymbol().FieldBloombergTicker().FieldSecurityName().FieldCfi()
                .FieldIssuerIdentifier().FieldReddeerId().Parent<OrderNode>().FieldOrderAllocations().FieldId()
                .FieldOrderId().FieldFund().FieldStrategy().FieldClientAccountId().FieldOrderFilledVolume().FieldLive()
                .FieldAutoScheduled().FieldCreatedDate().Parent<OrderNode>().FieldBroker().FieldId().FieldExternalId()
                .FieldName().FieldCreatedOn();

            // act  
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));

            var order = orders[0];
            Assert.That(order.Id, Is.EqualTo(5));
            Assert.That(order.ClientOrderId, Is.EqualTo("order id"));
            Assert.That(order.OrderVersion, Is.EqualTo("order version"));
            Assert.That(order.OrderVersionLinkId, Is.EqualTo("order version link id"));
            Assert.That(order.OrderGroupId, Is.EqualTo("order group id"));
            Assert.That(order.OrderType, Is.EqualTo(OrderType.Limit));
            Assert.That(order.Direction, Is.EqualTo(OrderDirection.Short));
            Assert.That(order.Currency, Is.EqualTo("GBP"));
            Assert.That(order.SettlementCurrency, Is.EqualTo("USD"));
            Assert.That(order.CleanDirty, Is.EqualTo("clean dirty"));
            Assert.That(order.AccumulatedInterest, Is.EqualTo(0.6m));
            Assert.That(order.LimitPrice, Is.EqualTo(4.3m));
            Assert.That(order.AverageFillPrice, Is.EqualTo(7.6m));
            Assert.That(order.OrderedVolume, Is.EqualTo(870));
            Assert.That(order.FilledVolume, Is.EqualTo(643));
            Assert.That(order.ClearingAgent, Is.EqualTo("clearing agent"));
            Assert.That(order.DealingInstructions, Is.EqualTo("dealing instructions"));
            Assert.That(order.OptionStrikePrice, Is.EqualTo(8.8m));
            Assert.That(
                order.OptionExpirationDate,
                Is.EqualTo(new DateTime(2019, 05, 09, 14, 42, 11, DateTimeKind.Utc)));
            Assert.That(order.OptionEuropeanAmerican, Is.EqualTo("european american"));

            Assert.That(
                order.OrderDates.PlacedDate,
                Is.EqualTo(new DateTime(2019, 05, 09, 01, 00, 00, DateTimeKind.Utc)));
            Assert.That(
                order.OrderDates.BookedDate,
                Is.EqualTo(new DateTime(2019, 05, 09, 02, 00, 00, DateTimeKind.Utc)));
            Assert.That(
                order.OrderDates.AmendedDate,
                Is.EqualTo(new DateTime(2019, 05, 09, 03, 00, 00, DateTimeKind.Utc)));
            Assert.That(
                order.OrderDates.RejectedDate,
                Is.EqualTo(new DateTime(2019, 05, 09, 04, 00, 00, DateTimeKind.Utc)));
            Assert.That(
                order.OrderDates.CancelledDate,
                Is.EqualTo(new DateTime(2019, 05, 09, 05, 00, 00, DateTimeKind.Utc)));
            Assert.That(
                order.OrderDates.FilledDate,
                Is.EqualTo(new DateTime(2019, 05, 09, 06, 00, 00, DateTimeKind.Utc)));
            Assert.That(
                order.OrderDates.StatusChangedDate,
                Is.EqualTo(new DateTime(2019, 05, 09, 07, 00, 00, DateTimeKind.Utc)));

            Assert.That(order.Trader.Id, Is.EqualTo("trader id"));
            Assert.That(order.Trader.Name, Is.EqualTo("trader name"));

            Assert.That(order.Market.Id, Is.EqualTo(9));
            Assert.That(order.Market.MarketId, Is.EqualTo("market id"));
            Assert.That(order.Market.MarketName, Is.EqualTo("market name"));

            var instrument = order.FinancialInstrument;
            Assert.That(instrument.Id, Is.EqualTo(14));
            Assert.That(instrument.ClientIdentifier, Is.EqualTo("client identifier"));
            Assert.That(instrument.Sedol, Is.EqualTo("sedol value"));
            Assert.That(instrument.Isin, Is.EqualTo("isin value"));
            Assert.That(instrument.Figi, Is.EqualTo("figi value"));
            Assert.That(instrument.Cusip, Is.EqualTo("cusip value"));
            Assert.That(instrument.Lei, Is.EqualTo("lei value"));
            Assert.That(instrument.ExchangeSymbol, Is.EqualTo("exchange symbol"));
            Assert.That(instrument.BloombergTicker, Is.EqualTo("bloomber ticker"));
            Assert.That(instrument.SecurityName, Is.EqualTo("security name"));
            Assert.That(instrument.Cfi, Is.EqualTo("cfi value"));
            Assert.That(instrument.IssuerIdentifier, Is.EqualTo("issuer identifier"));
            Assert.That(instrument.ReddeerId, Is.EqualTo("reddeer id"));

            var orderAllocations = order.OrderAllocations;

            Assert.That(orderAllocations, Is.Not.Null);
            Assert.That(orderAllocations.Count, Is.EqualTo(expectedOrderAllocations.Count));

            for (var i = 0; i < expectedOrderAllocations.Count; i++)
            {
                var actual = orderAllocations[i];
                var expected = expectedOrderAllocations[i];

                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.OrderId, Is.EqualTo(expected.OrderId));
                Assert.That(actual.Fund, Is.EqualTo(expected.Fund));
                Assert.That(actual.Strategy, Is.EqualTo(expected.Strategy));
                Assert.That(actual.ClientAccountId, Is.EqualTo(expected.ClientAccountId));
                Assert.That(actual.OrderFilledVolume, Is.EqualTo(expected.OrderFilledVolume));
                Assert.That(actual.Autoscheduled, Is.EqualTo(expected.AutoScheduled));
                Assert.That(actual.CreatedDate, Is.EqualTo(expected.CreatedDate));
            }

            var expectedBroker = this._dbContext.DbBrokers.Single();

            Assert.That(order.Broker, Is.Not.Null);
            Assert.That(order.Broker.Id, Is.EqualTo(expectedBroker.Id));
            Assert.That(order.Broker.Name, Is.EqualTo(expectedBroker.Name));
            Assert.That(order.Broker.ExternalId, Is.EqualTo(expectedBroker.ExternalId));
            Assert.That(order.Broker.CreatedOn, Is.EqualTo(expectedBroker.CreatedOn));
        }

        [Test]
        public async Task CanRequest_Orders_ByDirections()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 4,
                        PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc),
                        Direction = (int)OrderDirections.COVER
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5,
                        PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc),
                        Direction = (int)OrderDirections.SELL
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found
                        Id = 6,
                        PlacedDate = new DateTime(2019, 05, 12, 07, 50, 05, DateTimeKind.Utc),
                        Direction = (int)OrderDirections.SHORT
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentDirections(new List<OrderDirection> { OrderDirection.Cover, OrderDirection.Sell }).Node
                .FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(2));
            Assert.That(orders[0].Id, Is.EqualTo(5));
            Assert.That(orders[1].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task CanRequest_Orders_ByExcludeTraderIds()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found
                        Id = 3, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc), TraderId = "bob"
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 4, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 04, DateTimeKind.Utc), TraderId = "sam"
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc), TraderId = "ben"
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found
                        Id = 6, PlacedDate = new DateTime(2019, 05, 12, 07, 50, 05, DateTimeKind.Utc), TraderId = "vic"
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentExcludeTraderIds(new HashSet<string> { "vic", "bob", "bob" }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(2));
            Assert.That(orders[0].Id, Is.EqualTo(5));
            Assert.That(orders[1].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task CanRequest_Orders_ById_OrderedByPlacedDateDesc()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order { Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order { Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found
                        Id = 6
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentIds(new List<int> { 4, 5 }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(2));
            Assert.That(orders[0].Id, Is.EqualTo(5));
            Assert.That(orders[1].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task CanRequest_Orders_ByReddeerId()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc), SecurityId = 9
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc), SecurityId = 10
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 6, PlacedDate = new DateTime(2019, 05, 12, 07, 50, 05, DateTimeKind.Utc), SecurityId = 10
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found
                        Id = 7, PlacedDate = new DateTime(2019, 05, 12, 06, 50, 05, DateTimeKind.Utc), SecurityId = 11
                    });

            this._dbContext.DbDFinancialInstruments.Add(new FinancialInstrument { Id = 9, ReddeerId = "abc" });
            this._dbContext.DbDFinancialInstruments.Add(new FinancialInstrument { Id = 10, ReddeerId = "xyz" });
            this._dbContext.DbDFinancialInstruments.Add(new FinancialInstrument { Id = 11, ReddeerId = "qwe" });
            this._dbContext.DbDFinancialInstruments.Add(new FinancialInstrument { Id = 12, ReddeerId = "iop" });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentReddeerIds(new List<string> { "abc", "xyz" }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(3));
            Assert.That(orders[0].Id, Is.EqualTo(6));
            Assert.That(orders[1].Id, Is.EqualTo(5));
            Assert.That(orders[2].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task CanRequest_Orders_ByTraderId()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc), TraderId = "bob"
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found
                        Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc), TraderId = "sam"
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 6, PlacedDate = new DateTime(2019, 05, 12, 07, 50, 05, DateTimeKind.Utc), TraderId = "vic"
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentTraderIds(new HashSet<string> { "vic", "bob", "bob" }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(2));
            Assert.That(orders[0].Id, Is.EqualTo(6));
            Assert.That(orders[1].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task CanRequest_Orders_ByTypes()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 4,
                        PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc),
                        OrderType = (int)OrderTypes.LIMIT
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 5,
                        PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc),
                        OrderType = (int)OrderTypes.NONE
                    });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found
                        Id = 6,
                        PlacedDate = new DateTime(2019, 05, 12, 07, 50, 05, DateTimeKind.Utc),
                        OrderType = (int)OrderTypes.MARKET
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentTypes(new List<OrderType> { OrderType.Limit, OrderType.None }).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(2));
            Assert.That(orders[0].Id, Is.EqualTo(5));
            Assert.That(orders[1].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task CanRequest_Orders_WithDateRange()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order { Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order { Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order { Id = 6, PlacedDate = new DateTime(2019, 05, 12, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order { Id = 7, PlacedDate = new DateTime(2019, 05, 13, 07, 50, 05, DateTimeKind.Utc) });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentPlacedDateFrom(new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc))
                .ArgumentPlacedDateTo(new DateTime(2019, 05, 12, 07, 50, 05, DateTimeKind.Utc)).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(2));
            Assert.That(orders[0].Id, Is.EqualTo(6));
            Assert.That(orders[1].Id, Is.EqualTo(5));
        }

        [Test]
        public async Task CanRequest_Orders_WithTop()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order { Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order { Id = 5, PlacedDate = new DateTime(2019, 05, 11, 07, 50, 05, DateTimeKind.Utc) });
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        // not to be found
                        Id = 6, PlacedDate = new DateTime(2019, 05, 09, 07, 50, 05, DateTimeKind.Utc)
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentTake(2).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(2));
            Assert.That(orders[0].Id, Is.EqualTo(5));
            Assert.That(orders[1].Id, Is.EqualTo(4));
        }

        [Test]
        public async Task NothingReturnsFrom_Orders_WithEmptyTraders()
        {
            // arrange
            this._dbContext.DbOrders.Add(
                new Order
                    {
                        Id = 4, PlacedDate = new DateTime(2019, 05, 10, 07, 50, 05, DateTimeKind.Utc), TraderId = "bob"
                    });

            await this._dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query.Filter.ArgumentTraderIds(new HashSet<string>()).Node.FieldId();

            // act
            var orders = await this._apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(0));
        }
    }
}