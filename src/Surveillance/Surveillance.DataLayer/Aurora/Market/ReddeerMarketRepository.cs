using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DomainV2.Equity;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class ReddeerMarketRepository : IReddeerMarketRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ReddeerMarketRepository> _logger;
        private readonly object _lock = new object();

        private const string CreateTmp = @"
            CREATE TEMPORARY TABLE IF NOT EXISTS MarketData(MarketId nvarchar(16), MarketName nvarchar(255), ClientIdentifier nvarchar(255), Sedol nvarchar(8), Isin nvarchar(20), Figi nvarchar(12), Cusip nvarchar(9), Lei nvarchar(20), ExchangeSymbol nvarchar(4000), BloombergTicker nvarchar(4000), SecurityName nvarchar(255), Cfi nvarchar(6), IssuerIdentifier nvarchar(255), SecurityCurrency nvarchar(10), Epoch datetime, BidPrice decimal(18, 3), AskPrice decimal(18, 3), MarketPrice decimal(18, 3), OpenPrice decimal(18, 3), ClosePrice decimal(18, 3), HighIntradayPrice decimal(18, 3), LowIntradayPrice decimal(18, 3), ListedSecurities bigint, MarketCap decimal(18, 3), VolumeTradedInTick bigint, DailyVolume bigint, SecurityId bigint, INDEX(Epoch));";

        private const string InsertIntoTmp = @"
            INSERT INTO MarketData(MarketId, MarketName, ClientIdentifier, Sedol, Isin, Figi, Cusip, Lei, ExchangeSymbol, BloombergTicker, SecurityName, Cfi, IssuerIdentifier, SecurityCurrency, Epoch, BidPrice, AskPrice, MarketPrice, OpenPrice, ClosePrice, HighIntradayPrice, LowIntradayPrice, ListedSecurities, MarketCap, VolumeTradedInTick, DailyVolume) VALUES(@MarketId, @MarketName, @ClientIdentifier, @Sedol, @Isin, @Figi, @Cusip, @Lei, @ExchangeSymbol, @BloombergTicker, @SecurityName, @Cfi, @IssuerIdentifier, @SecurityCurrency, @Epoch, @BidPrice, @AskPrice, @MarketPrice, @OpenPrice, @ClosePrice, @HighIntradayPrice, @LowIntradayPrice, @ListedSecurities, @MarketCap, @VolumeTradedInTick, @DailyVolume);";

        private const string CopyAcrossMarketAndSecurityData = @"
            INSERT INTO MarketStockExchange(MarketId, MarketName) SELECT DISTINCT (MarketId), MarketName FROM MarketData WHERE NOT MarketId in (SELECT MarketId FROM MarketStockExchange);

            INSERT INTO MarketStockExchangeSecurities(MarketStockExchangeId, ClientIdentifier, Sedol, Isin, Figi, Cusip, Lei, ExchangeSymbol, BloombergTicker, SecurityName, Cfi, IssuerIdentifier, SecurityCurrency)
             SELECT  mse.id as MarketStockExchangeId, md.clientIdentifier as ClientIdentifier, md.Sedol as Sedol, md.Isin as Isin, md.Figi as Figi, md.Cusip as Cusip, md.Lei as Lei, md.ExchangeSymbol as ExchangeSymbol, md.BloombergTicker as BloombergTicker, md.SecurityName as SecurityName, md.Cfi as Cfi, md.IssuerIdentifier as IssuerIdentifier, md.SecurityCurrency as SecurityCurrency
             FROM MarketData as md LEFT OUTER JOIN MarketStockExchange as mse ON md.MarketId = mse.MarketId WHERE NOT md.sedol in (SELECT Sedol FROM MarketStockExchangeSecurities) AND NOT md.isin IN (SELECT Isin FROM MarketStockExchangeSecurities);

            UPDATE MarketData
            LEFT OUTER JOIN MarketStockExchangeSecurities on MarketData.isin = MarketStockExchangeSecurities.isin set MarketData.SecurityId = MarketStockExchangeSecurities.Id;
            UPDATE MarketData
            LEFT OUTER JOIN MarketStockExchangeSecurities on MarketData.sedol = MarketStockExchangeSecurities.sedol set MarketData.SecurityId = MarketStockExchangeSecurities.Id;";
        
        private const string InsertSecuritySql = @"
            INSERT INTO MarketStockExchangePrices (SecurityId, Epoch, BidPrice, AskPrice, MarketPrice, OpenPrice, ClosePrice, HighIntradayPrice, LowIntradayPrice, ListedSecurities, MarketCap, VolumeTradedInTick, DailyVolume) SELECT SecurityId, Epoch, BidPrice, AskPrice, MarketPrice, OpenPrice, ClosePrice, HighIntradayPrice, LowIntradayPrice, ListedSecurities, MarketCap, VolumeTradedInTick, DailyVolume FROM MarketData;

            DROP TABLE MarketData;";

        private const string GetMarketSql =
            @"
            SELECT
             MSE.Id as MarketId,
             MSE.MarketId as MarketIdentifierCode,
             MSE.MarketName as MarketName,
             MSES.ReddeerId as ReddeerId,
             MSES.ClientIdentifier as ClientIdentifier,
             MSES.Sedol as Sedol,
             MSES.Isin as Isin,
             MSES.Figi as Figi,
             MSES.Cusip as Cusip,
             MSES.Lei as Lei,
             MSES.ExchangeSymbol as ExchangeSymbol,
             MSES.BloombergTicker as BloombergTicker,
             MSES.SecurityName as SecurityName,
             MSES.Cfi as Cfi,
             MSES.IssuerIdentifier as IssuerIdentifier,
             MSES.SecurityCurrency as SecurityCurrency,
             MSEP.Epoch as Epoch,
             MSEP.BidPrice as BidPrice,
             MSEP.AskPrice as AskPrice,
             MSEP.MarketPrice as MarketPrice,
             MSEP.OpenPrice as OpenPrice,
             MSEP.ClosePrice as ClosePrice,
             MSEP.HighIntradayPrice as HighIntradayPrice,
             MSEP.LowIntradayPrice as LowIntradayPrice,
             MSEP.ListedSecurities as ListedSecurities,
             MSEP.MarketCap as MarketCap,
             MSEP.VolumeTradedInTick as VolumeTradedInTick,
             MSEP.DailyVolume as DailyVolume
             FROM MarketStockExchangePrices AS MSEP
             LEFT JOIN MarketStockExchangeSecurities AS MSES
             ON MSEP.SecurityId = MSES.Id
             LEFT JOIN MarketStockExchange AS MSE
             ON MSES.MarketStockExchangeId = MSE.Id
             WHERE MSEP.Epoch >= @start
             AND MSEP.Epoch <= @end;";

        private const string GetUnEnrichedSecuritiesSql =
            @"SELECT 
              sec.Id AS Id,
              sec.ReddeerId As ReddeerId,
              mse.MarketId as MarketIdentifierCode,
              mse.MarketName As MarketName,
              sec.SecurityName As SecurityName,
              sec.Cfi As Cfi,
              sec.IssuerIdentifier As IssuerIdentifier,
              sec.ClientIdentifier As SecurityClientIdentifier,
              sec.Sedol As Sedol,
              sec.Isin As Isin,
              sec.Figi As Figi,
              sec.ExchangeSymbol As ExchangeSymbol,
              sec.Cusip As Cusip,
              sec.Lei As Lei,
              sec.BloombergTicker As BloombergTicker
              FROM MarketStockExchangeSecurities as sec
              left join MarketStockExchange as mse
              on sec.MarketStockExchangeId = mse.Id
              WHERE sec.Enrichment is null
              LIMIT 10000;";

        private const string UpdateUnEnrichedSecuritiesSql =
            @"UPDATE MarketStockExchangeSecurities
              SET ReddeerId = @ReddeerId,
              SecurityName = @SecurityName,
              Cfi = @Cfi,
              IssuerIdentifier = @IssuerIdentifier,
              ClientIdentifier = @SecurityClientIdentifier,
              Sedol = @Sedol,
              Isin = @Isin,
              Figi = @Figi,
              ExchangeSymbol = @ExchangeSymbol,
              Cusip = @Cusip,
              Lei = @Lei,
              BloombergTicker = @BloombergTicker,
              Enrichment = (SELECT UTC_TIMESTAMP())
              WHERE Id = @Id;";

        private const string MarketMatchOrInsertSql = @"
            INSERT INTO MarketStockExchange(MarketId, MarketName)
            SELECT @MarketId, @MarketName
            FROM DUAL
            WHERE NOT EXISTS(
                SELECT 1
                FROM MarketStockExchange
                WHERE MarketId = @MarketId)
            LIMIT 1;

            SELECT Id FROM MarketStockExchange WHERE MarketId = @MarketId;";

        private const string SecurityMatchOrInsertSql = @"
            INSERT INTO MarketStockExchangeSecurities(MarketStockExchangeId, ClientIdentifier, Sedol, Isin, Figi, Cusip, Lei, ExchangeSymbol, BloombergTicker, SecurityName, Cfi, IssuerIdentifier, SecurityCurrency, ReddeerId)
            SELECT @MarketIdPrimaryKey, @ClientIdentifier, @Sedol, @Isin, @Figi, @Cusip, @Lei, @ExchangeSymbol, @BloombergTicker, @SecurityName, @Cfi, @IssuerIdentifier, @SecurityCurrency, @ReddeerId
            FROM DUAL
            WHERE NOT EXISTS(
	            SELECT 1
	            FROM MarketStockExchangeSecurities
	            WHERE Sedol = @Sedol
                Or (Isin = @Isin and MarketStockExchangeId = @MarketIdPrimaryKey))
            LIMIT 1;

            SELECT Id FROM MarketStockExchangeSecurities WHERE Sedol = @sedol or (Isin = @Isin and MarketStockExchangeId = @MarketIdPrimaryKey);";

        public ReddeerMarketRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<ReddeerMarketRepository> logger)
        {
            _dbConnectionFactory =
                dbConnectionFactory
                ?? throw new ArgumentNullException(nameof(dbConnectionFactory));

            _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<SecurityEnrichmentDto>> GetUnEnrichedSecurities()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<SecurityEnrichmentDto>(GetUnEnrichedSecuritiesSql))
                {
                    var result = await conn;

                    return result?.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerMarketRepository GetUnEnrichedSecurities method for {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new SecurityEnrichmentDto[0];
        }

        public async Task UpdateUnEnrichedSecurities(IReadOnlyCollection<SecurityEnrichmentDto> dtos)
        {
            if (dtos == null
                || !dtos.Any())
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.ExecuteAsync(UpdateUnEnrichedSecuritiesSql, dtos))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerMarketRepository UpdateUnEnrichedSecurities method for {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task Create(ExchangeFrame entity)
        {
            if (entity == null)
            {
                return;
            }

            lock (_lock)
            {

                var dbConnection = _dbConnectionFactory.BuildConn();

                try
                {
                    dbConnection.Open();

                    using (var conn = dbConnection.ExecuteAsync(CreateTmp))
                    {
                        conn.Wait();
                    }

                    if (!entity.Securities?.Any() ?? true)
                    {
                        return;
                    }

                    var projectedSecurities = entity.Securities.Select(Project).ToList();
                    using (var conn = dbConnection.ExecuteAsync(InsertIntoTmp, projectedSecurities))
                     {
                        conn.Wait();
                    }

                    using (var conn = dbConnection.ExecuteAsync(CopyAcrossMarketAndSecurityData))
                    {
                        conn.Wait();
                    }

                    using (var conn = dbConnection.ExecuteAsync(InsertSecuritySql))
                    {
                        conn.Wait();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"ReddeerMarketRepository Create Method For {entity.Exchange?.Name} {e.Message}");
                }
                finally
                {
                    dbConnection.Close();
                    dbConnection.Dispose();
                }
            }
        }

        public async Task<IReadOnlyCollection<ExchangeFrame>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx)
        {
            start = start.Date;
            end = end.Date;

            if (start > end)
            {
                return new ExchangeFrame[0];
            }

            if (start == end)
            {
                end = end.AddDays(1).AddMilliseconds(-1);
            }

            var dbConnection = _dbConnectionFactory.BuildConn();
            var query = new GetQuery { Start = start, End = end };

            try
            {
                dbConnection.Open();

                using (var conn = dbConnection.QueryAsync<MarketStockExchangeSecuritiesDto>(GetMarketSql, query))
                {
                    var response = await conn;

                    var groupedByExchange =
                        response
                            .GroupBy(rep =>
                                new { rep.MarketId, Mic = rep.MarketIdentifierCode, rep.MarketName, rep.Epoch},
                                (key, group) => new
                                {
                                    Key1 = key.MarketId,
                                    Key2 = key.MarketName,
                                    Key3 = key.Epoch,
                                    Key4 = key.Mic,
                                    Result = group.ToList()
                                })
                            .Select(i =>
                            {
                                var market = new DomainV2.Financial.Market(i.Key1, i.Key4, i.Key2, MarketTypes.STOCKEXCHANGE);
                                var frame =
                                    new ExchangeFrame(
                                        market,
                                        i.Key3,
                                        i.Result.Select(o => ProjectToSecurity(o, market)).ToList());

                                    return frame;
                                });

                    return groupedByExchange.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerMarketRepository Get Method For {start} {end} {e.Message}");
                opCtx.EventError(e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ExchangeFrame[0];
        }

        /// <summary>
        /// Trades consist of market data plus trade data
        /// Try to get the security id if we already know it
        /// Otherwise create a new entry and provide the market ids and security ids
        /// Future caching possibility if you're investigating slow inserts
        /// </summary>
        public async Task<MarketSecurityIds> CreateAndOrGetSecurityId(MarketDataPair pair)
        {
            if (pair == null)
            {
                _logger.LogError("Reddeer Market Repository CreateAndOrGetSecurityId was passed a null market data pair.");
                return new MarketSecurityIds();
            }

            if (!string.IsNullOrWhiteSpace(pair.Security?.Identifiers.Id)
                && !string.IsNullOrWhiteSpace(pair.Exchange?.Id))
            {
                return new MarketSecurityIds { MarketId = pair.Exchange.Id, SecurityId = pair.Security?.Identifiers.Id};
            }

            if (pair.Exchange == null
                || string.IsNullOrWhiteSpace(pair.Exchange?.MarketIdentifierCode))
            {
                _logger.LogError("Reddeer Market Repository CreateAndOrGetSecurityId either received a null exchange object or an empty market id for a trade. Not able to continue.");
                return new MarketSecurityIds();
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var marketId = string.Empty;
                var securityId = string.Empty;

                var marketUpdate = new MarketUpdateDto(pair.Exchange);
                using (var conn = dbConnection.ExecuteScalarAsync<string>(MarketMatchOrInsertSql, marketUpdate))
                {
                    marketId = await conn;                 
                }

                var securityUpdate = new InsertSecurityDto(pair.Security, marketId);
                using (var conn = dbConnection.ExecuteScalarAsync<string>(SecurityMatchOrInsertSql, securityUpdate))
                {
                    securityId = await conn;
                }

                return new MarketSecurityIds {MarketId = marketId, SecurityId = securityId};
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerMarketRepository CreateAndOrGetSecurityId {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new MarketSecurityIds();
        }

        public class MarketSecurityIds
        {
            public string MarketId { get; set; } = string.Empty;
            public string SecurityId { get; set; } = string.Empty;
        }

        private class MarketUpdateDto
        {
            public MarketUpdateDto()
            { }

            public MarketUpdateDto(DomainV2.Financial.Market market)
            {
                MarketId = market.MarketIdentifierCode;
                MarketName = market.Name;
            }

            // ReSharper disable MemberCanBePrivate.Local
            public string MarketId { get; set; }
            public string MarketName { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
        }

        private SecurityTick ProjectToSecurity(MarketStockExchangeSecuritiesDto dto, DomainV2.Financial.Market market)
        {
            if (dto == null)
            {
                return null;
            }

            var security =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    new InstrumentIdentifiers(
                        dto.Id,
                        dto.ReddeerId,
                        dto.ClientIdentifier,
                        dto.Sedol,
                        dto.Isin,
                        dto.Figi,
                        dto.Cusip,
                        dto.ExchangeSymbol,
                        dto.Lei,
                        dto.BloombergTicker),
                    dto.SecurityName,
                    dto.Cfi,
                    dto.IssuerIdentifier);

            var spread =
                new Spread(
                    new CurrencyAmount(dto.BidPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.AskPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.MarketPrice.GetValueOrDefault(0), dto.SecurityCurrency));

            var intradayPrices =
                new IntradayPrices(
                    new CurrencyAmount(dto.OpenPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.ClosePrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.HighIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.LowIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency));

            var tick =
                new SecurityTick(
                    security,
                    spread,
                    new Volume(dto.VolumeTradedInTick.GetValueOrDefault(0)),
                    new Volume(dto.DailyVolume.GetValueOrDefault(0)),
                    dto.Epoch,
                    dto.MarketCap,
                    intradayPrices,
                    dto.ListedSecurities,
                    market);

            return tick;
        }

        private class GetQuery
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }


        private class MarketStockExchangeDto
        {
            public MarketStockExchangeDto()
            { }

            public MarketStockExchangeDto(ExchangeFrame entity)
            {
                if (entity == null)
                {
                    return;
                }

                MarketId = entity?.Exchange?.MarketIdentifierCode;
                MarketName = entity?.Exchange?.Name;
            }

            // ReSharper disable MemberCanBePrivate.Local
            public int Id { get; set; }
            public string MarketId { get; set; }
            public string MarketName { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
        }

        private class MarketStockExchangeSecuritiesDto
        {
            public MarketStockExchangeSecuritiesDto()
            { }

            public MarketStockExchangeSecuritiesDto(SecurityTick entity, int marketStockExchangeId)
            {
                if (entity == null)
                {
                    return;
                }

                MarketStockExchangeId = marketStockExchangeId;
                ClientIdentifier = entity.Security?.Identifiers.ClientIdentifier;
                ReddeerId = entity.Security?.Identifiers.ReddeerId;
                Sedol = entity.Security?.Identifiers.Sedol;
                Isin = entity.Security?.Identifiers.Isin;
                Figi = entity.Security?.Identifiers.Figi;
                Cusip = entity.Security?.Identifiers.Cusip;
                Lei = entity.Security?.Identifiers.Lei;
                ExchangeSymbol = entity.Security?.Identifiers.ExchangeSymbol;
                BloombergTicker = entity.Security?.Identifiers.BloombergTicker;
                SecurityName = entity.Security?.Name;
                Cfi = entity.Security?.Cfi;
                IssuerIdentifier = entity.Security?.IssuerIdentifier;
                SecurityCurrency =
                    entity.Spread.Price.Currency.Value
                    ?? entity.Spread.Ask.Currency.Value
                    ?? entity.Spread.Bid.Currency.Value;

                Epoch = entity.TimeStamp;
                BidPrice = entity.Spread.Bid.Value;
                AskPrice = entity.Spread.Ask.Value;
                MarketPrice = entity.Spread.Price.Value;
                OpenPrice = entity.IntradayPrices.Open?.Value;
                ClosePrice = entity.IntradayPrices.Close?.Value;
                HighIntradayPrice = entity.IntradayPrices.High?.Value;
                LowIntradayPrice = entity.IntradayPrices.Low?.Value;
                ListedSecurities = entity.ListedSecurities;
                MarketCap = entity.MarketCap;
                VolumeTradedInTick = entity.Volume.Traded;
                DailyVolume = entity.DailyVolume.Traded;
            }

            public string Id { get; set; }

            public int MarketStockExchangeId { get; set; }

            public string ClientIdentifier { get; set; }

            public string ReddeerId { get; set; }

            public string Sedol { get; set; }

            public string Isin { get; set; }

            public string Figi { get; set; }

            public string Cusip { get; set; }

            public string Lei { get; set; }

            public string ExchangeSymbol { get; set; }

            public string BloombergTicker { get; set; }

            public string SecurityName { get; set; }

            public string Cfi { get; set; }

            public string IssuerIdentifier { get; set; }

            public string SecurityCurrency { get; set; }





            public DateTime Epoch { get; set; }

            public decimal? BidPrice { get; set; }

            public decimal? AskPrice { get; set; }

            public decimal? MarketPrice { get; set; }

            public decimal? OpenPrice { get; set; }

            public decimal? ClosePrice { get; set; }

            public decimal? HighIntradayPrice { get; set; }

            public decimal? LowIntradayPrice { get; set; }

            public long? ListedSecurities { get; set; }

            public decimal? MarketCap { get; set; }

            public long? VolumeTradedInTick { get; set; }

            public long? DailyVolume { get; set; }



            // dont set these two for writes they're just for reads
            public string MarketId { get; set; }
            public string MarketIdentifierCode { get; set; }
            public string MarketName { get; set; }
        }

        private InsertSecurityDto Project(SecurityTick tick)
        {
            return new InsertSecurityDto(tick);
        }

        private class InsertSecurityDto
        {
            public InsertSecurityDto(SecurityTick entity)
            {
                if (entity == null)
                {
                    return;
                }

                Id = entity?.Security?.Identifiers.Id ?? string.Empty;
                MarketId = entity.Market?.MarketIdentifierCode;
                MarketName = entity.Market?.Name;
                ReddeerId = entity.Security?.Identifiers.ReddeerId;
                ClientIdentifier = entity.Security?.Identifiers.ClientIdentifier;
                Sedol = entity.Security?.Identifiers.Sedol;
                Isin = entity.Security?.Identifiers.Isin;
                Figi = entity.Security?.Identifiers.Figi;
                Cusip = entity.Security?.Identifiers.Cusip;
                Lei = entity.Security?.Identifiers.Lei;
                ExchangeSymbol = entity.Security?.Identifiers.ExchangeSymbol;
                BloombergTicker = entity.Security?.Identifiers.BloombergTicker;
                SecurityName = entity.Security?.Name;
                Cfi = entity.Security?.Cfi;
                IssuerIdentifier = entity.Security?.IssuerIdentifier;
                SecurityCurrency =
                    entity.Spread.Price.Currency.Value
                    ?? entity.Spread.Ask.Currency.Value
                    ?? entity.Spread.Bid.Currency.Value;
                Epoch = entity.TimeStamp;
                BidPrice = entity.Spread.Bid.Value;
                AskPrice = entity.Spread.Ask.Value;
                MarketPrice = entity.Spread.Price.Value;
                OpenPrice = entity.IntradayPrices.Open?.Value;
                ClosePrice = entity.IntradayPrices.Close?.Value;
                HighIntradayPrice = entity.IntradayPrices.High?.Value;
                LowIntradayPrice = entity.IntradayPrices.Low?.Value;
                ListedSecurities = entity.ListedSecurities;
                MarketCap = entity.MarketCap;
                VolumeTradedInTick = entity.Volume.Traded;
                DailyVolume = entity.DailyVolume.Traded;
            }

            public InsertSecurityDto(FinancialInstrument security, string marketIdForeignKey)
            {
                MarketIdPrimaryKey = marketIdForeignKey;

                Id = security.Identifiers.Id;
                ReddeerId = security.Identifiers.ReddeerId;
                ClientIdentifier = security.Identifiers.ClientIdentifier;
                Sedol = security.Identifiers.Sedol;
                Isin = security.Identifiers.Isin;
                Figi = security.Identifiers.Figi;
                Cusip = security.Identifiers.Cusip;
                Lei = security.Identifiers.Lei;
                ExchangeSymbol = security.Identifiers.ExchangeSymbol;
                BloombergTicker = security.Identifiers.BloombergTicker;
                SecurityName = security.Name;
                Cfi = security.Cfi;
                IssuerIdentifier = security.IssuerIdentifier;
            }

            public string MarketIdPrimaryKey { get; set; }
            // This is the MIC
            public string MarketId { get; set; }
            public string MarketName { get; set; }

            public string ClientIdentifier { get; set; }

            public string Id { get; set; }

            public string ReddeerId { get; set; }

            public string Sedol { get; set; }

            public string Isin { get; set; }

            public string Figi { get; set; }

            public string Cusip { get; set; }

            public string Lei { get; set; }

            public string ExchangeSymbol { get; set; }

            public string BloombergTicker { get; set; }

            public string SecurityName { get; set; }

            public string Cfi { get; set; }

            public string IssuerIdentifier { get; set; }

            public string SecurityCurrency { get; set; }



            public DateTime Epoch { get; set; }

            public decimal? BidPrice { get; set; }

            public decimal? AskPrice { get; set; }

            public decimal? MarketPrice { get; set; }

            public decimal? OpenPrice { get; set; }

            public decimal? ClosePrice { get; set; }

            public decimal? HighIntradayPrice { get; set; }

            public decimal? LowIntradayPrice { get; set; }

            public long? ListedSecurities { get; set; }

            public decimal? MarketCap { get; set; }

            public long? VolumeTradedInTick { get; set; }

            public long? DailyVolume { get; set; }
        }
    }
}
