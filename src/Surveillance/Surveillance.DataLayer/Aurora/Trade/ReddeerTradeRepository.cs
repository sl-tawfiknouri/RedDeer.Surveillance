﻿using System;
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

        private const string CreateSql2 = @"
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
                @SecurityId,
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
                @OrderClientAccountAttributionId);";

        private const string GetSql = @"
            SELECT
	            ord.Id as Id,
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
            FROM orders as ord
            LEFT OUTER JOIN financialinstruments as fi
            ON fi.Id = ord.SecurityId
            LEFT OUTER JOIN market as mark
            on mark.Id = ord.MarketId
            WHERE 
            ord.PlacedDate >= @Start
            AND Ord.StatusChangedDate <= @End;";

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

                if (string.IsNullOrWhiteSpace(dto.SecurityId))
                {
                    var marketDataPair = new MarketDataPair {Exchange = entity.Market, Security = entity.Instrument};
                    var marketSecurityId = await _marketRepository.CreateAndOrGetSecurityId(marketDataPair);
                    dto.SecurityId = marketSecurityId.SecurityId;
                    dto.MarketId = marketSecurityId.MarketId;
                }

                using (var conn = dbConnection.ExecuteAsync(CreateSql2, dto))
                {
                    await conn;
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

                var query = new GetQuery {Start = start, End = end};
                using (var conn = dbConnection.QueryAsync<OrderDto>(GetSql, query))
                {
                    var rawResult = await conn;

                    return rawResult?.Select(Project).ToList();
                }
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
                        dto.UnderlyingSecurityBloombergTicker),
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

            return new Order(
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
                new DomainV2.Trading.Trade[0]);
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
        }
    }
}
