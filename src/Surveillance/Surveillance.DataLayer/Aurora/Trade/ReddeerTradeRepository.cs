using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Equity;
using Domain.Finance;
using Domain.Market;
using Domain.Trades.Orders;
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

        private const string CreateSql = @"
    INSERT INTO TradeReddeer(
	    OrderTypeId,
	    LimitPrice,
	    LimitCurrency,
	    TradeSubmittedOn,
	    StatusChangedOn,
	    FilledVolume,
	    OrderedVolume,
	    OrderPositionId,
	    OrderStatusId,
	    OrderCurrency,
	    TraderId,
	    TradeClientAttributionId,
	    AccountId,
	    PartyBrokerId,
	    CounterPartyBrokerId,
	    ExecutedPrice,
	    DealerInstructions,
	    TradeRationale,
	    TradeStrategy,
        SecurityId)
    VALUES(
	    @OrderTypeId,
	    @LimitPrice,
	    @LimitCurrency,
	    @TradeSubmittedOn,
	    @StatusChangedOn,
	    @FilledVolume,
	    @OrderedVolume,
	    @OrderPositionId,
	    @OrderStatusId,
	    @OrderCurrency,
	    @TraderId,
	    @TradeClientAttributionId,
	    @AccountId,
	    @PartyBrokerId,
	    @CounterPartyBrokerId,
	    @ExecutedPrice,
	    @DealerInstructions,
	    @TradeRationale,
	    @TradeStrategy,
        @SecurityId);";

        private const string GetSql = @"
        SELECT 
            tr.Id,
	        tr.OrderTypeId,
	        tr.LimitPrice,
	        tr.LimitCurrency,
	        tr.TradeSubmittedOn,
	        tr.StatusChangedOn,
	        tr.FilledVolume,
	        tr.OrderedVolume,
	        tr.OrderPositionId,
	        tr.OrderStatusId,
	        tr.OrderCurrency,
	        tr.TraderId,
	        tr.TradeClientAttributionId,
	        tr.AccountId,
	        tr.PartyBrokerId,
	        tr.CounterPartyBrokerId,
	        tr.ExecutedPrice,
	        tr.DealerInstructions,
	        tr.TradeRationale,
	        tr.TradeStrategy,
            tr.SecurityId,
            mses.ReddeerId AS SecurityReddeerId,
            mses.ClientIdentifier AS SecurityClientIdentifier,
            mses.Sedol AS SecuritySedol,
            mses.Isin AS SecurityIsin,
            mses.Figi AS SecurityFigi,
            mses.Cusip AS SecurityCusip,
            mses.ExchangeSymbol AS SecurityExchangeSymbol,
            mses.Lei AS SecurityLei,
            mses.BloombergTicker AS SecurityBloombergTicker,
            mses.SecurityName AS SecurityName,
            mses.Cfi AS SecurityCfi,
            mses.IssuerIdentifier AS SecurityIssuerIdentifier,
            mse.MarketId,
            mse.MarketName
        FROM 
            TradeReddeer AS tr
        LEFT OUTER JOIN MarketStockExchangeSecurities AS mses 
            ON tr.SecurityId = mses.Id
        LEFT OUTER JOIN MarketStockExchange as mse
            ON mses.MarketStockExchangeId = mse.Id
        WHERE 
            tr.StatusChangedOn >= @Start
            AND tr.StatusChangedOn <= @End;";

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

        public async Task Create(TradeOrderFrame entity)
        {
            if (entity == null)
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var dto = new TradeOrderFrameDto(entity);

                if (string.IsNullOrWhiteSpace(dto.SecurityId))
                {
                    var marketDataPair = new MarketDataPair {Exchange = entity.Market, Security = entity.Security};
                    var securityId = await _marketRepository.CreateAndOrGetSecurityId(marketDataPair);
                    dto.SecurityId = securityId;
                }

                using (var conn = dbConnection.ExecuteAsync(CreateSql, dto))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerTradeRepository Create Method For {entity.Security?.Name} {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<TradeOrderFrame>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx)
        {
            start = start.Date;
            end = end.Date.AddDays(1).AddMilliseconds(-1);

            if (end < start)
            {
                return new TradeOrderFrame[0];
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var query = new GetQuery {Start = start, End = end};
                using (var conn = dbConnection.QueryAsync<TradeOrderFrameDto>(GetSql, query))
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

            return new TradeOrderFrame[0];
        }

        private TradeOrderFrame Project(TradeOrderFrameDto dto)
        {
            var limit =
                dto.LimitPrice.HasValue
                    ? new CurrencyAmount(dto.LimitPrice.Value, dto.LimitCurrency)
                    : (CurrencyAmount?)null;

            var executedPrice =
                dto.ExecutedPrice.HasValue
                    ? new CurrencyAmount(dto.ExecutedPrice.Value, dto.OrderCurrency)
                    : (CurrencyAmount?) null; 

            return new TradeOrderFrame(
                dto.Id,
                (OrderType)dto.OrderTypeId.GetValueOrDefault(0),
                new StockExchange(
                    new Domain.Market.Market.MarketId(dto.MarketId),
                    dto.MarketName),
                new Security(
                    new SecurityIdentifiers(
                        dto.SecurityId,
                        dto.SecurityReddeerId,
                        dto.SecurityClientIdentifier,
                        dto.SecuritySedol,
                        dto.SecurityIsin,
                        dto.SecurityFigi,
                        dto.SecurityCusip,
                        dto.SecurityExchangeSymbol,
                        dto.SecurityLei,
                        dto.SecurityBloombergTicker),
                    dto.SecurityName,
                    dto.SecurityCfi,
                    dto.SecurityIssuerIdentifier),
                limit,
                executedPrice,
                dto.FilledVolume,
                dto.OrderedVolume,
                (OrderPosition)dto.OrderPositionId.GetValueOrDefault(0),
                (OrderStatus)dto.OrderStatusId.GetValueOrDefault(0),
                dto.StatusChangedOn,
                dto.TradeSubmittedOn,
                dto.TraderId,
                dto.TradeClientAttributionId,
                dto.AccountId,
                dto.DealerInstructions,
                dto.PartyBrokerId,
                dto.CounterPartyBrokerId,
                dto.TradeRationale,
                dto.TradeStrategy,
                dto.OrderCurrency);
        }

        private class GetQuery
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        private class TradeOrderFrameDto
        {
            public TradeOrderFrameDto()
            {
            }

            public TradeOrderFrameDto(TradeOrderFrame frame)
            {
                if (frame == null)
                {
                    return;
                }

                Id = frame.Id;
                OrderTypeId = (int)frame.OrderType;
                MarketId = frame.Market?.Id?.Id;
                MarketName = frame.Market?.Name;
                SecurityId = frame.Security?.Identifiers.Id ?? string.Empty;
                SecurityReddeerId = frame.Security?.Identifiers.ReddeerId;
                SecurityClientIdentifier = frame.Security?.Identifiers.ClientIdentifier;
                SecuritySedol = frame.Security?.Identifiers.Sedol;
                SecurityIsin = frame.Security?.Identifiers.Isin;
                SecurityFigi = frame.Security?.Identifiers.Figi;
                SecurityCusip = frame.Security?.Identifiers.Cusip;
                SecurityExchangeSymbol = frame.Security?.Identifiers.ExchangeSymbol;
                SecurityLei = frame.Security?.Identifiers.Lei;
                SecurityBloombergTicker = frame.Security?.Identifiers.BloombergTicker;
                SecurityName = frame.Security?.Name;
                SecurityCfi = frame.Security?.Cfi;
                SecurityIssuerIdentifier = frame.Security?.IssuerIdentifier;
                LimitPrice = frame.Limit?.Value;
                LimitCurrency = frame.Limit?.Currency.Value;
                TradeSubmittedOn = frame.TradeSubmittedOn;
                StatusChangedOn = frame.StatusChangedOn;
                OrderedVolume = frame.OrderedVolume;
                FilledVolume = frame.FulfilledVolume;
                ExecutedPrice = frame.ExecutedPrice?.Value;
                OrderCurrency = frame.OrderCurrency;
                OrderPositionId = (int)frame.Position;
                OrderStatusId = (int)frame.OrderStatus;
                TraderId = frame.TraderId;
                TradeClientAttributionId = frame.TradeClientAttributionId;
                AccountId = frame.AccountId;
                PartyBrokerId = frame.PartyBrokerId;
                CounterPartyBrokerId = frame.CounterPartyBrokerId;
                DealerInstructions = frame.DealerInstructions;
                TradeRationale = frame.TradeRationale;
                TradeStrategy = frame.TradeStrategy;
            }

            /// <summary>
            /// Dapper field for primary key
            /// </summary>
            public int? Id { get; set; }

            /// <summary>
            /// The type of order i.e. market / limit
            /// </summary>
            public int? OrderTypeId { get; set; }

            /// <summary>
            /// The market the security is being traded on
            /// </summary>
            public string MarketId { get; set; }

            /// <summary>
            /// The market the security is being traded on
            /// </summary>
            public string MarketName { get; set; }

            public string SecurityId { get; set; }
            public string SecurityReddeerId { get; set; }
            public string SecurityClientIdentifier { get; set; }
            public string SecuritySedol { get; set; }
            public string SecurityIsin { get; set; }
            public string SecurityFigi { get; set; }
            public string SecurityCusip { get; set; }
            public string SecurityExchangeSymbol { get; set; }
            public string SecurityLei { get; set; }
            public string SecurityBloombergTicker { get; set; }
            public string SecurityName { get; set; }
            public string SecurityCfi { get; set; }
            public string SecurityIssuerIdentifier { get; set; }

            /// <summary>
            /// If its a limit order, the limit price
            /// </summary>
            public decimal? LimitPrice { get; set; }

            /// <summary>
            /// If its a limit order, the limit price (currency)
            /// </summary>
            public string LimitCurrency { get; set; }

            /// <summary>
            /// Trade initially submitted on
            /// </summary>
            public DateTime TradeSubmittedOn { get; set; }

            /// <summary>
            /// Last update to the order (i.e. placed -> cancelled; placed -> fulfilled)
            /// </summary>
            public DateTime StatusChangedOn { get; set; }

            /// <summary>
            /// The amount that was requested to trade
            /// </summary>
            public int OrderedVolume { get; set; }

            /// <summary>
            /// The amount that was traded
            /// </summary>
            public int FilledVolume { get; set; }

            /// <summary>
            /// This is the price the trade was executed at; if several prices use the weighted average
            /// </summary>
            public decimal? ExecutedPrice { get; set; }

            /// <summary>
            /// The currency for the executed price
            /// </summary>
            public string OrderCurrency { get; set; }

            /// <summary>
            /// Buy or Sell
            /// </summary>
            public int? OrderPositionId { get; set; }

            /// <summary>
            /// Status of the order (placed/cancelled/fulfilled)
            /// </summary>
            public int? OrderStatusId { get; set; }

            /// <summary>
            /// A client identifier of the trader placing the order
            /// </summary>
            public string TraderId { get; set; }

            /// <summary>
            /// The client the trader is trading on behalf of
            /// </summary>
            public string TradeClientAttributionId { get; set; }

            /// <summary>
            /// The client account traded for
            /// </summary>
            public string AccountId { get; set; }

            /// <summary>
            /// The broker submitting the trade to the market
            /// </summary>
            public string PartyBrokerId { get; set; }

            /// <summary>
            /// The counter party broker matching the order
            /// </summary>
            public string CounterPartyBrokerId { get; set; }

            /// <summary>
            /// The instruction notes passed to the dealer
            /// </summary>
            public string DealerInstructions { get; set; }

            /// <summary>
            /// The traders rationalisation for the trade
            /// </summary>
            public string TradeRationale { get; set; }

            /// <summary>
            /// The strategy behind the trade
            /// </summary>
            public string TradeStrategy { get; set; }
        }
    }
}
