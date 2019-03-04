using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;

namespace Surveillance.DataLayer.Aurora.Orders
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly IReddeerMarketRepository _marketRepository;
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger _logger;

        private const string InsertOrderSql = @"
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
                @OrderClearingAgent,
                @OrderDealingInstructions,
                @OptionStrikePrice,
                @OptionExpirationDate,
                @OptionEuropeanAmerican)
            ON DUPLICATE KEY UPDATE
                ClientOrderId =@OrderId,
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
                ClearingAgent=@OrderClearingAgent,
                DealingInstructions=@OrderDealingInstructions,
                OptionStrikePrice=@OptionStrikePrice,
                OptionExpirationDate=@OptionExpirationDate,
                OptionEuropeanAmerican=@OptionEuropeanAmerican,
                Live = 0,
                Autoscheduled = 0,
                Id = LAST_INSERT_ID(Id);
                SELECT LAST_INSERT_ID();";

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
	            fi.Id AS SecurityReddeerId,
	            fi.ClientIdentifier AS SecurityClientIdentifier,
	            fi.Sedol AS SecuritySedol,
	            fi.Isin AS SecurityIsin,
	            fi.Figi AS SecurityFigi,
	            fi.Cusip AS SecurityCusip,
	            fi.ExchangeSymbol AS SecurityExchangeSymbol,
	            fi.Lei AS SecurityLei,
	            fi.BloombergTicker AS SecurityBloombergTicker,
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
                mark.Id AS MarketId,
                mark.MarketId AS MarketIdentifierCode,
                mark.MarketName AS MarketName
            FROM Orders as ord
            LEFT OUTER JOIN FinancialInstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN Market as mark
            on mark.Id = ord.MarketId
            WHERE Live = 1 AND Autoscheduled = 0

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
	            fi.Id AS SecurityReddeerId,
	            fi.ClientIdentifier AS SecurityClientIdentifier,
	            fi.Sedol AS SecuritySedol,
	            fi.Isin AS SecurityIsin,
	            fi.Figi AS SecurityFigi,
	            fi.Cusip AS SecurityCusip,
	            fi.ExchangeSymbol AS SecurityExchangeSymbol,
	            fi.Lei AS SecurityLei,
	            fi.BloombergTicker AS SecurityBloombergTicker,
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
                mark.Id AS MarketId,
                mark.MarketId AS MarketIdentifierCode,
                mark.MarketName AS MarketName
            FROM OrdersAllocation as OrdAlloc
            LEFT OUTER JOIN Orders as ord
            ON OrdAlloc.OrderId = ord.ClientOrderId
            LEFT OUTER JOIN FinancialInstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN Market as mark
            on mark.Id = ord.MarketId
            WHERE OrdAlloc.Live = 1 AND OrdAlloc.Autoscheduled = 0;";

        private const string SetOrdersToScheduled = @"
            UPDATE Orders
            SET Autoscheduled = 1
            WHERE ClientOrderId = @OrderId;

            UPDATE OrdersAllocation
            SET Autoscheduled = 1
            WHERE OrderId = @OrderId;";

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
	            fi.Id AS SecurityReddeerId,
	            fi.ClientIdentifier AS SecurityClientIdentifier,
	            fi.Sedol AS SecuritySedol,
	            fi.Isin AS SecurityIsin,
	            fi.Figi AS SecurityFigi,
	            fi.Cusip AS SecurityCusip,
	            fi.ExchangeSymbol AS SecurityExchangeSymbol,
	            fi.Lei AS SecurityLei,
	            fi.BloombergTicker AS SecurityBloombergTicker,
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
                mark.Id AS MarketId,
                mark.MarketId AS MarketIdentifierCode,
                mark.MarketName AS MarketName
            FROM Orders as ord
            LEFT OUTER JOIN FinancialInstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN Market as mark
            on mark.Id = ord.MarketId
            WHERE Live = 0 AND CreatedDate < @StaleDate;";

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
	            fi.Id AS SecurityReddeerId,
	            fi.ClientIdentifier AS SecurityClientIdentifier,
	            fi.Sedol AS SecuritySedol,
	            fi.Isin AS SecurityIsin,
	            fi.Figi AS SecurityFigi,
	            fi.Cusip AS SecurityCusip,
	            fi.ExchangeSymbol AS SecurityExchangeSymbol,
	            fi.Lei AS SecurityLei,
	            fi.BloombergTicker AS SecurityBloombergTicker,
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
                mark.Id AS MarketId,
                mark.MarketId AS MarketIdentifierCode,
                mark.MarketName AS MarketName
            FROM Orders as ord
            LEFT OUTER JOIN FinancialInstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN Market as mark
            on mark.Id = ord.MarketId
            WHERE
            ord.PlacedDate >= @Start
            AND ord.StatusChangedDate <= @End
            AND ord.Live = 1;";

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

         public OrdersRepository(
            IConnectionStringFactory connectionStringFactory,
            IReddeerMarketRepository marketRepository,
            ILogger<OrdersRepository> logger)
        {
            _dbConnectionFactory =
                connectionStringFactory
                ?? throw new ArgumentNullException(nameof(connectionStringFactory));

            _marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(Order entity)
        {
            if (entity == null)
            {
                _logger.LogError($"ReddeerTradeRepository Create passed a null order entity. Returning.");
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var dto = new OrderDto(entity);

                _logger.LogInformation($"ReddeerTradeRepository beginning save for order {entity.OrderId}");

                if (string.IsNullOrWhiteSpace(dto.SecurityReddeerId)
                || string.IsNullOrWhiteSpace(dto.MarketId))
                {
                    var marketDataPair = new MarketDataPair { Exchange = entity.Market, Security = entity.Instrument };
                    var marketSecurityId = await _marketRepository.CreateAndOrGetSecurityId(marketDataPair);
                    dto.SecurityReddeerId = marketSecurityId.SecurityId;
                    dto.MarketId = marketSecurityId.MarketId;
                }

                _logger.LogInformation($"ReddeerTradeRepository Create about to insert a new order");
                using (var conn = dbConnection.ExecuteScalarAsync<int?>(InsertOrderSql, dto))
                {
                    var orderId = await conn;
                    entity.ReddeerOrderId = orderId;
                    _logger.LogInformation($"ReddeerTradeRepository Create completed for the new order {orderId}");
                }

                if (entity.ReddeerOrderId == null)
                {
                    _logger.LogError($"Attempted to save order {entity.OrderId} from client but did not get a reddeer order id (primary key) value.");
                }

                if (entity.DealerOrders == null
                    || !entity.DealerOrders.Any())
                {
                    _logger.LogInformation($"ReddeerTradeRepository Create saved an order with id {entity.ReddeerOrderId} and it had no trades so returning.");
                    return;
                }

                foreach (var trade in entity.DealerOrders)
                {
                    if (trade == null)
                    {
                        continue;
                    }

                    _logger.LogInformation($"ReddeerTradeRepository Create about to insert a new trade entry for order {entity.ReddeerOrderId}");
                    var tradeDto = new DealerOrdersDto(trade, entity.ReddeerOrderId);
                    using (var conn = dbConnection.ExecuteScalarAsync<string>(InsertDealerOrderSql, tradeDto))
                    {
                        var tradeId = await conn;
                        tradeDto.ReddeerDealerOrderId = tradeId;
                        _logger.LogInformation($"ReddeerTradeRepository Create inserted a new trade entry for order {entity.ReddeerOrderId} and it had an id of {tradeId}");
                    }
                }

                _logger.LogInformation($"ReddeerTradeRepository finished save for order {entity.OrderId}");
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerTradeRepository Create Method For {entity.Instrument?.Name} {e.Message} {e.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<Order>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx)
        {
            _logger.LogInformation($"ReddeerTradeRepository asked to get orders from {start} to {end} for system process operation {opCtx?.Id}");

            start = start.Date;
            end = end.Date.AddDays(1).AddMilliseconds(-1);

            if (end < start)
            {
                _logger.LogError($"ReddeerTradeRepository asked to get orders from {start} to {end} for system process operation {opCtx?.Id} but the end date predated the start date!");

                return new Order[0];
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                // GET ORDERS
                var orders = new List<Order>();
                var query = new GetQuery { Start = start, End = end };

                _logger.LogInformation($"ReddeerTradeRepository asked to get orders from {start} to {end} for system process operation {opCtx?.Id}");
                using (var conn = dbConnection.QueryAsync<OrderDto>(GetOrderSql, query))
                {
                    var rawResult = await conn;

                    _logger.LogInformation($"ReddeerTradeRepository has gotten orders {rawResult?.Count() ?? 0} from {start} to {end} for system process operation {opCtx?.Id}");

                    orders = rawResult?.Select(Project).ToList();
                }

                // GET TRADES
                var orderIds = orders.Select(ord => ord.ReddeerOrderId?.ToString()).Where(x => x != null).ToList();
                var tradeIds = new List<string>();
                var tradeDtos = new List<DealerOrdersDto>();

                if (orderIds?.Any() ?? false)
                {
                    _logger.LogInformation($"ReddeerTradeRepository getting trades from {start} to {end} for system process operation {opCtx?.Id}");
                    using (var conn = dbConnection.QueryAsync<DealerOrdersDto>(GetDealerOrdersSql, new { OrderIds = orderIds }))
                    {
                        tradeDtos = (await conn).ToList();
                        tradeIds = tradeDtos.Select(tfo => tfo.ReddeerDealerOrderId?.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        _logger.LogInformation($"ReddeerTradeRepository completed getting trades from {start} to {end} for system process operation {opCtx?.Id}");
                    }
                }

                // JOIN trades to orders
                var groups = tradeDtos.GroupBy(tfo => tfo.OrderId);
                foreach (var grp in groups)
                {
                    var order = orders.FirstOrDefault(ord => ord.ReddeerOrderId == grp.Key);
                    if (order == null)
                    {
                        continue;
                    }

                    order.DealerOrders = grp.Select(tr => Project(tr, order.Instrument)).ToList();
                    foreach (var trad in order.DealerOrders)
                        trad.ParentOrder = order;
                }

                _logger.LogInformation($"ReddeerTradeRepository returning from get orders from {start} to {end} for system process operation {opCtx?.Id} with {orders?.Count} orders");

                return orders;
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerTradeRepository Get Method For {start.ToShortDateString()} to {end.ToShortDateString()} {e.Message} {e.InnerException?.Message}");
                opCtx.EventError(e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new Order[0];
        }

        public async Task<IReadOnlyCollection<Order>> LiveUnscheduledOrders()
        {
            _logger.LogInformation($"OrdersRepository asked to get live unscheduled order ids");

            try
            {
                using (var open = _dbConnectionFactory.BuildConn())
                using (var conn = open.QueryAsync<OrderDto>(GetLiveUnautoscheduledOrders))
                {
                    var response = await conn;
                    var projectedResponse = response.Select(Project).ToList();

                    _logger.LogInformation($"OrdersRepository completed getting live unscheduled order ids");

                    return projectedResponse;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"OrdersRepository LiveUnscheduledOrderIds encountered an exception {e.Message}", e);
            }

            return new List<Order>();
        }

        public async Task SetOrdersScheduled(IReadOnlyCollection<Order> orders)
        {
            if (orders == null
                || !orders.Any())
            {
                return;
            }

            _logger.LogInformation($"OrdersRepository asked to set orders scheduled");

            try
            {
                var dtos = orders.Select(i => new OrderDto(i)).ToList();

                using (var open = _dbConnectionFactory.BuildConn())
                using (var conn = open.ExecuteAsync(SetOrdersToScheduled, dtos))
                {
                    await conn;

                    _logger.LogInformation($"OrdersRepository completed setting orders scheduled");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"OrdersRepository set orders as scheduled encountered an exception {e.Message}", e);
            }
        }

        public async Task LivenCompletedOrderSets()
        {
            _logger.LogInformation($"OrdersRepository asked to set order livening");

            try
            {
                using (var open = _dbConnectionFactory.BuildConn())
                using (var conn = open.ExecuteAsync(SetOrdersToLivened))
                {
                    await conn;

                    _logger.LogInformation($"OrdersRepository completed setting order livening");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"OrdersRepository liven completed order sets exception {e.Message}", e);
            }
        }

        /// <summary>
        /// Does not eagerly fetch related domain entities
        /// </summary>
        public async Task<IReadOnlyCollection<Order>> StaleOrders(DateTime stalenessDate)
        {
            _logger.LogInformation($"OrdersRepository asked to fetch stale orders");

            try
            {
                using (var open = _dbConnectionFactory.BuildConn())
                using (var conn = open.QueryAsync<OrderDto>(GetStaleOrders, new { StaleDate = stalenessDate }))
                {
                    var queryResult = await conn;

                    var staleOrders = queryResult.Select(Project).ToList();
                    _logger.LogInformation($"OrdersRepository completed fetching stale orders");

                    return staleOrders;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"OrdersRepository fetch stale orders exception {e.Message}", e);
            }

            return new List<Order>();
        }

        private Order Project(OrderDto dto)
        {
            var financialInstrument =
                new FinancialInstrument(
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
                        dto.UnderlyingSecuritySedol,
                        dto.UnderlyingSecurityIsin,
                        dto.UnderlyingSecurityFigi,
                        dto.UnderlyingSecurityCusip,
                        dto.UnderlyingSecurityLei,
                        dto.UnderlyingSecurityExchangeSymbol,
                        dto.UnderlyingSecurityBloombergTicker,
                        dto.UnderlyingClientIdentifier),
                    dto.SecurityName,
                    dto.SecurityCfi,
                    dto.OrderCurrency,
                    dto.SecurityIssuerIdentifier,
                    dto.UnderlyingSecurityName,
                    dto.UnderlyingSecurityCfi,
                    dto.UnderlyingSecurityIssuerIdentifier);

            Enum.TryParse(dto.MarketType?.ToString() ?? string.Empty, out MarketTypes result);
            var orderTypeResult = (OrderTypes)dto.OrderType.GetValueOrDefault(0);
            var orderDirectionResult = (OrderDirections)dto.OrderDirection.GetValueOrDefault(0);
            var orderCurrency = new Currency(dto.OrderCurrency);
            var limitPrice = new CurrencyAmount(dto.OrderLimitPrice, dto.OrderCurrency);
            var averagePrice = new CurrencyAmount(dto.OrderAverageFillPrice, dto.OrderCurrency);

            var settlementCurrency = 
                !string.IsNullOrWhiteSpace(dto.OrderSettlementCurrency)
                    ? (Currency?) new Currency(dto.OrderSettlementCurrency)
                    : null;

            var orderCleanDirty = (OrderCleanDirty)dto.CleanDirty.GetValueOrDefault(0);
            var orderAccumulatedInterest = dto.AccumulatedInterest;

            var market = new Domain.Financial.Market(dto.MarketId, dto.MarketIdentifierCode, dto.MarketName, result);
            var dealerOrders = dto.DealerOrders?.Select(tr => Project(tr, financialInstrument)).ToList() ?? new List<DealerOrder>();

            var optionEuropeanAmerican = (OptionEuropeanAmerican) dto.OptionEuropeanAmerican.GetValueOrDefault(0);
            var optionStrikePrice = dto.OptionStrikePrice == null
                ? null
                : (CurrencyAmount?) new CurrencyAmount(dto.OptionStrikePrice, dto.OrderCurrency);

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
            var orderCleanDirty = (OrderCleanDirty) dto.CleanDirty.GetValueOrDefault(0);
            var optionEuropeanAmerican = (OptionEuropeanAmerican) dto.OptionEuropeanAmerican.GetValueOrDefault();
            var orderCurrency = new Currency(dto.Currency);
            var settlementCurrency = new Currency(dto.SettlementCurrency);

            var orderLimit =
                dto.LimitPrice != null
                    ? (CurrencyAmount?)new CurrencyAmount(dto.LimitPrice, dto.Currency)
                    : null;
            var orderAveragePrice =
                dto.AverageFillPrice != null
                    ? (CurrencyAmount?)new CurrencyAmount(dto.AverageFillPrice, dto.Currency)
                    : null;

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

        private class GetQuery
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        private class OrderDto
        {
            public OrderDto()
            {
                // used by reads
            }

            public OrderDto(Order order)
            {
                if (order == null)
                {
                    return;
                }

                MarketId = order.Market?.Id ?? string.Empty;
                MarketIdentifierCode = order.Market?.MarketIdentifierCode;
                MarketName = order.Market?.Name;
                MarketType = (int?)order.Market?.Type;

                SecurityId = order?.Instrument.Identifiers.Id;
                SecurityReddeerId = order?.Instrument.Identifiers.ReddeerId;
                SecurityReddeerEnrichmentId = order?.Instrument.Identifiers.ReddeerEnrichmentId;
                SecurityClientIdentifier = order?.Instrument.Identifiers.ClientIdentifier;

                SecurityName = order?.Instrument.Name;
                SecurityCfi = order?.Instrument.Cfi;
                SecurityIssuerIdentifier = order?.Instrument.IssuerIdentifier;
                SecurityType = (int?)order?.Instrument.Type;

                SecuritySedol = order?.Instrument.Identifiers.Sedol;
                SecurityIsin = order?.Instrument.Identifiers.Isin;
                SecurityFigi = order?.Instrument.Identifiers.Figi;
                SecurityCusip = order?.Instrument.Identifiers.Cusip;
                SecurityExchangeSymbol = order?.Instrument.Identifiers.ExchangeSymbol;
                SecurityLei = order?.Instrument.Identifiers.Lei;
                SecurityBloombergTicker = order?.Instrument.Identifiers.BloombergTicker;

                UnderlyingSecurityName = order?.Instrument.Name;
                UnderlyingSecurityCfi = order?.Instrument.Cfi;
                UnderlyingSecurityIssuerIdentifier = order?.Instrument.IssuerIdentifier;

                UnderlyingSecuritySedol = order?.Instrument.Identifiers.Sedol;
                UnderlyingSecurityIsin = order?.Instrument.Identifiers.Isin;
                UnderlyingSecurityFigi = order?.Instrument.Identifiers.Figi;
                UnderlyingSecurityCusip = order?.Instrument.Identifiers.Cusip;
                UnderlyingSecurityExchangeSymbol = order?.Instrument.Identifiers.ExchangeSymbol;
                UnderlyingSecurityLei = order?.Instrument.Identifiers.Lei;
                UnderlyingSecurityBloombergTicker = order?.Instrument.Identifiers.BloombergTicker;
                UnderlyingClientIdentifier = order?.Instrument.Identifiers.UnderlyingClientIdentifier;

                OrderVersion = order?.OrderVersion;
                OrderVersionLinkId = order?.OrderVersionLinkId;
                OrderGroupId = order?.OrderGroupId;

                ReddeerOrderId = order.ReddeerOrderId;
                OrderId = order.OrderId;
                OrderPlacedDate = order.PlacedDate;
                OrderBookedDate = order.BookedDate;
                OrderAmendedDate = order.AmendedDate;
                OrderRejectedDate = order.RejectedDate;
                OrderCancelledDate = order.CancelledDate;
                OrderFilledDate = order.FilledDate;
                OrderStatusChangedDate = order.MostRecentDateEvent();
                CreatedDate = order.CreatedDate;

                LifeCycleStatus = (int?)order.OrderStatus();
                OrderType = (int?)order.OrderType;
                OrderDirection = (int?)order.OrderDirection;
                OrderCurrency = order.OrderCurrency.Value ?? string.Empty;
                OrderSettlementCurrency = order.OrderSettlementCurrency?.Value ?? string.Empty;
                OrderLimitPrice = order.OrderLimitPrice.GetValueOrDefault().Value;
                OrderAverageFillPrice = order.OrderAverageFillPrice.GetValueOrDefault().Value;
                OrderOrderedVolume = order.OrderOrderedVolume;
                OrderFilledVolume = order.OrderFilledVolume;
                CleanDirty = (int?)order.OrderCleanDirty;

                OrderTraderId = order.OrderTraderId;
                OrderTraderName = order.OrderTraderName;
                OrderClearingAgent = order.OrderClearingAgent;
                OrderDealingInstructions = order.OrderDealingInstructions;
                AccumulatedInterest = order.OrderAccumulatedInterest;

                OptionEuropeanAmerican = (int?)order.OrderOptionEuropeanAmerican;
                OptionExpirationDate = order.OrderOptionExpirationDate;
                OptionStrikePrice = order.OrderOptionStrikePrice?.Value;
            }

            /// <summary>
            /// The id for the market (primary key)
            /// </summary>
            public string MarketId { get; set; }

            /// <summary>
            /// The market the security is being traded on
            /// </summary>
            public string MarketIdentifierCode { get; set; }

            /// <summary>
            /// The market the security is being traded on
            /// </summary>
            public string MarketName { get; set; }

            /// <summary>
            /// The enumeration for the type of market i.e. stock exchange or otc
            /// </summary>
            public int? MarketType { get; set; }

            // client key
            public string SecurityId { get; set; }
            // primary key
            public string SecurityReddeerId { get; set; }
            // primary key in the sql server db
            public string SecurityReddeerEnrichmentId { get; set; }
            public string SecurityClientIdentifier { get; set; }

            public string SecurityName { get; set; }
            public string SecurityCfi { get; set; }
            public string SecurityIssuerIdentifier { get; set; }
            public int? SecurityType { get; set; }

            public string SecuritySedol { get; set; }
            public string SecurityIsin { get; set; }
            public string SecurityFigi { get; set; }
            public string SecurityCusip { get; set; }
            public string SecurityExchangeSymbol { get; set; }
            public string SecurityLei { get; set; }
            public string SecurityBloombergTicker { get; set; }

            public string UnderlyingSecurityName { get; set; }
            public string UnderlyingSecurityCfi { get; set; }
            public string UnderlyingSecurityIssuerIdentifier { get; set; }

            public string UnderlyingSecuritySedol { get; set; }
            public string UnderlyingSecurityIsin { get; set; }
            public string UnderlyingSecurityFigi { get; set; }
            public string UnderlyingSecurityCusip { get; set; }
            public string UnderlyingSecurityExchangeSymbol { get; set; }
            public string UnderlyingSecurityLei { get; set; }
            public string UnderlyingSecurityBloombergTicker { get; set; }
            public string UnderlyingClientIdentifier { get; set; }


            public int? ReddeerOrderId { get; set; } // primary key
            public string OrderId { get; set; } // the client id for the order
            public DateTime? OrderPlacedDate { get; set; }
            public DateTime? OrderBookedDate { get; set; }
            public DateTime? OrderAmendedDate { get; set; }
            public DateTime? OrderRejectedDate { get; set; }
            public DateTime? OrderCancelledDate { get; set; }
            public DateTime? OrderFilledDate { get; set; }
            public DateTime? OrderStatusChangedDate { get; set; }
            public DateTime? CreatedDate { get; set; }


            public string OrderVersion { get; set; }
            public string OrderVersionLinkId { get; set; }
            public string OrderGroupId { get; set; }

            public int? LifeCycleStatus { get; set; }
            public int? OrderType { get; set; }
            public int? OrderDirection { get; set; }
            public string OrderCurrency { get; set; }
            public string OrderSettlementCurrency { get; set; }
            public decimal? OrderLimitPrice { get; set; }
            public int? CleanDirty { get; set; }
            public decimal? OrderAverageFillPrice { get; set; }
            public long? OrderOrderedVolume { get; set; }
            public long? OrderFilledVolume { get; set; }
            public string OrderTraderId { get; set; }
            public string OrderTraderName { get; set; }
            public string OrderClearingAgent { get; set; }
            public string OrderDealingInstructions { get; set; }
            public decimal? AccumulatedInterest { get; set; }

            public decimal? OptionStrikePrice { get; set; }
            public DateTime? OptionExpirationDate { get; set; }
            public int? OptionEuropeanAmerican { get; set; }

            public IList<DealerOrdersDto> DealerOrders { get; set; } = new List<DealerOrdersDto>();
        }

        public class DealerOrdersDto
        {
            public DealerOrdersDto()
            { }

            public DealerOrdersDto(DealerOrder dealerOrder, int? orderId)
            {
                OrderId = orderId;

                if (dealerOrder == null)
                {
                    return;
                }

                ReddeerDealerOrderId = dealerOrder.ReddeerDealerOrderId;
                ClientDealerOrderId = dealerOrder.DealerOrderId;

                DealerOrderVersion = dealerOrder.DealerOrderVersion;
                DealerOrderVersionLinkId = dealerOrder.DealerOrderVersionLinkId;
                DealerOrderGroupId = dealerOrder.DealerOrderGroupId;

                PlacedDate = dealerOrder.PlacedDate;
                BookedDate = dealerOrder.BookedDate;
                AmendedDate = dealerOrder.AmendedDate;
                RejectedDate = dealerOrder.RejectedDate;
                CancelledDate = dealerOrder.CancelledDate;
                FilledDate = dealerOrder.FilledDate;
                StatusChangedDate = dealerOrder.MostRecentDateEvent();
                CreatedDate = dealerOrder.CreatedDate;

                DealerId = dealerOrder.DealerId;
                TraderName = dealerOrder.DealerName;
                Notes = dealerOrder.Notes;

                CounterParty = dealerOrder.DealerCounterParty;
                LifeCycleStatus = (int?)dealerOrder.OrderStatus();
                OrderType = (int?)dealerOrder.OrderType;
                Direction = (int?)dealerOrder.OrderDirection;

                Currency = dealerOrder.Currency.Value;
                SettlementCurrency = dealerOrder.SettlementCurrency.Value;
                CleanDirty = (int?)dealerOrder.CleanDirty;
                AccumulatedInterest = dealerOrder.AccumulatedInterest;

                LimitPrice = dealerOrder.LimitPrice?.Value;
                AverageFillPrice = dealerOrder.AverageFillPrice?.Value;
                OrderedVolume = dealerOrder.OrderedVolume;
                FilledVolume = dealerOrder.FilledVolume;

                OptionStrikePrice = dealerOrder.OptionStrikePrice;
                OptionExpirationDate = dealerOrder.OptionExpirationDate;
                OptionEuropeanAmerican = (int?)dealerOrder.OptionEuropeanAmerican;
            }

            public string ReddeerDealerOrderId { get; set; }
            public int? OrderId { get; set; }
            public string ClientDealerOrderId { get; set; }

            public string DealerOrderVersion { get; set; }
            public string DealerOrderVersionLinkId { get; set; }
            public string DealerOrderGroupId { get; set; }


            public DateTime? PlacedDate { get; set; }
            public DateTime? BookedDate { get; set; }
            public DateTime? AmendedDate { get; set; }
            public DateTime? RejectedDate { get; set; }
            public DateTime? CancelledDate { get; set; }
            public DateTime? FilledDate { get; set; }
            public DateTime? StatusChangedDate { get; set; }
            public DateTime? CreatedDate { get; set; }


            public string DealerId { get; set; }
            public string TraderName { get; set; }
            public string Notes { get; set; }

            public string CounterParty { get; set; }
            public int? LifeCycleStatus { get; set; }
            public int? OrderType { get; set; }
            public int? Direction { get; set; }


            public string Currency { get; set; }
            public string SettlementCurrency { get; set; }

            public int? CleanDirty { get; set; }
            public decimal? AccumulatedInterest { get; set; }

            public decimal? LimitPrice { get; set; }
            public decimal? AverageFillPrice { get; set; }
            public long? OrderedVolume { get; set; }
            public long? FilledVolume { get; set; }

            public decimal? OptionStrikePrice { get; set; }
            public DateTime? OptionExpirationDate { get; set; }
            public int? OptionEuropeanAmerican { get; set; }
        }    
    }
}