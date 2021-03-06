﻿namespace Surveillance.DataLayer.Aurora.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Market;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    public class OrdersRepository : IOrdersRepository
    {
        private const string GetDealerOrdersSql = @"
            SELECT
                Id as ReddeerDealerOrderId,
                OrderId as OrderId,
                ClientDealerOrderId as ClientDealerOrderId,
                DealerOrderVersion,
                DealerOrderVersionLinkId,
                DealerOrderGroupId,
                PlacedDate as PlacedDate,
                BookedDate as BookedDate,
                AmendedDate as AmendedDate,
                RejectedDate as RejectedDate,
                CancelledDate as CancelledDate,
                FilledDate as FilledDate,
                StatusChangedDate as StatusChangedDate,
                CreatedDate as CreatedDate,
                DealerId as DealerId,
                TraderName as TraderName,
                Notes as Notes,
                CounterParty as CounterParty,
                OrderType as OrderType,
                Direction as Direction,
                Currency as Currency,
                SettlementCurrency as SettlementCurrency,
                CleanDirty as CleanDirty,
                AccumulatedInterest as AccumulatedInterest,
                LimitPrice as LimitPrice,
                AverageFillPrice as AverageFillPrice,
                OrderedVolume as OrderedVolume,
                FilledVolume as FilledVolume,
                OptionStrikePrice as OptionStrikePrice,
                OptionExpirationDate as OptionExpirationDate,
                OptionEuropeanAmerican as OptionEuropeanAmerican
            FROM DealerOrders
            WHERE OrderId IN @OrderIds";

        private const string GetLiveUnautoscheduledOrders = @"
            SELECT
	            ord.Id as ReddeerOrderId,
                ord.ClientOrderId as OrderId,
                ord.SecurityId as SecurityId,
                ord.OrderVersion as OrderVersion,
                ord.OrderVersionLinkId as OrderVersionLinkId,
                ord.OrderGroupId as OrderGroupId,
                ord.PlacedDate as OrderPlacedDate,
                ord.BookedDate as OrderBookedDate,
                ord.AmendedDate as OrderAmendedDate,
                ord.RejectedDate as OrderRejectedDate,
                ord.CancelledDate as OrderCancelledDate,
                ord.FilledDate as OrderFilledDate,
                ord.CreatedDate as CreatedDate,
                ord.OrderType as OrderType,
                ord.Direction as OrderDirection,
                ord.Currency as OrderCurrency,
                ord.SettlementCurrency as OrderSettlementCurrency,
                ord.CleanDirty as CleanDirty,
                ord.AccumulatedInterest,
                ord.LimitPrice as OrderLimitPrice,
                ord.AverageFillPrice as OrderAverageFillPrice,
                ord.OrderedVolume as OrderOrderedVolume,
                ord.FilledVolume as OrderFilledVolume,
                ord.TraderId as OrderTraderId,
                ord.TraderName as OrderTraderName,
                ord.ClearingAgent as OrderClearingAgent,
                ord.DealingInstructions as OrderDealingInstructions,
                ord.OptionStrikePrice as OptionStrikePrice,
                ord.OptionExpirationDate as OptionExpirationDate,
                ord.OptionEuropeanAmerican as OptionEuropeanAmerican,
                ord.BrokerId as OrderBrokerId,
	            fi.Id AS SecurityReddeerId,
	            fi.ClientIdentifier AS SecurityClientIdentifier,
	            fi.Sedol AS SecuritySedol,
	            fi.Isin AS SecurityIsin,
	            fi.Figi AS SecurityFigi,
	            fi.Cusip AS SecurityCusip,
	            fi.ExchangeSymbol AS SecurityExchangeSymbol,
	            fi.Lei AS SecurityLei,
	            fi.BloombergTicker AS SecurityBloombergTicker,
                fi.Ric AS SecurityRic,
	            fi.SecurityName AS SecurityName,
	            fi.Cfi AS SecurityCfi,
	            fi.IssuerIdentifier AS SecurityIssuerIdentifier,
                fi.ReddeerId AS SecurityReddeerEnrichmentId,
	            fi.UnderlyingSedol AS UnderlyingSedol,
	            fi.UnderlyingIsin AS UnderlyingIsin,
	            fi.UnderlyingFigi AS UnderlyingFigi,
	            fi.UnderlyingCusip AS UnderlyingCusip,
	            fi.UnderlyingExchangeSymbol AS UnderlyingExchangeSymbol,
	            fi.UnderlyingLei AS UnderlyingLei,
	            fi.UnderlyingBloombergTicker AS UnderlyingBloombergTicker,
	            fi.UnderlyingName AS UnderlyingName,
	            fi.UnderlyingCfi AS UnderlyingCfi,
                fi.UnderlyingRic AS UnderlyingRic,
                fi.SectorCode As SectorCode,
                fi.IndustryCode As IndustryCode,
                fi.RegionCode As RegionCode,
                fi.CountryCode As CountryCode,
                mark.Id AS MarketId,
                mark.MarketId AS MarketIdentifierCode,
                mark.MarketName AS MarketName,
                broker.ExternalId as OrderBrokerReddeerId,
                broker.Name as OrderBroker,
                broker.CreatedOn as OrderBrokerCreatedOn,
                broker.Live as OrderBrokerLive
            FROM Orders as ord
            LEFT OUTER JOIN FinancialInstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN Market as mark
            on mark.Id = ord.MarketId
            LEFT OUTER JOIN Brokers as broker
            on broker.Id = ord.BrokerId
            WHERE ord.Live = 1 AND ord.Autoscheduled = 0

            UNION

            SELECT
	            ord.Id as ReddeerOrderId,
                ord.ClientOrderId as OrderId,
                ord.SecurityId as SecurityId,
                ord.OrderVersion as OrderVersion,
                ord.OrderVersionLinkId as OrderVersionLinkId,
                ord.OrderGroupId as OrderGroupId,
                ord.PlacedDate as OrderPlacedDate,
                ord.BookedDate as OrderBookedDate,
                ord.AmendedDate as OrderAmendedDate,
                ord.RejectedDate as OrderRejectedDate,
                ord.CancelledDate as OrderCancelledDate,
                ord.FilledDate as OrderFilledDate,
                ord.CreatedDate as CreatedDate,
                ord.OrderType as OrderType,
                ord.Direction as OrderDirection,
                ord.Currency as OrderCurrency,
                ord.SettlementCurrency as OrderSettlementCurrency,
                ord.CleanDirty as CleanDirty,
                ord.AccumulatedInterest,
                ord.LimitPrice as OrderLimitPrice,
                ord.AverageFillPrice as OrderAverageFillPrice,
                ord.OrderedVolume as OrderOrderedVolume,
                ord.FilledVolume as OrderFilledVolume,
                ord.TraderId as OrderTraderId,
                ord.TraderName as OrderTraderName,
                ord.ClearingAgent as OrderClearingAgent,
                ord.DealingInstructions as OrderDealingInstructions,
                ord.OptionStrikePrice as OptionStrikePrice,
                ord.OptionExpirationDate as OptionExpirationDate,
                ord.OptionEuropeanAmerican as OptionEuropeanAmerican,
                ord.BrokerId as OrderBrokerId,
	            fi.Id AS SecurityReddeerId,
	            fi.ClientIdentifier AS SecurityClientIdentifier,
	            fi.Sedol AS SecuritySedol,
	            fi.Isin AS SecurityIsin,
	            fi.Figi AS SecurityFigi,
	            fi.Cusip AS SecurityCusip,
	            fi.ExchangeSymbol AS SecurityExchangeSymbol,
	            fi.Lei AS SecurityLei,
	            fi.BloombergTicker AS SecurityBloombergTicker,
                fi.Ric AS SecurityRic,
	            fi.SecurityName AS SecurityName,
	            fi.Cfi AS SecurityCfi,
	            fi.IssuerIdentifier AS SecurityIssuerIdentifier,
                fi.ReddeerId AS SecurityReddeerEnrichmentId,
	            fi.UnderlyingSedol AS UnderlyingSedol,
	            fi.UnderlyingIsin AS UnderlyingIsin,
	            fi.UnderlyingFigi AS UnderlyingFigi,
	            fi.UnderlyingCusip AS UnderlyingCusip,
	            fi.UnderlyingExchangeSymbol AS UnderlyingExchangeSymbol,
	            fi.UnderlyingLei AS UnderlyingLei,
	            fi.UnderlyingBloombergTicker AS UnderlyingBloombergTicker,
	            fi.UnderlyingName AS UnderlyingName,
	            fi.UnderlyingCfi AS UnderlyingCfi,
                fi.UnderlyingRic AS UnderlyingRic,
                fi.SectorCode As SectorCode,
                fi.IndustryCode As IndustryCode,
                fi.RegionCode As RegionCode,
                fi.CountryCode As CountryCode,
                mark.Id AS MarketId,
                mark.MarketId AS MarketIdentifierCode,
                mark.MarketName AS MarketName,
                broker.ExternalId as OrderBrokerReddeerId,
                broker.Name as OrderBroker,
                broker.CreatedOn as OrderBrokerCreatedOn,
                broker.Live as OrderBrokerLive
            FROM OrdersAllocation as OrdAlloc
            LEFT OUTER JOIN Orders as ord
            ON OrdAlloc.OrderId = ord.ClientOrderId
            LEFT OUTER JOIN FinancialInstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN Market as mark
            on mark.Id = ord.MarketId
            LEFT OUTER JOIN Brokers as broker
            on broker.Id = ord.BrokerId
            WHERE OrdAlloc.Live = 1 AND OrdAlloc.Autoscheduled = 0;";

        private const string GetOrderSql = @"
            SELECT
	            ord.Id as ReddeerOrderId,
                ord.ClientOrderId as OrderId,
                ord.SecurityId as SecurityId,
                ord.OrderVersion as OrderVersion,
                ord.OrderVersionLinkId as OrderVersionLinkId,
                ord.OrderGroupId as OrderGroupId,
                ord.PlacedDate as OrderPlacedDate,
                ord.BookedDate as OrderBookedDate,
                ord.AmendedDate as OrderAmendedDate,
                ord.RejectedDate as OrderRejectedDate,
                ord.CancelledDate as OrderCancelledDate,
                ord.FilledDate as OrderFilledDate,
                ord.CreatedDate as CreatedDate,
                ord.OrderType as OrderType,
                ord.Direction as OrderDirection,
                ord.Currency as OrderCurrency,
                ord.SettlementCurrency as OrderSettlementCurrency,
                ord.CleanDirty as CleanDirty,
                ord.AccumulatedInterest,
                ord.LimitPrice as OrderLimitPrice,
                ord.AverageFillPrice as OrderAverageFillPrice,
                ord.OrderedVolume as OrderOrderedVolume,
                ord.FilledVolume as OrderFilledVolume,
                ord.TraderId as OrderTraderId,
                ord.TraderName as OrderTraderName,
                ord.ClearingAgent as OrderClearingAgent,
                ord.DealingInstructions as OrderDealingInstructions,
                ord.OptionStrikePrice as OptionStrikePrice,
                ord.OptionExpirationDate as OptionExpirationDate,
                ord.OptionEuropeanAmerican as OptionEuropeanAmerican,
                ord.BrokerId as OrderBrokerId,
	            fi.Id AS SecurityReddeerId,
	            fi.ClientIdentifier AS SecurityClientIdentifier,
	            fi.Sedol AS SecuritySedol,
	            fi.Isin AS SecurityIsin,
	            fi.Figi AS SecurityFigi,
	            fi.Cusip AS SecurityCusip,
	            fi.ExchangeSymbol AS SecurityExchangeSymbol,
	            fi.Lei AS SecurityLei,
	            fi.BloombergTicker AS SecurityBloombergTicker,
                fi.Ric AS SecurityRic,
	            fi.SecurityName AS SecurityName,
	            fi.Cfi AS SecurityCfi,
	            fi.IssuerIdentifier AS SecurityIssuerIdentifier,
                fi.ReddeerId AS SecurityReddeerEnrichmentId,
	            fi.UnderlyingSedol AS UnderlyingSedol,
	            fi.UnderlyingIsin AS UnderlyingIsin,
	            fi.UnderlyingFigi AS UnderlyingFigi,
	            fi.UnderlyingCusip AS UnderlyingCusip,
	            fi.UnderlyingExchangeSymbol AS UnderlyingExchangeSymbol,
	            fi.UnderlyingLei AS UnderlyingLei,
	            fi.UnderlyingBloombergTicker AS UnderlyingBloombergTicker,
	            fi.UnderlyingName AS UnderlyingName,
	            fi.UnderlyingCfi AS UnderlyingCfi,
                fi.UnderlyingRic AS UnderlyingRic,
                fi.SectorCode As SectorCode,
                fi.IndustryCode As IndustryCode,
                fi.RegionCode As RegionCode,
                fi.CountryCode As CountryCode,
                mark.Id AS MarketId,
                mark.MarketId AS MarketIdentifierCode,
                mark.MarketName AS MarketName,
                broker.ExternalId as OrderBrokerReddeerId,
                broker.Name as OrderBroker,
                broker.CreatedOn as OrderBrokerCreatedOn,
                broker.Live as OrderBrokerLive
            FROM Orders as ord
            LEFT OUTER JOIN FinancialInstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN Market as mark
            on mark.Id = ord.MarketId
            LEFT OUTER JOIN Brokers as broker
            on broker.Id = ord.BrokerId
            WHERE
            ord.PlacedDate >= @Start
            AND ord.StatusChangedDate <= @End
            AND ord.Live = 1;";

        private const string GetStaleOrders = @"
            SELECT
	            ord.Id as ReddeerOrderId,
                ord.ClientOrderId as OrderId,
                ord.SecurityId as SecurityId,
                ord.OrderVersion as OrderVersion,
                ord.OrderVersionLinkId as OrderVersionLinkId,
                ord.OrderGroupId as OrderGroupId,
                ord.PlacedDate as OrderPlacedDate,
                ord.BookedDate as OrderBookedDate,
                ord.AmendedDate as OrderAmendedDate,
                ord.RejectedDate as OrderRejectedDate,
                ord.CancelledDate as OrderCancelledDate,
                ord.FilledDate as OrderFilledDate,
                ord.CreatedDate as CreatedDate,
                ord.OrderType as OrderType,
                ord.Direction as OrderDirection,
                ord.Currency as OrderCurrency,
                ord.SettlementCurrency as OrderSettlementCurrency,
                ord.CleanDirty as CleanDirty,
                ord.AccumulatedInterest,
                ord.LimitPrice as OrderLimitPrice,
                ord.AverageFillPrice as OrderAverageFillPrice,
                ord.OrderedVolume as OrderOrderedVolume,
                ord.FilledVolume as OrderFilledVolume,
                ord.TraderId as OrderTraderId,
                ord.TraderName as OrderTraderName,
                ord.ClearingAgent as OrderClearingAgent,
                ord.DealingInstructions as OrderDealingInstructions,
                ord.OptionStrikePrice as OptionStrikePrice,
                ord.OptionExpirationDate as OptionExpirationDate,
                ord.OptionEuropeanAmerican as OptionEuropeanAmerican,
                ord.BrokerId as OrderBrokerId,
	            fi.Id AS SecurityReddeerId,
	            fi.ClientIdentifier AS SecurityClientIdentifier,
	            fi.Sedol AS SecuritySedol,
	            fi.Isin AS SecurityIsin,
	            fi.Figi AS SecurityFigi,
	            fi.Cusip AS SecurityCusip,
	            fi.ExchangeSymbol AS SecurityExchangeSymbol,
	            fi.Lei AS SecurityLei,
	            fi.BloombergTicker AS SecurityBloombergTicker,
                fi.Ric AS SecurityRic,
	            fi.SecurityName AS SecurityName,
	            fi.Cfi AS SecurityCfi,
	            fi.IssuerIdentifier AS SecurityIssuerIdentifier,
                fi.ReddeerId AS SecurityReddeerEnrichmentId,
	            fi.UnderlyingSedol AS UnderlyingSedol,
	            fi.UnderlyingIsin AS UnderlyingIsin,
	            fi.UnderlyingFigi AS UnderlyingFigi,
	            fi.UnderlyingCusip AS UnderlyingCusip,
	            fi.UnderlyingExchangeSymbol AS UnderlyingExchangeSymbol,
	            fi.UnderlyingLei AS UnderlyingLei,
	            fi.UnderlyingBloombergTicker AS UnderlyingBloombergTicker,
	            fi.UnderlyingName AS UnderlyingName,
	            fi.UnderlyingCfi AS UnderlyingCfi,
                fi.UnderlyingRic AS UnderlyingRic,
                fi.SectorCode As SectorCode,
                fi.IndustryCode As IndustryCode,
                fi.RegionCode As RegionCode,
                fi.CountryCode As CountryCode,
                mark.Id AS MarketId,
                mark.MarketId AS MarketIdentifierCode,
                mark.MarketName AS MarketName,
                broker.ExternalId as OrderBrokerReddeerId,
                broker.Name as OrderBroker,
                broker.CreatedOn as OrderBrokerCreatedOn,
                broker.Live as OrderBrokerLive
            FROM Orders as ord
            LEFT OUTER JOIN FinancialInstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN Market as mark
            on mark.Id = ord.MarketId
            LEFT OUTER JOIN Brokers as broker
            on broker.Id = ord.BrokerId
            WHERE ord.Live = 0 AND ord.CreatedDate < @StaleDate;";

        private const string InsertDealerOrderSql = @"
            INSERT INTO DealerOrders(
                OrderId,
                ClientDealerOrderId,
                DealerOrderVersion,
                DealerOrderVersionLinkId,
                DealerOrderGroupId,              
                PlacedDate,
                BookedDate,
                AmendedDate,
                RejectedDate,
                CancelledDate,
                FilledDate,
                StatusChangedDate,
                CreatedDate,
                LifeCycleStatus,
                DealerId,
                TraderName,
                Notes,
                CounterParty,
                OrderType,
                Direction,
                Currency,
                SettlementCurrency,
                CleanDirty,
                AccumulatedInterest,
                LimitPrice,
                AverageFillPrice,
                OrderedVolume,
                FilledVolume,
                OptionStrikePrice,
                OptionExpirationDate,
                OptionEuropeanAmerican)
            VALUES(
                @OrderId,
                @ClientDealerOrderId,
                @DealerOrderVersion,
                @DealerOrderVersionLinkId,
                @DealerOrderGroupId,
                @PlacedDate,
                @BookedDate,
                @AmendedDate,
                @RejectedDate,
                @CancelledDate,
                @FilledDate,
                @StatusChangedDate,
                @CreatedDate,
                @LifeCycleStatus,
                @DealerId,
                @TraderName,
                @Notes,
                @CounterParty,
                @OrderType,
                @Direction,
                @Currency,
                @SettlementCurrency,
                @CleanDirty,
                @AccumulatedInterest,
                @LimitPrice,
                @AverageFillPrice,
                @OrderedVolume,
                @FilledVolume,
                @OptionStrikePrice,
                @OptionExpirationDate,
                @OptionEuropeanAmerican)
            ON DUPLICATE KEY UPDATE
                ClientDealerOrderId = @ClientDealerOrderId,
                DealerOrderVersion = @DealerOrderVersion,
                DealerOrderVersionLinkId = @DealerOrderVersionLinkId,
                DealerOrderGroupId = @DealerOrderGroupId,
                PlacedDate = @PlacedDate,
                BookedDate = @BookedDate,
                AmendedDate = @AmendedDate,
                RejectedDate = @RejectedDate,
                CancelledDate = @CancelledDate,
                FilledDate = @FilledDate,
                StatusChangedDate = @StatusChangedDate,
                DealerId = @DealerId,
                TraderName = @TraderName,
                Notes = @Notes,
                CounterParty = @CounterParty,
                OrderType = @OrderType,
                Direction = @Direction,
                Currency = @Currency,
                SettlementCurrency = @SettlementCurrency,
                CleanDirty = @CleanDirty,
                AccumulatedInterest = @AccumulatedInterest,
                LimitPrice = @LimitPrice,
                AverageFillPrice = @AverageFillPrice,
                OrderedVolume = @OrderedVolume,
                FilledVolume = @FilledVolume,
                OptionStrikePrice = @OptionStrikePrice,
                OptionExpirationDate = @OptionExpirationDate,
                OptionEuropeanAmerican = @OptionEuropeanAmerican,
                Id = LAST_INSERT_ID(Id);
                SELECT LAST_INSERT_ID();";

        private const string InsertOrderSql = @"
            SELECT
                1
            INTO
                @match
            FROM Orders O
            WHERE
                MarketId=@MarketId AND
                SecurityId=@SecurityReddeerId AND
                ClientOrderId=@OrderId AND
                OrderVersion=@OrderVersion AND
                OrderVersionLinkId=@OrderVersionLinkId AND
                OrderGroupId=@OrderGroupId AND
                ((@OrderPlacedDate IS NULL AND PlacedDate IS NULL) OR (PlacedDate = @OrderPlacedDate)) AND
                ((@OrderBookedDate IS NULL AND BookedDate IS NULL) OR (BookedDate = @OrderBookedDate)) AND
                ((@OrderAmendedDate IS NULL AND AmendedDate IS NULL) OR (AmendedDate = @OrderAmendedDate)) AND
                ((@OrderRejectedDate IS NULL AND RejectedDate IS NULL) OR (RejectedDate = @OrderRejectedDate)) AND
                ((@OrderCancelledDate IS NULL AND CancelledDate IS NULL) OR (CancelledDate = @OrderCancelledDate)) AND
                ((@OrderFilledDate IS NULL AND FilledDate IS NULL) OR (FilledDate = @OrderFilledDate)) AND
                ((@OrderStatusChangedDate IS NULL AND StatusChangedDate IS NULL) OR (StatusChangedDate = @OrderStatusChangedDate)) AND
                OrderType=@OrderType AND
                Direction=@OrderDirection AND
                Currency=@OrderCurrency AND
                SettlementCurrency=@OrderSettlementCurrency AND
                CleanDirty=@CleanDirty AND
                ((@AccumulatedInterest IS NULL AND AccumulatedInterest IS NULL) OR (AccumulatedInterest=@AccumulatedInterest)) AND
                LimitPrice=@OrderLimitPrice AND
                AverageFillPrice=@OrderAverageFillPrice AND
                OrderedVolume=@OrderOrderedVolume AND
                FilledVolume=@OrderFilledVolume AND
                TraderId=@OrderTraderId AND
                TraderName = @OrderTraderName AND
                ((@OrderBrokerId IS NULL AND BrokerId IS NULL) OR (BrokerId = @OrderBrokerId)) AND
                ClearingAgent=@OrderClearingAgent AND
                DealingInstructions=@OrderDealingInstructions AND
                OptionStrikePrice=@OptionStrikePrice AND
                ((@OptionExpirationDate IS NULL AND OptionExpirationDate IS NULL) OR (OptionExpirationDate=@OptionExpirationDate)) AND
                OptionEuropeanAmerican=@OptionEuropeanAmerican
            LIMIT 1;

            INSERT INTO Orders(
                MarketId,
                SecurityId,
                ClientOrderId,
                OrderVersion,
                OrderVersionLinkId,
                OrderGroupId,
                PlacedDate,
                BookedDate,
                AmendedDate,
                RejectedDate,
                CancelledDate,
                FilledDate,
                StatusChangedDate,
                CreatedDate,
                LifeCycleStatus,
                OrderType,
                Direction,
                Currency,
                SettlementCurrency,
                CleanDirty,
                AccumulatedInterest,
                LimitPrice,
                AverageFillPrice,
                OrderedVolume,
                FilledVolume,
                TraderId,
                TraderName,
                BrokerId,
                ClearingAgent,
                DealingInstructions,
                OptionStrikePrice,
                OptionExpirationDate,
                OptionEuropeanAmerican)
            VALUES(
                @MarketId,
                @SecurityReddeerId,
                @OrderId,
                @OrderVersion,
                @OrderVersionLinkId,
                @OrderGroupId,
                @OrderPlacedDate,
                @OrderBookedDate,
                @OrderAmendedDate,
                @OrderRejectedDate,
                @OrderCancelledDate,
                @OrderFilledDate,
                @OrderStatusChangedDate,
                @CreatedDate,
                @LifeCycleStatus,
                @OrderType,
                @OrderDirection,
                @OrderCurrency,
                @OrderSettlementCurrency,
                @CleanDirty,
                @AccumulatedInterest,
                @OrderLimitPrice,
                @OrderAverageFillPrice,
                @OrderOrderedVolume,
                @OrderFilledVolume,
                @OrderTraderId,
                @OrderTraderName,
                @OrderBrokerId,
                @OrderClearingAgent,
                @OrderDealingInstructions,
                @OptionStrikePrice,
                @OptionExpirationDate,
                @OptionEuropeanAmerican)
            ON DUPLICATE KEY UPDATE
                MarketId=@MarketId,
                SecurityId=@SecurityReddeerId,
                ClientOrderId=@OrderId,
                OrderVersion=@OrderVersion,
                OrderVersionLinkId=@OrderVersionLinkId,
                OrderGroupId=@OrderGroupId,
                PlacedDate=@OrderPlacedDate,
                BookedDate=@OrderBookedDate,
                AmendedDate=@OrderAmendedDate,
                RejectedDate=@OrderRejectedDate,
                CancelledDate=@OrderCancelledDate,
                FilledDate=@OrderFilledDate,
                StatusChangedDate=@OrderStatusChangedDate,
                OrderType=@OrderType,
                Direction=@OrderDirection,
                Currency=@OrderCurrency,
                SettlementCurrency=@OrderSettlementCurrency,
                CleanDirty=@CleanDirty,
                AccumulatedInterest=@AccumulatedInterest,
                LimitPrice=@OrderLimitPrice,
                AverageFillPrice=@OrderAverageFillPrice,
                OrderedVolume=@OrderOrderedVolume,
                FilledVolume=@OrderFilledVolume,
                TraderId=@OrderTraderId,
                TraderName = @OrderTraderName,
                BrokerId = @OrderBrokerId,
                ClearingAgent=@OrderClearingAgent,
                DealingInstructions=@OrderDealingInstructions,
                OptionStrikePrice=@OptionStrikePrice,
                OptionExpirationDate=@OptionExpirationDate,
                OptionEuropeanAmerican=@OptionEuropeanAmerican,
                Live = IF(
                    @match = 1,
                    Live,
                    0
                ),
                Autoscheduled = IF(
                    @match = 1,
                    Autoscheduled,
                    0
                ),
                CreatedDate = UTC_TIMESTAMP(),
                Id = LAST_INSERT_ID(Id);

                SELECT LAST_INSERT_ID();";

        private const string SetOrdersToLivened = @"
            UPDATE Orders AS ord
            LEFT OUTER JOIN OrdersAllocation AS oa
            ON ord.ClientOrderId = oa.OrderId
            SET ord.Live = 1
            WHERE oa.OrderId IS NOT NULL;

            UPDATE OrdersAllocation AS oa
            LEFT OUTER JOIN Orders AS ord
            ON oa.OrderId = ord.ClientOrderId
            SET oa.Live = 1
            WHERE ord.Id IS NOT NULL;";

        private const string SetOrdersToScheduled = @"
            UPDATE Orders
            SET Autoscheduled = 1
            WHERE ClientOrderId = @OrderId;

            UPDATE OrdersAllocation
            SET Autoscheduled = 1
            WHERE OrderId = @OrderId;";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly object _lock = new object();

        private readonly ILogger _logger;

        private readonly IReddeerMarketRepository _marketRepository;

        private readonly IOrderBrokerRepository _orderBrokerRepository;

        public OrdersRepository(
            IConnectionStringFactory connectionStringFactory,
            IReddeerMarketRepository marketRepository,
            IOrderBrokerRepository orderBrokerRepository,
            ILogger<OrdersRepository> logger)
        {
            this._dbConnectionFactory = connectionStringFactory
                                        ?? throw new ArgumentNullException(nameof(connectionStringFactory));

            this._marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            this._orderBrokerRepository =
                orderBrokerRepository ?? throw new ArgumentNullException(nameof(orderBrokerRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(Order entity)
        {
            if (entity == null)
            {
                this._logger.LogError("ReddeerTradeRepository Create passed a null order entity. Returning.");
                return;
            }

            lock (this._lock)
            {
                var dbConnection = this._dbConnectionFactory.BuildConn();

                try
                {
                    dbConnection.Open();

                    if (entity.OrderBroker != null && !string.IsNullOrWhiteSpace(entity.OrderBroker?.Name)
                                                   && string.IsNullOrWhiteSpace(entity.OrderBroker?.Id))
                    {
                        var broker = this._orderBrokerRepository.InsertOrUpdateBrokerAsync(entity.OrderBroker)
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

                        entity.OrderBroker.Id = broker;
                    }

                    var dto = new OrderDto(entity);

                    this._logger.LogInformation($"ReddeerTradeRepository beginning save for order {entity.OrderId}");

                    if (string.IsNullOrWhiteSpace(dto.SecurityReddeerId) || string.IsNullOrWhiteSpace(dto.MarketId))
                    {
                        var marketDataPair =
                            new MarketDataPair { Exchange = entity.Market, Security = entity.Instrument };
                        
                        var marketSecurityId = this._marketRepository.CreateAndOrGetSecurityId(marketDataPair)
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();
                        
                        dto.SecurityReddeerId = marketSecurityId.SecurityId;
                        dto.MarketId = marketSecurityId.MarketId;
                    }

                    this._logger.LogInformation("ReddeerTradeRepository Create about to insert a new order");
                    var orderId = dbConnection.ExecuteScalarAsync<int?>(InsertOrderSql, dto)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                    
                    entity.ReddeerOrderId = orderId;
                    this._logger.LogInformation($"ReddeerTradeRepository Create completed for the new order {orderId}");

                    if (entity.ReddeerOrderId == null)
                        this._logger.LogError($"Attempted to save order {entity.OrderId} from client but did not get a reddeer order id (primary key) value.");

                    if (entity.DealerOrders == null || !entity.DealerOrders.Any())
                    {
                        this._logger.LogInformation(
                            $"ReddeerTradeRepository Create saved an order with id {entity.ReddeerOrderId} and it had no trades so returning.");
                        return;
                    }

                    foreach (var trade in entity.DealerOrders)
                    {
                        if (trade == null) continue;

                        this._logger.LogInformation(
                            $"ReddeerTradeRepository Create about to insert a new trade entry for order {entity.ReddeerOrderId}");
                        var tradeDto = new DealerOrdersDto(trade, entity.ReddeerOrderId);
                        using (var conn = dbConnection.ExecuteScalarAsync<string>(InsertDealerOrderSql, tradeDto))
                        {
                            var tradeIdTask = conn;
                            tradeIdTask.Wait();
                            var tradeId = tradeIdTask.Result;
                            tradeDto.ReddeerDealerOrderId = tradeId;
                            this._logger.LogInformation(
                                $"ReddeerTradeRepository Create inserted a new trade entry for order {entity.ReddeerOrderId} and it had an id of {tradeId}");
                        }
                    }

                    this._logger.LogInformation($"ReddeerTradeRepository finished save for order {entity.OrderId}");
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"ReddeerTradeRepository Create Method For {entity.Instrument?.Name}");
                }
                finally
                {
                    dbConnection.Close();
                    dbConnection.Dispose();
                }
            }
        }

        public async Task<IReadOnlyCollection<Order>> Get(
            DateTime start,
            DateTime end,
            ISystemProcessOperationContext opCtx)
        {
            this._logger.LogInformation(
                $"ReddeerTradeRepository asked to get orders from {start} to {end} for system process operation {opCtx?.Id}");

            start = start.Date;
            end = end.Date.AddDays(1).AddMilliseconds(-1);

            if (end < start)
            {
                this._logger.LogError(
                    $"ReddeerTradeRepository asked to get orders from {start} to {end} for system process operation {opCtx?.Id} but the end date predated the start date!");

                return new Order[0];
            }

            var dbConnection = this._dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                // GET ORDERS
                var orders = new List<Order>();
                var query = new GetQuery { Start = start, End = end };

                this._logger.LogInformation(
                    $"ReddeerTradeRepository asked to get orders from {start} to {end} for system process operation {opCtx?.Id}");
                using (var conn = dbConnection.QueryAsync<OrderDto>(GetOrderSql, query))
                {
                    var rawResult = await conn;

                    this._logger.LogInformation(
                        $"ReddeerTradeRepository has gotten orders {rawResult?.Count() ?? 0} from {start} to {end} for system process operation {opCtx?.Id}");

                    orders = rawResult?.Select(this.Project).ToList();
                }

                // GET TRADES
                var orderIds = orders.Select(ord => ord.ReddeerOrderId?.ToString()).Where(x => x != null).ToList();
                var tradeIds = new List<string>();
                var tradeDtos = new List<DealerOrdersDto>();

                if (orderIds != null && orderIds.Any())
                {
                    this._logger.LogInformation(
                        $"ReddeerTradeRepository getting trades from {start} to {end} for system process operation {opCtx?.Id}");
                    using (var conn = dbConnection.QueryAsync<DealerOrdersDto>(
                        GetDealerOrdersSql,
                        new { OrderIds = orderIds }))
                    {
                        tradeDtos = (await conn).ToList();
                        tradeIds = tradeDtos.Select(tfo => tfo.ReddeerDealerOrderId?.ToString())
                            .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        this._logger.LogInformation(
                            $"ReddeerTradeRepository completed getting trades from {start} to {end} for system process operation {opCtx?.Id}");
                    }
                }

                // JOIN trades to orders
                var groups = tradeDtos.GroupBy(tfo => tfo.OrderId);
                foreach (var grp in groups)
                {
                    var order = orders.FirstOrDefault(ord => ord.ReddeerOrderId == grp.Key);
                    if (order == null) continue;

                    order.DealerOrders = grp.Select(tr => this.Project(tr, order.Instrument)).ToList();
                    foreach (var trad in order.DealerOrders)
                        trad.ParentOrder = order;
                }

                this._logger.LogInformation(
                    $"ReddeerTradeRepository returning from get orders from {start} to {end} for system process operation {opCtx?.Id} with {orders?.Count} orders");

                return orders;
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"ReddeerTradeRepository Get Method For {start.ToShortDateString()} to {end.ToShortDateString()}");
                opCtx?.EventError(e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new Order[0];
        }

        public async Task LivenCompletedOrderSets()
        {
            this._logger.LogInformation("OrdersRepository asked to set order livening");

            try
            {
                using (var open = this._dbConnectionFactory.BuildConn())
                using (var conn = open.ExecuteAsync(SetOrdersToLivened))
                {
                    await conn;

                    this._logger.LogInformation("OrdersRepository completed setting order livening");
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(e, $"OrdersRepository liven completed order sets exception {e.Message}");
            }
        }

        public async Task<IReadOnlyCollection<Order>> LiveUnscheduledOrders()
        {
            this._logger.LogInformation("OrdersRepository asked to get live unscheduled order ids");

            try
            {
                using (var open = this._dbConnectionFactory.BuildConn())
                using (var conn = open.QueryAsync<OrderDto>(GetLiveUnautoscheduledOrders))
                {
                    var response = await conn;
                    var projectedResponse = response.Select(this.Project).ToList();

                    this._logger.LogInformation("OrdersRepository completed getting live unscheduled order ids");

                    return projectedResponse;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(e, $"OrdersRepository LiveUnscheduledOrderIds encountered an exception {e.Message}");
            }

            return new List<Order>();
        }

        public async Task SetOrdersScheduled(IReadOnlyCollection<Order> orders)
        {
            if (orders == null || !orders.Any()) return;

            this._logger.LogInformation("OrdersRepository asked to set orders scheduled");

            try
            {
                var dtos = orders.Select(i => new OrderDto(i)).ToList();

                using (var open = this._dbConnectionFactory.BuildConn())
                using (var conn = open.ExecuteAsync(SetOrdersToScheduled, dtos))
                {
                    await conn;

                    this._logger.LogInformation("OrdersRepository completed setting orders scheduled");
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(e, $"OrdersRepository set orders as scheduled encountered an exception {e.Message}");
            }
        }

        /// <summary>
        ///     Does not eagerly fetch related domain entities
        /// </summary>
        public async Task<IReadOnlyCollection<Order>> StaleOrders(DateTime stalenessDate)
        {
            this._logger.LogInformation("OrdersRepository asked to fetch stale orders");

            try
            {
                using (var open = this._dbConnectionFactory.BuildConn())
                using (var conn = open.QueryAsync<OrderDto>(GetStaleOrders, new { StaleDate = stalenessDate }))
                {
                    var queryResult = await conn;

                    var staleOrders = queryResult.Select(this.Project).ToList();
                    this._logger.LogInformation("OrdersRepository completed fetching stale orders");

                    return staleOrders;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(e, $"OrdersRepository fetch stale orders exception {e.Message}");
            }

            return new List<Order>();
        }

        private Order Project(OrderDto dto)
        {
            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Equity,
                new InstrumentIdentifiers(
                    dto.SecurityId,
                    dto.SecurityReddeerId,
                    dto.SecurityReddeerEnrichmentId,
                    dto.SecurityClientIdentifier,
                    dto.SecuritySedol,
                    dto.SecurityIsin,
                    dto.SecurityFigi,
                    dto.SecurityCusip,
                    dto.SecurityExchangeSymbol,
                    dto.SecurityLei,
                    dto.SecurityBloombergTicker,
                    dto.SecurityRic,
                    dto.UnderlyingSecuritySedol,
                    dto.UnderlyingSecurityIsin,
                    dto.UnderlyingSecurityFigi,
                    dto.UnderlyingSecurityCusip,
                    dto.UnderlyingSecurityLei,
                    dto.UnderlyingSecurityExchangeSymbol,
                    dto.UnderlyingSecurityBloombergTicker,
                    dto.UnderlyingClientIdentifier,
                    dto.UnderlyingSecurityRic),
                dto.SecurityName,
                dto.SecurityCfi,
                dto.OrderCurrency,
                dto.SecurityIssuerIdentifier,
                dto.UnderlyingSecurityName,
                dto.UnderlyingSecurityCfi,
                dto.UnderlyingSecurityIssuerIdentifier,
                dto.SectorCode,
                dto.IndustryCode,
                dto.RegionCode,
                dto.CountryCode);

            Enum.TryParse(dto.MarketType?.ToString() ?? string.Empty, out MarketTypes result);
            var orderTypeResult = (OrderTypes)dto.OrderType.GetValueOrDefault(0);
            var orderDirectionResult = (OrderDirections)dto.OrderDirection.GetValueOrDefault(0);
            var orderCurrency = new Currency(dto.OrderCurrency);
            var limitPrice = new Money(dto.OrderLimitPrice, dto.OrderCurrency);
            var averagePrice = new Money(dto.OrderAverageFillPrice, dto.OrderCurrency);

            var settlementCurrency = !string.IsNullOrWhiteSpace(dto.OrderSettlementCurrency)
                                         ? (Currency?)new Currency(dto.OrderSettlementCurrency)
                                         : null;

            var orderCleanDirty = (OrderCleanDirty)dto.CleanDirty.GetValueOrDefault(0);
            var orderAccumulatedInterest = dto.AccumulatedInterest;

            var market = new Market(dto.MarketId, dto.MarketIdentifierCode, dto.MarketName, result);
            var dealerOrders = dto.DealerOrders?.Select(tr => this.Project(tr, financialInstrument)).ToList()
                               ?? new List<DealerOrder>();

            var optionEuropeanAmerican = (OptionEuropeanAmerican)dto.OptionEuropeanAmerican.GetValueOrDefault(0);
            var optionStrikePrice = dto.OptionStrikePrice == null
                                        ? (Money?)null
                                        : new Money(dto.OptionStrikePrice, dto.OrderCurrency);

            var order = new Order(
                financialInstrument,
                market,
                dto.ReddeerOrderId,
                dto.OrderId,
                dto.CreatedDate,
                dto.OrderVersion,
                dto.OrderVersionLinkId,
                dto.OrderGroupId,
                dto.OrderPlacedDate,
                dto.OrderBookedDate,
                dto.OrderAmendedDate,
                dto.OrderRejectedDate,
                dto.OrderCancelledDate,
                dto.OrderFilledDate,
                orderTypeResult,
                orderDirectionResult,
                orderCurrency,
                settlementCurrency,
                orderCleanDirty,
                orderAccumulatedInterest,
                limitPrice,
                averagePrice,
                dto.OrderOrderedVolume,
                dto.OrderFilledVolume,
                dto.OrderTraderId,
                dto.OrderTraderName,
                dto.OrderClearingAgent,
                dto.OrderDealingInstructions,
                new OrderBroker(
                    dto.OrderBrokerId,
                    dto.OrderBrokerReddeerId,
                    dto.OrderBroker,
                    dto.OrderBrokerCreatedOn,
                    dto.OrderBrokerLive),
                optionStrikePrice,
                dto.OptionExpirationDate,
                optionEuropeanAmerican,
                dealerOrders);

            foreach (var trad in dealerOrders)
                trad.ParentOrder = order;

            return order;
        }

        private DealerOrder Project(DealerOrdersDto dto, FinancialInstrument fi)
        {
            var orderType = (OrderTypes)dto.OrderType.GetValueOrDefault(0);
            var orderDirection = (OrderDirections)dto.Direction.GetValueOrDefault(0);
            var orderCleanDirty = (OrderCleanDirty)dto.CleanDirty.GetValueOrDefault(0);
            var optionEuropeanAmerican = (OptionEuropeanAmerican)dto.OptionEuropeanAmerican.GetValueOrDefault();
            var orderCurrency = new Currency(dto.Currency);
            var settlementCurrency = new Currency(dto.SettlementCurrency);

            var orderLimit = dto.LimitPrice != null ? new Money(dto.LimitPrice, dto.Currency) : (Money?)null;
            var orderAveragePrice = dto.AverageFillPrice != null
                                        ? new Money(dto.AverageFillPrice, dto.Currency)
                                        : (Money?)null;

            var dealerOrder = new DealerOrder(
                fi,
                dto.ReddeerDealerOrderId,
                dto.ClientDealerOrderId,
                dto.PlacedDate,
                dto.BookedDate,
                dto.AmendedDate,
                dto.RejectedDate,
                dto.CancelledDate,
                dto.FilledDate,
                dto.CreatedDate,
                dto.DealerId,
                dto.TraderName,
                dto.Notes,
                dto.CounterParty,
                orderType,
                orderDirection,
                orderCurrency,
                settlementCurrency,
                orderCleanDirty,
                dto.AccumulatedInterest,
                dto.DealerOrderVersion,
                dto.DealerOrderVersionLinkId,
                dto.DealerOrderGroupId,
                orderLimit,
                orderAveragePrice,
                dto.OrderedVolume,
                dto.FilledVolume,
                dto.OptionStrikePrice,
                dto.OptionExpirationDate,
                optionEuropeanAmerican);

            return dealerOrder;
        }

        public class DealerOrdersDto
        {
            public DealerOrdersDto()
            {
            }

            public DealerOrdersDto(DealerOrder dealerOrder, int? orderId)
            {
                this.OrderId = orderId;

                if (dealerOrder == null) return;

                this.ReddeerDealerOrderId = dealerOrder.ReddeerDealerOrderId;
                this.ClientDealerOrderId = dealerOrder.DealerOrderId;

                this.DealerOrderVersion = dealerOrder.DealerOrderVersion;
                this.DealerOrderVersionLinkId = dealerOrder.DealerOrderVersionLinkId;
                this.DealerOrderGroupId = dealerOrder.DealerOrderGroupId;

                this.PlacedDate = dealerOrder.PlacedDate;
                this.BookedDate = dealerOrder.BookedDate;
                this.AmendedDate = dealerOrder.AmendedDate;
                this.RejectedDate = dealerOrder.RejectedDate;
                this.CancelledDate = dealerOrder.CancelledDate;
                this.FilledDate = dealerOrder.FilledDate;
                this.StatusChangedDate = dealerOrder.MostRecentDateEvent();
                this.CreatedDate = dealerOrder.CreatedDate;

                this.DealerId = dealerOrder.DealerId;
                this.TraderName = dealerOrder.DealerName;
                this.Notes = dealerOrder.Notes;

                this.CounterParty = dealerOrder.DealerCounterParty;
                this.LifeCycleStatus = (int?)dealerOrder.OrderStatus();
                this.OrderType = (int?)dealerOrder.OrderType;
                this.Direction = (int?)dealerOrder.OrderDirection;

                this.Currency = dealerOrder.Currency.Code;
                this.SettlementCurrency = dealerOrder.SettlementCurrency.Code;
                this.CleanDirty = (int?)dealerOrder.CleanDirty;
                this.AccumulatedInterest = dealerOrder.AccumulatedInterest;

                this.LimitPrice = dealerOrder.LimitPrice?.Value;
                this.AverageFillPrice = dealerOrder.AverageFillPrice?.Value;
                this.OrderedVolume = dealerOrder.OrderedVolume;
                this.FilledVolume = dealerOrder.FilledVolume;

                this.OptionStrikePrice = dealerOrder.OptionStrikePrice;
                this.OptionExpirationDate = dealerOrder.OptionExpirationDate;
                this.OptionEuropeanAmerican = (int?)dealerOrder.OptionEuropeanAmerican;
            }

            public decimal? AccumulatedInterest { get; set; }

            public DateTime? AmendedDate { get; set; }

            public decimal? AverageFillPrice { get; set; }

            public DateTime? BookedDate { get; set; }

            public DateTime? CancelledDate { get; set; }

            public int? CleanDirty { get; set; }

            public string ClientDealerOrderId { get; set; }

            public string CounterParty { get; set; }

            public DateTime? CreatedDate { get; set; }

            public string Currency { get; set; }

            public string DealerId { get; set; }

            public string DealerOrderGroupId { get; set; }

            public string DealerOrderVersion { get; set; }

            public string DealerOrderVersionLinkId { get; set; }

            public int? Direction { get; set; }

            public DateTime? FilledDate { get; set; }

            public decimal? FilledVolume { get; set; }

            public int? LifeCycleStatus { get; set; }

            public decimal? LimitPrice { get; set; }

            public string Notes { get; set; }

            public int? OptionEuropeanAmerican { get; set; }

            public DateTime? OptionExpirationDate { get; set; }

            public decimal? OptionStrikePrice { get; set; }

            public decimal? OrderedVolume { get; set; }

            public int? OrderId { get; set; }

            public int? OrderType { get; set; }

            public DateTime? PlacedDate { get; set; }

            public string ReddeerDealerOrderId { get; set; }

            public DateTime? RejectedDate { get; set; }

            public string SettlementCurrency { get; set; }

            public DateTime? StatusChangedDate { get; set; }

            public string TraderName { get; set; }
        }

        private class GetQuery
        {
            public DateTime End { get; set; }

            public DateTime Start { get; set; }
        }

        private class OrderDto
        {
            public OrderDto()
            {
                // used by reads
            }

            public OrderDto(Order order)
            {
                if (order == null) return;

                this.MarketId = order.Market?.Id ?? string.Empty;
                this.MarketIdentifierCode = order.Market?.MarketIdentifierCode;
                this.MarketName = order.Market?.Name;
                this.MarketType = (int?)order.Market?.Type;

                this.SecurityId = order?.Instrument.Identifiers.Id;
                this.SecurityReddeerId = order?.Instrument.Identifiers.ReddeerId;
                this.SecurityReddeerEnrichmentId = order?.Instrument.Identifiers.ReddeerEnrichmentId;
                this.SecurityClientIdentifier = order?.Instrument.Identifiers.ClientIdentifier;

                this.SecurityName = order?.Instrument.Name;
                this.SecurityCfi = order?.Instrument.Cfi;
                this.SecurityIssuerIdentifier = order?.Instrument.IssuerIdentifier;
                this.SecurityType = (int?)order?.Instrument.Type;

                this.SecuritySedol = order?.Instrument.Identifiers.Sedol;
                this.SecurityIsin = order?.Instrument.Identifiers.Isin;
                this.SecurityFigi = order?.Instrument.Identifiers.Figi;
                this.SecurityCusip = order?.Instrument.Identifiers.Cusip;
                this.SecurityExchangeSymbol = order?.Instrument.Identifiers.ExchangeSymbol;
                this.SecurityLei = order?.Instrument.Identifiers.Lei;
                this.SecurityBloombergTicker = order?.Instrument.Identifiers.BloombergTicker;
                this.SecurityRic = order?.Instrument.Identifiers.Ric;

                this.UnderlyingSecurityName = order?.Instrument.Name;
                this.UnderlyingSecurityCfi = order?.Instrument.Cfi;
                this.UnderlyingSecurityIssuerIdentifier = order?.Instrument.IssuerIdentifier;

                this.UnderlyingSecuritySedol = order?.Instrument.Identifiers.Sedol;
                this.UnderlyingSecurityIsin = order?.Instrument.Identifiers.Isin;
                this.UnderlyingSecurityFigi = order?.Instrument.Identifiers.Figi;
                this.UnderlyingSecurityCusip = order?.Instrument.Identifiers.Cusip;
                this.UnderlyingSecurityExchangeSymbol = order?.Instrument.Identifiers.ExchangeSymbol;
                this.UnderlyingSecurityLei = order?.Instrument.Identifiers.Lei;
                this.UnderlyingSecurityBloombergTicker = order?.Instrument.Identifiers.BloombergTicker;
                this.UnderlyingClientIdentifier = order?.Instrument.Identifiers.UnderlyingClientIdentifier;
                this.UnderlyingSecurityRic = order?.Instrument.Identifiers.Ric;

                this.SectorCode = order?.Instrument.SectorCode;
                this.IndustryCode = order?.Instrument.IndustryCode;
                this.RegionCode = order?.Instrument.RegionCode;
                this.CountryCode = order?.Instrument.CountryCode;

                this.OrderVersion = order?.OrderVersion;
                this.OrderVersionLinkId = order?.OrderVersionLinkId;
                this.OrderGroupId = order?.OrderGroupId;

                this.ReddeerOrderId = order.ReddeerOrderId;
                this.OrderId = order.OrderId;
                this.OrderPlacedDate = order.PlacedDate;
                this.OrderBookedDate = order.BookedDate;
                this.OrderAmendedDate = order.AmendedDate;
                this.OrderRejectedDate = order.RejectedDate;
                this.OrderCancelledDate = order.CancelledDate;
                this.OrderFilledDate = order.FilledDate;
                this.OrderStatusChangedDate = order.MostRecentDateEvent();
                this.CreatedDate = order.CreatedDate;

                this.OrderBroker = order.OrderBroker?.Name;
                this.OrderBrokerId = order.OrderBroker?.Id;
                if (this.OrderBrokerId == string.Empty) this.OrderBrokerId = null;

                this.OrderBrokerCreatedOn = order.OrderBroker?.CreatedOn;
                this.OrderBrokerReddeerId = order.OrderBroker?.ReddeerId;
                this.OrderBrokerLive = order.OrderBroker?.Live ?? false;

                this.LifeCycleStatus = (int?)order.OrderStatus();
                this.OrderType = (int?)order.OrderType;
                this.OrderDirection = (int?)order.OrderDirection;
                this.OrderCurrency = order.OrderCurrency.Code ?? string.Empty;
                this.OrderSettlementCurrency = order.OrderSettlementCurrency?.Code ?? string.Empty;
                this.OrderLimitPrice = order.OrderLimitPrice.GetValueOrDefault().Value;
                this.OrderAverageFillPrice = order.OrderAverageFillPrice.GetValueOrDefault().Value;
                this.OrderOrderedVolume = order.OrderOrderedVolume;
                this.OrderFilledVolume = order.OrderFilledVolume;
                this.CleanDirty = (int?)order.OrderCleanDirty;

                this.OrderTraderId = order.OrderTraderId;
                this.OrderTraderName = order.OrderTraderName;
                this.OrderClearingAgent = order.OrderClearingAgent;
                this.OrderDealingInstructions = order.OrderDealingInstructions;
                this.AccumulatedInterest = order.OrderAccumulatedInterest;

                this.OptionEuropeanAmerican = (int?)order.OrderOptionEuropeanAmerican;
                this.OptionExpirationDate = order.OrderOptionExpirationDate;
                this.OptionStrikePrice = order.OrderOptionStrikePrice?.Value;
            }

            public decimal? AccumulatedInterest { get; }

            public int? CleanDirty { get; }

            public string CountryCode { get; }

            public DateTime? CreatedDate { get; }

            public IList<DealerOrdersDto> DealerOrders { get; } = new List<DealerOrdersDto>();

            public string IndustryCode { get; }

            public int? LifeCycleStatus { get; }

            /// <summary>
            ///     The id for the market (primary key)
            /// </summary>
            public string MarketId { get; set; }

            /// <summary>
            ///     The market the security is being traded on
            /// </summary>
            public string MarketIdentifierCode { get; }

            /// <summary>
            ///     The market the security is being traded on
            /// </summary>
            public string MarketName { get; }

            /// <summary>
            ///     The enumeration for the type of market i.e. stock exchange or otc
            /// </summary>
            public int? MarketType { get; }

            public int? OptionEuropeanAmerican { get; }

            public DateTime? OptionExpirationDate { get; }

            public decimal? OptionStrikePrice { get; }

            public DateTime? OrderAmendedDate { get; }

            public decimal? OrderAverageFillPrice { get; }

            public DateTime? OrderBookedDate { get; }

            public string OrderBroker { get; }

            public DateTime? OrderBrokerCreatedOn { get; }

            public string OrderBrokerId { get; }

            public bool OrderBrokerLive { get; }

            public string OrderBrokerReddeerId { get; }

            public DateTime? OrderCancelledDate { get; }

            public string OrderClearingAgent { get; }

            public string OrderCurrency { get; }

            public string OrderDealingInstructions { get; }

            public int? OrderDirection { get; }

            public DateTime? OrderFilledDate { get; }

            public decimal? OrderFilledVolume { get; }

            public string OrderGroupId { get; }

            public string OrderId { get; } // the client id for the order

            public decimal? OrderLimitPrice { get; }

            public decimal? OrderOrderedVolume { get; }

            public DateTime? OrderPlacedDate { get; }

            public DateTime? OrderRejectedDate { get; }

            public string OrderSettlementCurrency { get; }

            public DateTime? OrderStatusChangedDate { get; }

            public string OrderTraderId { get; }

            public string OrderTraderName { get; }

            public int? OrderType { get; }

            public string OrderVersion { get; }

            public string OrderVersionLinkId { get; }

            public int? ReddeerOrderId { get; } // primary key

            public string RegionCode { get; }

            public string SectorCode { get; }

            public string SecurityBloombergTicker { get; }

            public string SecurityCfi { get; }

            public string SecurityClientIdentifier { get; }

            public string SecurityCusip { get; }

            public string SecurityExchangeSymbol { get; }

            public string SecurityFigi { get; }

            // client key
            public string SecurityId { get; }

            public string SecurityIsin { get; }

            public string SecurityIssuerIdentifier { get; }

            public string SecurityLei { get; }

            public string SecurityName { get; }

            // primary key in the sql server db
            public string SecurityReddeerEnrichmentId { get; }

            // primary key
            public string SecurityReddeerId { get; set; }

            public string SecurityRic { get; set; }

            public string SecuritySedol { get; }

            public int? SecurityType { get; }

            public string UnderlyingClientIdentifier { get; }

            public string UnderlyingSecurityBloombergTicker { get; }

            public string UnderlyingSecurityCfi { get; }

            public string UnderlyingSecurityCusip { get; }

            public string UnderlyingSecurityExchangeSymbol { get; }

            public string UnderlyingSecurityFigi { get; }

            public string UnderlyingSecurityIsin { get; }

            public string UnderlyingSecurityIssuerIdentifier { get; }

            public string UnderlyingSecurityLei { get; }

            public string UnderlyingSecurityName { get; }

            public string UnderlyingSecurityRic { get; }

            public string UnderlyingSecuritySedol { get; }
        }
    }
}