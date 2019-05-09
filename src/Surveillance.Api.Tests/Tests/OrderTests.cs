using NUnit.Framework;
using Surveillance.Api.Client.Enums;
using Surveillance.Api.Client.Nodes;
using Surveillance.Api.Client.Queries;
using Surveillance.Api.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Tests.Tests
{
    public class OrderTests : BaseTest
    {
        [Test]
        public async Task CanRequest_Order_AllFields()
        {
            // arrange
            _dbContext.DbOrders.Add(new Order
            {
                Id = 5,
                ClientOrderId = "order id",
                OrderVersion = "order version",
                OrderVersionLinkId = "order version link id",
                OrderGroupId = "order group id",
                OrderType = (int)Domain.Core.Trading.Orders.OrderTypes.LIMIT,
                Direction = (int)Domain.Core.Trading.Orders.OrderDirections.SHORT,
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

                SecurityId = 14
            });
            _dbContext.DbMarkets.Add(new Market
            {
                Id = 9,
                MarketId = "market id",
                MarketName = "market name"
            });
            _dbContext.DbDFinancialInstruments.Add(new FinancialInstrument
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
            await _dbContext.SaveChangesAsync();

            var query = new OrderQuery();
            query
                .OrderNode
                    .FieldId()
                    .FieldClientOrderId()
                    .FieldOrderVersion()
                    .FieldOrderVersionLinkId()
                    .FieldOrderGroupId()
                    .FieldOrderType()
                    .FieldDirection()
                    .FieldCurrency()
                    .FieldSettlementCurrency()
                    .FieldCleanDirty()
                    .FieldAccumulatedInterest()
                    .FieldLimitPrice()
                    .FieldAverageFillPrice()
                    .FieldOrderedVolume()
                    .FieldFilledVolume()
                    .FieldClearingAgent()
                    .FieldDealingInstructions()
                    .FieldOptionStrikePrice()
                    .FieldOptionExpirationDate()
                    .FieldOptionEuropeanAmerican()
                    .FieldOrderDates()
                        .FieldPlaced()
                        .FieldBooked()
                        .FieldAmended()
                        .FieldRejected()
                        .FieldCancelled()
                        .FieldFilled()
                        .FieldStatusChanged()
                        .Parent<OrderNode>()
                    .FieldTrader()
                        .FieldId()
                        .FieldName()
                        .Parent<OrderNode>()
                    .FieldMarket()
                        .FieldId()
                        .FieldMarketId()
                        .FieldMarketName()
                        .Parent<OrderNode>()
                    .FieldFinancialInstrument()
                        .FieldId()
                        .FieldClientIdentifier()
                        .FieldSedol()
                        .FieldIsin()
                        .FieldFigi()
                        .FieldCusip()
                        .FieldLei()
                        .FieldExchangeSymbol()
                        .FieldBloombergTicker()
                        .FieldSecurityName()
                        .FieldCfi()
                        .FieldIssuerIdentifier()
                        .FieldReddeerId();
        
            // act  
            var orders = await _apiClient.QueryAsync(query, CancellationToken.None);

            // assert
            Assert.That(orders, Has.Count.EqualTo(1));

            var order = orders[0];
            Assert.That(order.Id, Is.EqualTo(5));
            Assert.That(order.ClientOrderId, Is.EqualTo("order id"));
            Assert.That(order.OrderVersion, Is.EqualTo("order version"));
            Assert.That(order.OrderVersionLinkId, Is.EqualTo("order version link id"));
            Assert.That(order.OrderGroupId, Is.EqualTo("order group id"));
            Assert.That(order.OrderType, Is.EqualTo(OrderType.LIMIT));
            Assert.That(order.Direction, Is.EqualTo(OrderDirection.SHORT));
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
            Assert.That(order.OptionExpirationDate, Is.EqualTo(new DateTime(2019, 05, 09, 14, 42, 11, DateTimeKind.Utc)));
            Assert.That(order.OptionEuropeanAmerican, Is.EqualTo("european american"));

            Assert.That(order.OrderDates.Placed, Is.EqualTo(new DateTime(2019, 05, 09, 01, 00, 00, DateTimeKind.Utc)));
            Assert.That(order.OrderDates.Booked, Is.EqualTo(new DateTime(2019, 05, 09, 02, 00, 00, DateTimeKind.Utc)));
            Assert.That(order.OrderDates.Amended, Is.EqualTo(new DateTime(2019, 05, 09, 03, 00, 00, DateTimeKind.Utc)));
            Assert.That(order.OrderDates.Rejected, Is.EqualTo(new DateTime(2019, 05, 09, 04, 00, 00, DateTimeKind.Utc)));
            Assert.That(order.OrderDates.Cancelled, Is.EqualTo(new DateTime(2019, 05, 09, 05, 00, 00, DateTimeKind.Utc)));
            Assert.That(order.OrderDates.Filled, Is.EqualTo(new DateTime(2019, 05, 09, 06, 00, 00, DateTimeKind.Utc)));
            Assert.That(order.OrderDates.StatusChanged, Is.EqualTo(new DateTime(2019, 05, 09, 07, 00, 00, DateTimeKind.Utc)));

            Assert.That(order.Trader.Id, Is.EqualTo("trader id"));
            Assert.That(order.Trader.Name, Is.EqualTo("trader name"));

            Assert.That(order.Market, Has.Count.EqualTo(1));
            Assert.That(order.Market[0].Id, Is.EqualTo(9));
            Assert.That(order.Market[0].MarketId, Is.EqualTo("market id"));
            Assert.That(order.Market[0].MarketName, Is.EqualTo("market name"));

            Assert.That(order.FinancialInstrument, Has.Count.EqualTo(1));
            var instrument = order.FinancialInstrument[0];
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
        }
    }
}
