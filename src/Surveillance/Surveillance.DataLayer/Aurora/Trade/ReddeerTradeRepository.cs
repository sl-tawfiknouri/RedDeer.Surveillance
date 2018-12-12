using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Trade
{
    public class ReddeerTradeRepository : IReddeerTradeRepository
    {
        private readonly IReddeerMarketRepository _marketRepository;
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger _logger;

        private const string InsertOrderSql = @"
            INSERT INTO Orders(
                MarketId,
                SecurityId,
                ClientOrderId,
                PlacedDate,
                BookedDate,
                AmendedDate,
                RejectedDate,
                CancelledDate,
                FilledDate,
                StatusChangedDate,
                OrderType,
                Position,
                Currency,
                LimitPrice,
                AveragePrice,
                OrderedVolume,
                FilledVolume,
                PortfolioManager,
                TraderId,
                ExecutingBroker,
                ClearingAgent,
                DealingInstructions,
                Strategy,
                Rationale,
                Fund,
                ClientAccountAttributionId)
            VALUES(
                @MarketId,
                @SecurityReddeerId,
                @OrderId,
                @OrderPlacedDate,
                @OrderBookedDate,
                @OrderAmendedDate,
                @OrderRejectedDate,
                @OrderCancelledDate,
                @OrderFilledDate,
                @OrderStatusChangedDate,
                @OrderType,
                @OrderPosition,
                @OrderCurrency,
                @OrderLimitPrice,
                @OrderAveragePrice,
                @OrderOrderedVolume,
                @OrderFilledVolume,
                @OrderPortfolioManager,
                @OrderTraderId,
                @OrderExecutingBroker,
                @OrderClearingAgent,
                @OrderDealingInstructions,
                @OrderStrategy,
                @OrderRationale,
                @OrderFund,
                @OrderClientAccountAttributionId);
                SELECT LAST_INSERT_ID();";

        private const string InsertTradeSql = @"
            INSERT INTO Trades(
                OrderId,
                ClientTradeId,
                PlacedDate,
                BookedDate,
                AmendedDate,
                RejectedDate,
                CancelledDate,
                FilledDate,
                TraderId,
                TradeCounterParty,
                OrderType,
                Position,
                Currency,
                LimitPrice,
                AveragePrice,
                OrderedVolume,
                FilledVolume,
                OptionStrikePrice,
                OptionExpirationDate,
                OptionEuropeanAmerican)
            VALUES(
                @OrderId,
                @ClientTradeId,
                @PlacedDate,
                @BookedDate,
                @AmendedDate,
                @RejectedDate,
                @CancelledDate,
                @FilledDate,
                @TraderId,
                @TradeCounterParty,
                @OrderType,
                @Position,
                @Currency,
                @LimitPrice,
                @AveragePrice,
                @OrderedVolume,
                @FilledVolume,
                @OptionStrikePrice,
                @OptionExpirationDate,
                @OptionEuropeanAmerican);
                SELECT LAST_INSERT_ID();";

        private const string InsertTransactionSql = @"
            INSERT INTO Transactions(
                TradeId,
                ClientTransactionId,
                PlacedDate,
                BookedDate,
                AmendedDate,
                RejectedDate,
                CancelledDate,
                FilledDate,
                TraderId,
                CounterParty,
                OrderType,
                Position,
                Currency,
                LimitPrice,
                AveragePrice,
                OrderedVolume,
                FilledVolume)
            VALUES(
                @TradeId,
                @ClientTransactionId,
                @PlacedDate,
                @BookedDate,
                @AmendedDate,
                @RejectedDate,
                @CancelledDate,
                @FilledDate,
                @TraderId,
                @CounterParty,
                @OrderType,
                @Position,
                @Currency,
                @LimitPrice,
                @AveragePrice,
                @OrderedVolume,
                @FilledVolume);
            SELECT LAST_INSERT_ID();";

        private const string GetSql = @"
            SELECT
	            ord.Id as ReddeerOrderId,
                ord.ClientOrderId as OrderId,
                ord.SecurityId as SecurityId,
                ord.PlacedDate as OrderPlacedDate,
                ord.BookedDate as OrderBookedDate,
                ord.AmendedDate as OrderAmendedDate,
                ord.RejectedDate as OrderRejectedDate,
                ord.CancelledDate as OrderCancelledDate,
                ord.FilledDate as OrderFilledDate,
                ord.OrderType as OrderType,
                ord.Position as OrderPosition,
                ord.Currency as OrderCurrency,
                ord.LimitPrice as OrderLimitPrice,
                ord.AveragePrice as OrderAveragePrice,
                ord.OrderedVolume as OrderOrderedVolume,
                ord.FilledVolume as OrderFilledVolume,
                ord.PortfolioManager as OrderPortfolioManager,
                ord.TraderId as OrderTraderId,
                ord.ExecutingBroker as OrderExecutingBroker,
                ord.ClearingAgent as OrderClearingAgent,
                ord.DealingInstructions as OrderDealingInstructions,
                ord.Strategy as OrderStrategy,
                ord.Rationale as OrderRationale,
                ord.Fund as OrderFund,
                ord.ClientAccountAttributionId as OrderClientAccountAttributionId,
	            fi.ReddeerId AS SecurityReddeerId,
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
            AND Ord.StatusChangedDate <= @End;";

        private const string GetTradeSql = @"
            SELECT
                Id as ReddeerTradeId,
                OrderId as OrderId,
                ClientTradeId as ClientTradeId,
                PlacedDate as PlacedDate,
                BookedDate as BookedDate,
                AmendedDate as AmendedDate,
                RejectedDate as RejectedDate,
                CancelledDate as CancelledDate,
                FilledDate as FilledDate,
                TraderId as TraderId,
                TradeCounterParty as TradeCounterParty,
                OrderType as OrderType,
                Position as Position,
                Currency as Currency,
                LimitPrice as LimitPrice,
                AveragePrice as AveragePrice,
                OrderedVolume as OrderedVolume,
                FilledVolume as FilledVolume,
                OptionStrikePrice as OptionStrikePrice,
                OptionExpirationDate as OptionExpirationDate,
                OptionEuropeanAmerican as OptionEuropeanAmerican
            FROM Trades
            WHERE OrderId IN @OrderIds";

        private const string GetTransactionSql = @"
            SELECT
                Id as ReddeerTransactionId,
                TradeId as TradeId,
                ClientTransactionId as ClientTransactionId,
                PlacedDate as PlacedDate,
                BookedDate as BookedDate,
                AmendedDate as AmendedDate,
                RejectedDate as RejectedDate,
                CancelledDate as CancelledDate,
                FilledDate as FilledDate,
                TraderId as TraderId,
                CounterParty as CounterParty,
                OrderType as OrderType,
                Position as Position,
                Currency as Currency,
                LimitPrice as LimitPrice,
                AveragePrice as AveragePrice,
                OrderedVolume as OrderedVolume,
                FilledVolume as FilledVolume
            FROM Transactions
            WHERE TradeId IN @TradeIds";

        public ReddeerTradeRepository(
            IConnectionStringFactory connectionStringFactory,
            IReddeerMarketRepository marketRepository,
            ILogger<ReddeerTradeRepository> logger)
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
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var dto = new OrderDto(entity);

                if (string.IsNullOrWhiteSpace(dto.SecurityReddeerId)
                || string.IsNullOrWhiteSpace(dto.MarketId))
                {
                    var marketDataPair = new MarketDataPair { Exchange = entity.Market, Security = entity.Instrument };
                    var marketSecurityId = await _marketRepository.CreateAndOrGetSecurityId(marketDataPair);
                    dto.SecurityReddeerId = marketSecurityId.SecurityId;
                    dto.MarketId = marketSecurityId.MarketId;
                }

                using (var conn = dbConnection.ExecuteScalarAsync<int?>(InsertOrderSql, dto))
                {
                    var orderId = await conn;
                    entity.ReddeerOrderId = orderId;
                }

                if (entity.ReddeerOrderId == null)
                {
                    _logger.LogError($"Attempted to save order {entity.OrderId} from client but did not get a reddeer order id (primary key) value.");
                }

                if (entity.Trades == null
                    || !entity.Trades.Any())
                {
                    return;
                }

                foreach (var trade in entity.Trades)
                {
                    if (trade == null)
                    {
                        continue;
                    }

                    var tradeDto = new TradeDto(trade, entity.ReddeerOrderId);
                    using (var conn = dbConnection.ExecuteScalarAsync<int?>(InsertTradeSql, tradeDto))
                    {
                        var tradeId = await conn;
                        tradeDto.ReddeerTradeId = tradeId;
                    }

                    if (tradeDto.ReddeerTradeId == null)
                    {
                        continue;
                    }

                    if (trade.Transactions == null
                        || !trade.Transactions.Any())
                    {
                        continue;
                    }

                    foreach (var transaction in trade.Transactions)
                    {
                        if (transaction == null)
                        {
                            continue;
                        }

                        var transactionDto = new TransactionDto(transaction, tradeDto.ReddeerTradeId);
                        using (var conn = dbConnection.ExecuteScalarAsync<int?>(InsertTransactionSql, transactionDto))
                        {
                            var transactionId = await conn;
                            transactionDto.ReddeerTransactionId = transactionId;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerTradeRepository Create Method For {entity.Instrument?.Name} {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<Order>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx)
        {
            start = start.Date;
            end = end.Date.AddDays(1).AddMilliseconds(-1);

            if (end < start)
            {
                return new Order[0];
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                // GET ORDERS
                var orders = new List<Order>();
                var query = new GetQuery { Start = start, End = end };
                using (var conn = dbConnection.QueryAsync<OrderDto>(GetSql, query))
                {
                    var rawResult = await conn;

                    orders = rawResult?.Select(Project).ToList();
                }

                // GET TRADES
                var orderIds = orders.Select(ord => ord.ReddeerOrderId?.ToString()).Where(x => x != null).ToList();
                var tradeIds = new List<string>();
                var tradeDtos = new List<TradeDto>();
                using (var conn = dbConnection.QueryAsync<TradeDto>(GetTradeSql, new { OrderIds = orderIds }))
                {
                    tradeDtos = (await conn).ToList();
                    tradeIds = tradeDtos.Select(tfo => tfo.ReddeerTradeId?.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                }

                // GET TRANSACTIONS
                var transactionDtos = new List<TransactionDto>();
                using (var conn = dbConnection.QueryAsync<TransactionDto>(GetTransactionSql, new { TradeIds = tradeIds }))
                {
                    transactionDtos = (await conn).ToList();
                }

                // JOIN transactions to trades
                var transGroups = transactionDtos.GroupBy(tfo => tfo.ReddeerTransactionId);
                foreach (var grp in transGroups)
                {
                    var trade = tradeDtos.FirstOrDefault(td => td.ReddeerTradeId == grp.Key);
                    if (trade == null)
                    {
                        continue;
                    }

                    trade.Transactions = grp.ToList();
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

                    order.Trades = grp.Select(tr => Project(tr, order.Instrument)).ToList();
                    foreach (var trad in order.Trades)
                        trad.ParentOrder = order;
                }

                return orders;
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerTradeRepository Get Method For {start.ToShortDateString()} to {end.ToShortDateString()} {e.Message}");
                opCtx.EventError(e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new Order[0];
        }

        private Order Project(OrderDto dto)
        {
            var financialInstrument =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    new InstrumentIdentifiers(
                        dto.SecurityId,
                        dto.SecurityReddeerId,
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
                    dto.SecurityIssuerIdentifier,
                    dto.UnderlyingSecurityName,
                    dto.UnderlyingSecurityCfi,
                    dto.UnderlyingSecurityIssuerIdentifier);

            Enum.TryParse(dto.MarketType?.ToString() ?? string.Empty, out MarketTypes result);
            var orderTypeResult = (OrderTypes)dto.OrderType.GetValueOrDefault(0);
            var orderPositionResult = (OrderPositions)dto.OrderPosition.GetValueOrDefault(0);
            var orderCurrency = new Currency(dto.OrderCurrency);
            var limitPrice = new CurrencyAmount(dto.OrderLimitPrice, dto.OrderCurrency);
            var averagePrice = new CurrencyAmount(dto.OrderAveragePrice, dto.OrderCurrency);

            var market = new DomainV2.Financial.Market(dto.MarketId, dto.MarketIdentifierCode, dto.MarketName, result);
            var trades = dto.Trades?.Select(tr => Project(tr, financialInstrument)).ToList() ?? new List<DomainV2.Trading.Trade>();

            var order = new Order(
                financialInstrument,
                market,
                dto.ReddeerOrderId,
                dto.OrderId,
                dto.OrderPlacedDate,
                dto.OrderBookedDate,
                dto.OrderAmendedDate,
                dto.OrderRejectedDate,
                dto.OrderCancelledDate,
                dto.OrderFilledDate,
                orderTypeResult,
                orderPositionResult,
                orderCurrency,
                limitPrice,
                averagePrice,
                dto.OrderOrderedVolume,
                dto.OrderFilledVolume,
                dto.OrderPortfolioManager,
                dto.OrderTraderId,
                dto.OrderExecutingBroker,
                dto.OrderClearingAgent,
                dto.OrderDealingInstructions,
                dto.OrderStrategy,
                dto.OrderRationale,
                dto.OrderFund,
                dto.OrderClientAccountAttributionId,
                trades);

            foreach (var trad in trades)
                trad.ParentOrder = order;

            return order;
        }

        private DomainV2.Trading.Trade Project(TradeDto dto, FinancialInstrument fi)
        {
            var orderType = (OrderTypes)dto.OrderType.GetValueOrDefault(0);
            var orderPosition = (OrderPositions)dto.Position.GetValueOrDefault(0);
            var orderCurrency = new Currency(dto.Currency);
            var orderLimit =
                dto.LimitPrice != null
                    ? (CurrencyAmount?)new CurrencyAmount(dto.LimitPrice, dto.Currency)
                    : null;
            var orderAveragePrice =
                dto.AveragePrice != null
                    ? (CurrencyAmount?)new CurrencyAmount(dto.AveragePrice, dto.Currency)
                    : null;

            var trans = dto.Transactions?.Select(tr => Project(tr, fi)).ToList();

            var trade = new DomainV2.Trading.Trade(
                fi,
                dto.ReddeerTradeId?.ToString(),
                dto.ClientTradeId,
                dto.PlacedDate,
                dto.BookedDate,
                dto.AmendedDate,
                dto.RejectedDate,
                dto.CancelledDate,
                dto.FilledDate,
                dto.TraderId,
                dto.TradeCounterParty,
                orderType,
                orderPosition,
                orderCurrency,
                orderLimit,
                orderAveragePrice,
                dto.OrderedVolume, 
                dto.FilledVolume,
                dto.OptionStrikePrice,
                dto.OptionExpirationDate,
                dto.OptionEuropeanAmerican,
                trans);

            foreach (var tran in trade.Transactions)
                tran.ParentTrade = trade;

            return trade;
        }

        private Transaction Project(TransactionDto dto, FinancialInstrument fi)
        {
            var orderType = (OrderTypes)dto.OrderType.GetValueOrDefault(0);
            var orderPosition = (OrderPositions)dto.Position.GetValueOrDefault(0);
            var orderCurrency = new Currency(dto.Currency);
            var orderLimit =
                dto.LimitPrice != null
                    ? (CurrencyAmount?)new CurrencyAmount(dto.LimitPrice, dto.Currency)
                    : null;
            var orderAveragePrice =
                dto.AveragePrice != null
                    ? (CurrencyAmount?)new CurrencyAmount(dto.AveragePrice, dto.Currency)
                    : null;

            return new Transaction(
                fi,
                dto.ReddeerTransactionId?.ToString(),
                dto.ClientTransactionId,
                dto.PlacedDate,
                dto.BookedDate,
                dto.AmendedDate,
                dto.RejectedDate,
                dto.CancelledDate,
                dto.FilledDate,
                dto.TraderId,
                dto.CounterParty,
                orderType,
                orderPosition,
                orderCurrency,
                orderLimit,
                orderAveragePrice,
                dto.OrderedVolume,
                dto.FilledVolume);
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

                ReddeerOrderId = order.ReddeerOrderId;
                OrderId = order.OrderId;
                OrderPlacedDate = order.OrderPlacedDate;
                OrderBookedDate = order.OrderBookedDate;
                OrderAmendedDate = order.OrderAmendedDate;
                OrderRejectedDate = order.OrderRejectedDate;
                OrderCancelledDate = order.OrderCancelledDate;
                OrderFilledDate = order.OrderFilledDate;
                OrderStatusChangedDate = order.MostRecentDateEvent();

                OrderType = (int?)order.OrderType;
                OrderPosition = (int?)order.OrderPosition;
                OrderCurrency = order.OrderCurrency.Value ?? string.Empty;
                OrderLimitPrice = order.OrderLimitPrice.GetValueOrDefault().Value;
                OrderAveragePrice = order.OrderAveragePrice.GetValueOrDefault().Value;
                OrderOrderedVolume = order.OrderOrderedVolume;
                OrderFilledVolume = order.OrderFilledVolume;

                OrderPortfolioManager = order.OrderPortfolioManager;
                OrderTraderId = order.OrderTraderId;
                OrderExecutingBroker = order.OrderExecutingBroker;
                OrderClearingAgent = order.OrderClearingAgent;
                OrderDealingInstructions = order.OrderDealingInstructions;
                OrderStrategy = order.OrderStrategy;
                OrderRationale = order.OrderRationale;
                OrderFund = order.OrderFund;
                OrderClientAccountAttributionId = order.OrderClientAccountAttributionId;
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


            public string SecurityId { get; set; }
            public string SecurityReddeerId { get; set; }
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
            public int? OrderType { get; set; }
            public int? OrderPosition { get; set; }
            public string OrderCurrency { get; set; }
            public decimal? OrderLimitPrice { get; set; }
            public decimal? OrderAveragePrice { get; set; }
            public long? OrderOrderedVolume { get; set; }
            public long? OrderFilledVolume { get; set; }
            public string OrderPortfolioManager { get; set; }
            public string OrderTraderId { get; set; }
            public string OrderExecutingBroker { get; set; }
            public string OrderClearingAgent { get; set; }
            public string OrderDealingInstructions { get; set; }
            public string OrderStrategy { get; set; }
            public string OrderRationale { get; set; }
            public string OrderFund { get; set; }
            public string OrderClientAccountAttributionId { get; set; }
            public IList<TradeDto> Trades { get; set; } = new List<TradeDto>();
        }

        public class TradeDto
        {
            public TradeDto()
            { }

            public TradeDto(DomainV2.Trading.Trade trade, int? orderId)
            {
                OrderId = orderId;

                if (trade == null)
                {
                    return;
                }

                ClientTradeId = trade.TradeId;
                PlacedDate = trade.TradePlacedDate;
                BookedDate = trade.TradeBookedDate;
                AmendedDate = trade.TradeAmendedDate;
                RejectedDate = trade.TradeRejectedDate;
                CancelledDate = trade.TradeCancelledDate;
                FilledDate = trade.TradeFilledDate;

                TraderId = trade.TraderId;
                TradeCounterParty = trade.TradeCounterParty;

                OrderType = (int?)trade.TradeType;
                Position = (int?)trade.TradePosition;
                Currency = trade.TradeCurrency.Value;
                LimitPrice = trade.TradeLimitPrice?.Value;
                AveragePrice = trade.TradeAveragePrice?.Value;
                OrderedVolume = trade.TradeOrderedVolume;
                FilledVolume = trade.TradeFilledVolume;

                OptionStrikePrice = trade.TradeOptionStrikePrice;
                OptionExpirationDate = trade.TradeOptionExpirationDate;
                OptionEuropeanAmerican = trade.TradeOptionEuropeanAmerican;

                if (string.IsNullOrWhiteSpace(OptionEuropeanAmerican))
                    OptionEuropeanAmerican = null;
            }

            public int? ReddeerTradeId { get; set; }
            public int? OrderId { get; set; }
            public string ClientTradeId { get; set; }

            public DateTime? PlacedDate { get; set; }
            public DateTime? BookedDate { get; set; }
            public DateTime? AmendedDate { get; set; }
            public DateTime? RejectedDate { get; set; }
            public DateTime? CancelledDate { get; set; }
            public DateTime? FilledDate { get; set; }

            public string TraderId { get; set; }
            public string TradeCounterParty { get; set; }

            public int? OrderType { get; set; }
            public int? Position { get; set; }
            public string Currency { get; set; }
            public decimal? LimitPrice { get; set; }
            public decimal? AveragePrice { get; set; }
            public long? OrderedVolume { get; set; }
            public long? FilledVolume { get; set; }

            public decimal? OptionStrikePrice { get; set; }
            public DateTime? OptionExpirationDate { get; set; }
            public string OptionEuropeanAmerican { get; set; }

            public IList<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
        }

        public class TransactionDto
        {
            public TransactionDto()
            { }

            public TransactionDto(Transaction transaction, int? tradeId)
            {
                TradeId = tradeId;

                if (transaction == null)
                {
                    return;
                }

                ClientTransactionId = transaction.TransactionId;

                PlacedDate = transaction.TransactionPlacedDate;
                BookedDate = transaction.TransactionBookedDate;
                AmendedDate = transaction.TransactionAmendedDate;
                RejectedDate = transaction.TransactionRejectedDate;
                CancelledDate = transaction.TransactionCancelledDate;
                FilledDate = transaction.TransactionFilledDate;

                TraderId = transaction.TransactionTraderId;
                CounterParty = transaction.TransactionCounterParty;

                OrderType = (int?)transaction.TransactionType;
                Position = (int?)transaction.TransactionPosition;
                Currency = transaction.TransactionCurrency.Value;
                LimitPrice = transaction.TransactionLimitPrice?.Value;
                AveragePrice = transaction.TransactionAveragePrice?.Value;
                OrderedVolume = transaction.TransactionOrderedVolume;
                FilledVolume = transaction.TransactionFilledVolume;
            }

            public int? ReddeerTransactionId { get; set; }
            public int? TradeId { get; set; }
            public string ClientTransactionId { get; set; }

            public DateTime? PlacedDate { get; set; }
            public DateTime? BookedDate { get; set; }
            public DateTime? AmendedDate { get; set; }
            public DateTime? RejectedDate { get; set; }
            public DateTime? CancelledDate { get; set; }
            public DateTime? FilledDate { get; set; }

            public string TraderId { get; set; }
            public string CounterParty { get; set; }

            public int? OrderType { get; set; }
            public int? Position { get; set; }
            public string Currency { get; set; }
            public decimal? LimitPrice { get; set; }
            public decimal? AveragePrice { get; set; }
            public long? OrderedVolume { get; set; }
            public long? FilledVolume { get; set; }
        }
    }
}