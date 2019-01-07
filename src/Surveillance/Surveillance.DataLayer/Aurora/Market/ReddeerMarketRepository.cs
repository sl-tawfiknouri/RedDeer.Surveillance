﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
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
        private readonly ICfiInstrumentTypeMapper _cfiMapper;
        private readonly ILogger<ReddeerMarketRepository> _logger;

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
             MSES.InstrumentType as InstrumentType,
             MSES.UnderlyingCfi as UnderlyingCfi,
             MSES.UnderlyingName as UnderlyingName,
             MSES.UnderlyingSedol as UnderlyingSedol,
             MSES.UnderlyingIsin as UnderlyingIsin,
             MSES.UnderlyingFigi as UnderlyingFigi,
             MSES.UnderlyingCusip as UnderlyingCusip,
             MSES.UnderlyingLei as UnderlyingLei,
             MSES.UnderlyingExchangeSymbol as UnderlyingExchangeSymbol,
             MSES.UnderlyingBloombergTicker as UnderlyingBloombergTicker,
             MSES.UnderlyingClientIdentifier as UnderlyingClientIdentifier,
             MSEP.Epoch as Epoch,
             MSEP.BidPrice as BidPrice,
             MSEP.AskPrice as AskPrice,
             MSEP.MarketPrice as MarketPrice,
             MSEP.VolumeTraded as VolumeTraded,
             IEDS.OpenPrice as OpenPrice,
             IEDS.ClosePrice as ClosePrice,
             IEDS.HighIntradayPrice as HighIntradayPrice,
             IEDS.LowIntradayPrice as LowIntradayPrice,
             IEDS.ListedSecurities as ListedSecurities,
             IEDS.MarketCap as MarketCap,
             IEDS.DailyVolume as DailyVolume
             FROM InstrumentEquityTimeBars AS MSEP
             LEFT OUTER JOIN InstrumentEquityDailySummary AS IEDS
             ON MSEP.SecurityId = IEDS.SecurityId AND date(MSEP.Epoch) = date(IEDS.Epoch) AND IEDS.Epoch >= MSEP.Epoch
             LEFT JOIN FinancialInstruments AS MSES
             ON MSEP.SecurityId = MSES.Id
             LEFT JOIN Market AS MSE
             ON MSES.MarketId = MSE.Id
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
              FROM FinancialInstruments as sec
              left join Market as mse
              on sec.MarketId = mse.Id
              WHERE sec.Enrichment is null
              LIMIT 10000;";

        private const string UpdateUnEnrichedSecuritiesSql =
            @"UPDATE FinancialInstruments
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
              InstrumentType = @InstrumentType,
              Enrichment = (SELECT UTC_TIMESTAMP())
              WHERE Id = @Id;";

        private const string MarketMatchOrInsertSql = @"
            INSERT INTO Market(MarketId, MarketName)
            SELECT @MarketId, @MarketName
            FROM DUAL
            WHERE NOT EXISTS(
                SELECT 1
                FROM Market
                WHERE MarketId = @MarketId)
            LIMIT 1;

            SELECT Id FROM Market WHERE MarketId = @MarketId;";

        private const string SecurityMatchOrInsertSql = @"
            INSERT INTO FinancialInstruments(
                MarketId,
                ClientIdentifier,
                Sedol,
                Isin,
                Figi,
                Cusip,
                Lei,
                ExchangeSymbol,
                BloombergTicker,
                SecurityName,
                Cfi,
                IssuerIdentifier,
                SecurityCurrency,
                ReddeerId,
                InstrumentType,
                UnderlyingCfi,
                UnderlyingName,
                UnderlyingSedol,
                UnderlyingIsin,
                UnderlyingFigi,
                UnderlyingCusip,
                UnderlyingLei,
                UnderlyingExchangeSymbol,
                UnderlyingBloombergTicker,
                UnderlyingClientIdentifier)
            SELECT @MarketIdPrimaryKey, @ClientIdentifier, @Sedol, @Isin, @Figi, @Cusip, @Lei, @ExchangeSymbol, @BloombergTicker, @SecurityName, @Cfi, @IssuerIdentifier, @SecurityCurrency, @ReddeerId, @InstrumentType, @UnderlyingCfi, @UnderlyingName, @UnderlyingSedol, @UnderlyingIsin,
                @UnderlyingFigi, @UnderlyingCusip, @UnderlyingLei, @UnderlyingExchangeSymbol, @UnderlyingBloombergTicker, @UnderlyingClientIdentifier
            FROM DUAL
            WHERE NOT EXISTS(
	            SELECT 1
	            FROM FinancialInstruments
	            WHERE Sedol = @Sedol
                Or (Isin = @Isin and MarketId = @MarketIdPrimaryKey))
            LIMIT 1;

            SELECT Id FROM FinancialInstruments WHERE Sedol = @sedol or (Isin = @Isin and MarketId = @MarketIdPrimaryKey);";

        private const string SecurityMatchOrInsertSqlv2 = @"
            INSERT INTO FinancialInstruments(
                MarketId,
                ClientIdentifier,
                Sedol,
                Isin,
                Figi,
                Cusip,
                Lei,
                ExchangeSymbol,
                BloombergTicker,
                SecurityName,
                Cfi,
                IssuerIdentifier,
                SecurityCurrency,
                ReddeerId,
                InstrumentType,
                UnderlyingCfi,
                UnderlyingName,
                UnderlyingSedol,
                UnderlyingIsin,
                UnderlyingFigi,
                UnderlyingCusip,
                UnderlyingLei,
                UnderlyingExchangeSymbol,
                UnderlyingBloombergTicker,
                UnderlyingClientIdentifier)
            SELECT @MarketIdPrimaryKey, @ClientIdentifier, @Sedol, @Isin, @Figi, @Cusip, @Lei, @ExchangeSymbol, @BloombergTicker, @SecurityName, @Cfi, @IssuerIdentifier, @SecurityCurrency, @ReddeerId, @InstrumentType, @UnderlyingCfi, @UnderlyingName, @UnderlyingSedol, @UnderlyingIsin,
                @UnderlyingFigi, @UnderlyingCusip, @UnderlyingLei, @UnderlyingExchangeSymbol, @UnderlyingBloombergTicker, @UnderlyingClientIdentifier
            FROM DUAL
            WHERE NOT EXISTS(
	            SELECT 1
	            FROM FinancialInstruments
	            WHERE Sedol = @Sedol
                Or (Isin = @Isin and MarketId = @MarketIdPrimaryKey))
            LIMIT 1;

            SELECT @FinancialInstrumentId2 := Id FROM FinancialInstruments WHERE Sedol = @sedol or (Isin = @Isin and MarketId = @MarketIdPrimaryKey) LIMIT 1;

             INSERT INTO InstrumentEquityTimeBars (SecurityId, Epoch, BidPrice, AskPrice, MarketPrice, VolumeTraded) VALUES (@FinancialInstrumentId2, @Epoch, @BidPrice, @AskPrice, @MarketPrice, @VolumeTraded);

             INSERT INTO InstrumentEquityDailySummary (SecurityId, Epoch, EpochDate, OpenPrice, ClosePrice, HighIntradayPrice, LowIntradayPrice, ListedSecurities, MarketCap, DailyVolume) VALUES (@FinancialInstrumentId2, @Epoch, @EpochDate, @OpenPrice, @ClosePrice, @HighIntradayPrice, @LowIntradayPrice, @ListedSecurities, @MarketCap, @DailyVolume);";

        public ReddeerMarketRepository(
            IConnectionStringFactory dbConnectionFactory,
            ICfiInstrumentTypeMapper cfiMapper,
            ILogger<ReddeerMarketRepository> logger)
        {
            _dbConnectionFactory =
                dbConnectionFactory
                ?? throw new ArgumentNullException(nameof(dbConnectionFactory));

            _cfiMapper =
                cfiMapper
                ?? throw new ArgumentNullException(nameof(cfiMapper));

            _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<SecurityEnrichmentDto>> GetUnEnrichedSecurities()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                _logger.LogInformation($"ReddeerMarketRepository opening db connection for unenriched securities");

                dbConnection.Open();
                _logger.LogInformation($"ReddeerMarketRepository getting unenriched securities");
                using (var conn = dbConnection.QueryAsync<SecurityEnrichmentDto>(GetUnEnrichedSecuritiesSql))
                {
                    var result = await conn;

                    _logger.LogInformation($"ReddeerMarketRepository returning {result?.Count() ?? 0} unenriched securities");
                    return result?.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerMarketRepository GetUnEnrichedSecurities method for {e.Message}");
            }
            finally
            {
                _logger.LogInformation($"ReddeerMarketRepository closing db connection for unenriched securities");

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
                _logger.LogInformation($"ReddeerMarketRepository update unenriched securities received none");

                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();
                _logger.LogInformation($"ReddeerMarketRepository opening db connection in update unenriched securities");

                var projectedDtos = dtos.Select(dto => new SecurityEnrichmentDtoDapper(dto, _cfiMapper)).ToList();

                _logger.LogInformation($"ReddeerMarketRepository about to update {projectedDtos.Count} unenriched securities");
                using (var conn = dbConnection.ExecuteAsync(UpdateUnEnrichedSecuritiesSql, projectedDtos))
                {
                    await conn;
                    _logger.LogInformation($"ReddeerMarketRepository completed updating unenriched securities");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerMarketRepository UpdateUnEnrichedSecurities method for {e.Message}");
            }
            finally
            {
                _logger.LogInformation($"ReddeerMarketRepository closing db connection for update unenriched securities");

                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task Create(MarketTimeBarCollection entity)
        {
            if (entity == null)
            {
                _logger.LogInformation($"ReddeerMarketRepository create received a null entity");

                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                _logger.LogInformation($"ReddeerMarketRepository opened connection to the database");
                dbConnection.Open();

                if (!entity.Securities?.Any() ?? true)
                {
                    _logger.LogInformation($"ReddeerMarketRepository did not detect any securities for {entity.Epoch} - {entity.Exchange?.MarketIdentifierCode}");
                    return;
                }

                var marketId = string.Empty;
                var marketUpdate = new MarketUpdateDto(entity.Exchange);

                _logger.LogInformation($"ReddeerMarketRepository Create method about to write market match or insert sql for market {marketUpdate.MarketId} {marketUpdate.MarketName}");
                marketId = dbConnection.ExecuteScalar<string>(MarketMatchOrInsertSql, marketUpdate);
                _logger.LogInformation($"ReddeerMarketRepository Create method finished writing market match or insert sql for market {marketUpdate.MarketId} {marketUpdate.MarketName} and fetched id of {marketId}");

                _logger.LogInformation($"ReddeerMarketRepository about to write {entity.Securities?.Count} rows to database");
                foreach (var security in entity.Securities)
                {
                    var securityUpdate = new InsertSecurityDto(security, marketId, _cfiMapper);
                    _logger.LogInformation($"ReddeerMarketRepository about to write row to database {security.Security?.Identifiers} {security?.TimeStamp}");
                     dbConnection.Execute(SecurityMatchOrInsertSqlv2, securityUpdate);
                    _logger.LogInformation($"ReddeerMarketRepository finished writing row to database {security.Security?.Identifiers} {security?.TimeStamp}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerMarketRepository Create Method For {entity.Exchange?.Name} ERROR MESSAGE ({e.Message}) INNER ERROR MESSAGE ({e?.InnerException.Message})");
            }
            finally
            {
                _logger.LogInformation($"ReddeerMarketRepository closing db connection for create security");

                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<MarketTimeBarCollection>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx)
        {
            start = start.Date;
            end = end.Date;

            if (start > end)
            {
                _logger.LogWarning($"ReddeerMarketRepository Get request had a start date larger than end date {start} {end}");
                return new MarketTimeBarCollection[0];
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
                _logger.LogInformation($"ReddeerMarketRepository get opened connection to db");

                _logger.LogInformation($"ReddeerMarketRepository get about to query for {start} and {end}");
                using (var conn = dbConnection.QueryAsync<MarketStockExchangeSecuritiesDto>(GetMarketSql, query))
                {
                    var response = (await conn)?.ToList() ?? new List<MarketStockExchangeSecuritiesDto>();

                    _logger.LogInformation($"ReddeerMarketRepository get query for {start} and {end} received {response?.Count() ?? 0} results");

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
                                    new MarketTimeBarCollection(
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
                _logger.LogInformation($"ReddeerMarketRepository get closed connection to db");

                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new MarketTimeBarCollection[0];
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
                _logger.LogInformation("Reddeer Market Repository CreateAndOrGetSecurityId opened connection to database");

                var marketId = string.Empty;
                var securityId = string.Empty;

                var marketUpdate = new MarketUpdateDto(pair.Exchange);

                _logger.LogInformation($"Reddeer Market Repository CreateAndOrGetSecurityId about to match market or insert sql for {pair.Exchange?.MarketIdentifierCode}");
                using (var conn = dbConnection.ExecuteScalarAsync<string>(MarketMatchOrInsertSql, marketUpdate))
                {
                    marketId = await conn;
                    _logger.LogInformation($"Reddeer Market Repository CreateAndOrGetSecurityId match market or insert sql for {pair.Exchange?.MarketIdentifierCode}");
                }

                var securityUpdate = new InsertSecurityDto(pair.Security, marketId, _cfiMapper);

                _logger.LogInformation($"Reddeer Market Repository CreateAndOrGetSecurityId about to match security or insert sql for {pair.Security?.Name}");
                using (var conn = dbConnection.ExecuteScalarAsync<string>(SecurityMatchOrInsertSql, securityUpdate))
                {
                    securityId = await conn;
                    _logger.LogInformation($"Reddeer Market Repository CreateAndOrGetSecurityId completed match security or insert sql for {pair.Security?.Name}");
                }

                return new MarketSecurityIds {MarketId = marketId, SecurityId = securityId};
            }
            catch (Exception e)
            {
                _logger.LogError($"ReddeerMarketRepository CreateAndOrGetSecurityId {e.Message} {e.InnerException?.Message}");
            }
            finally
            {
                _logger.LogInformation("Reddeer Market Repository CreateAndOrGetSecurityId closed connection to database");

                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new MarketSecurityIds();
        }

        public class MarketSecurityIds
        {
            // primary key
            public string MarketId { get; set; } = string.Empty;

            // primary key
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

        private FinancialInstrumentTimeBar ProjectToSecurity(MarketStockExchangeSecuritiesDto dto, DomainV2.Financial.Market market)
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
                        dto.ReddeerEnrichmentId,
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
                    dto.SecurityCurrency,
                    dto.IssuerIdentifier);

            var spread =
                new SpreadTimeBar(
                    new CurrencyAmount(dto.BidPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.AskPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.MarketPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Volume(dto.VolumeTraded.GetValueOrDefault(0)));

            var intradayPrices =
                new IntradayPrices(
                    new CurrencyAmount(dto.OpenPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.ClosePrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.HighIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new CurrencyAmount(dto.LowIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency));

            var dailySummary =
                new DailySummaryTimeBar(
                    dto.MarketCap,
                    intradayPrices,
                    dto.ListedSecurities,
                    new Volume(dto.DailyVolume.GetValueOrDefault(0)),
                    dto.Epoch);

            var tick =
                new FinancialInstrumentTimeBar(
                    security,
                    spread,
                    dailySummary,
                    dto.Epoch,
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

            public MarketStockExchangeDto(MarketTimeBarCollection entity)
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

            public MarketStockExchangeSecuritiesDto(FinancialInstrumentTimeBar entity, int marketId, ICfiInstrumentTypeMapper cfiMapper)
            {
                if (entity == null)
                {
                    return;
                }

                ClientIdentifier = entity.Security?.Identifiers.ClientIdentifier;
                ReddeerId = entity.Security?.Identifiers.ReddeerId;
                ReddeerEnrichmentId = entity.Security?.Identifiers.ReddeerEnrichmentId;
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
                    entity.SpreadTimeBar.Price.Currency.Value
                    ?? entity.SpreadTimeBar.Ask.Currency.Value
                    ?? entity.SpreadTimeBar.Bid.Currency.Value;

                Epoch = entity.TimeStamp;
                EpochDate = entity.TimeStamp.Date;
                BidPrice = entity.SpreadTimeBar.Bid.Value;
                AskPrice = entity.SpreadTimeBar.Ask.Value;
                MarketPrice = entity.SpreadTimeBar.Price.Value;
                OpenPrice = entity.DailySummaryTimeBar.IntradayPrices.Open?.Value;
                ClosePrice = entity.DailySummaryTimeBar.IntradayPrices.Close?.Value;
                HighIntradayPrice = entity.DailySummaryTimeBar.IntradayPrices.High?.Value;
                LowIntradayPrice = entity.DailySummaryTimeBar.IntradayPrices.Low?.Value;
                ListedSecurities = entity.DailySummaryTimeBar.ListedSecurities;
                MarketCap = entity.DailySummaryTimeBar.MarketCap;
                VolumeTraded = entity.SpreadTimeBar.Volume.Traded;
                DailyVolume = entity.DailySummaryTimeBar.DailyVolume.Traded;
                MarketId = marketId.ToString();
                InstrumentType = (int)cfiMapper.MapCfi(entity.Security?.Cfi);

                UnderlyingCfi = entity?.Security?.UnderlyingCfi;
                UnderlyingName = entity?.Security?.UnderlyingName;
                UnderlyingSedol = entity?.Security?.Identifiers.UnderlyingSedol;
                UnderlyingIsin = entity?.Security?.Identifiers.UnderlyingIsin;
                UnderlyingFigi = entity?.Security?.Identifiers.UnderlyingFigi;
                UnderlyingCusip = entity?.Security?.Identifiers.UnderlyingCusip;
                UnderlyingLei = entity?.Security?.Identifiers.UnderlyingLei;
                UnderlyingExchangeSymbol = entity?.Security?.Identifiers.UnderlyingExchangeSymbol;
                UnderlyingBloombergTicker = entity?.Security?.Identifiers.UnderlyingBloombergTicker;
                UnderlyingClientIdentifier = entity?.Security?.Identifiers.UnderlyingClientIdentifier;
            }

            public string Id { get; set; }

            public string ClientIdentifier { get; set; }

            public string ReddeerId { get; set; }

            public string ReddeerEnrichmentId { get; set; }

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

            public int InstrumentType { get; set; }


            public string UnderlyingCfi { get; set; }
            public string UnderlyingName { get; set; }
            public string UnderlyingSedol { get; set; }
            public string UnderlyingIsin { get; set; }
            public string UnderlyingFigi { get; set; }
            public string UnderlyingCusip { get; set; }
            public string UnderlyingLei { get; set; }
            public string UnderlyingExchangeSymbol { get; set; }
            public string UnderlyingBloombergTicker { get; set; }
            public string UnderlyingClientIdentifier { get; set; }


            public DateTime Epoch { get; set; }
            public DateTime EpochDate { get; set; }

            public decimal? BidPrice { get; set; }

            public decimal? AskPrice { get; set; }

            public decimal? MarketPrice { get; set; }

            public decimal? OpenPrice { get; set; }

            public decimal? ClosePrice { get; set; }

            public decimal? HighIntradayPrice { get; set; }

            public decimal? LowIntradayPrice { get; set; }

            public long? ListedSecurities { get; set; }

            public decimal? MarketCap { get; set; }

            public long? VolumeTraded { get; set; }

            public long? DailyVolume { get; set; }



            // dont set these two for writes they're just for reads
            public string MarketId { get; set; }
            public string MarketIdentifierCode { get; set; }
            public string MarketName { get; set; }
        }

        private InsertSecurityDto Project(FinancialInstrumentTimeBar tick)
        {
            return new InsertSecurityDto(tick, null, _cfiMapper);
        }

        private class InsertSecurityDto
        {
            public InsertSecurityDto(FinancialInstrumentTimeBar entity, string marketIdForeignKey, ICfiInstrumentTypeMapper cfiMapper)
            {
                if (entity == null)
                {
                    return;
                }

                Id = entity?.Security?.Identifiers.Id ?? string.Empty;
                MarketIdPrimaryKey = marketIdForeignKey;
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
                    entity.SpreadTimeBar.Price.Currency.Value
                    ?? entity.SpreadTimeBar.Ask.Currency.Value
                    ?? entity.SpreadTimeBar.Bid.Currency.Value
                    ?? entity.SpreadTimeBar.Price.Currency.Value;
                Epoch = entity.TimeStamp;
                BidPrice = entity.SpreadTimeBar.Bid.Value;
                AskPrice = entity.SpreadTimeBar.Ask.Value;
                MarketPrice = entity.SpreadTimeBar.Price.Value;
                OpenPrice = entity.DailySummaryTimeBar.IntradayPrices.Open?.Value;
                ClosePrice = entity.DailySummaryTimeBar.IntradayPrices.Close?.Value;
                HighIntradayPrice = entity.DailySummaryTimeBar.IntradayPrices.High?.Value;
                LowIntradayPrice = entity.DailySummaryTimeBar.IntradayPrices.Low?.Value;
                ListedSecurities = entity.DailySummaryTimeBar.ListedSecurities;
                MarketCap = entity.DailySummaryTimeBar.MarketCap;
                VolumeTraded = entity.SpreadTimeBar.Volume.Traded;
                DailyVolume = entity.DailySummaryTimeBar.DailyVolume.Traded;
                InstrumentType = (int)cfiMapper.MapCfi(entity.Security?.Cfi);
                EpochDate = entity.TimeStamp.Date;

                UnderlyingCfi = entity?.Security?.UnderlyingCfi;
                UnderlyingName = entity?.Security?.UnderlyingName;
                UnderlyingSedol = entity?.Security?.Identifiers.UnderlyingSedol;
                UnderlyingIsin = entity?.Security?.Identifiers.UnderlyingIsin;
                UnderlyingFigi = entity?.Security?.Identifiers.UnderlyingFigi;
                UnderlyingCusip = entity?.Security?.Identifiers.UnderlyingCusip;
                UnderlyingLei = entity?.Security?.Identifiers.UnderlyingLei;
                UnderlyingExchangeSymbol = entity?.Security?.Identifiers.UnderlyingExchangeSymbol;
                UnderlyingBloombergTicker = entity?.Security?.Identifiers.UnderlyingBloombergTicker;
                UnderlyingClientIdentifier = entity?.Security?.Identifiers.UnderlyingClientIdentifier;
            }

            public InsertSecurityDto(FinancialInstrument security, string marketIdForeignKey, ICfiInstrumentTypeMapper cfiMapper)
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
                InstrumentType = (int)cfiMapper.MapCfi(security.Cfi);
                SecurityCurrency = security.SecurityCurrency;

                UnderlyingCfi = security.UnderlyingCfi;
                UnderlyingName = security.UnderlyingName;
                UnderlyingSedol = security.Identifiers.UnderlyingSedol;
                UnderlyingIsin = security.Identifiers.UnderlyingIsin;
                UnderlyingFigi = security.Identifiers.UnderlyingFigi;
                UnderlyingCusip = security.Identifiers.UnderlyingCusip;
                UnderlyingLei = security.Identifiers.UnderlyingLei;
                UnderlyingExchangeSymbol = security.Identifiers.UnderlyingExchangeSymbol;
                UnderlyingBloombergTicker = security.Identifiers.UnderlyingBloombergTicker;
                UnderlyingClientIdentifier = security.Identifiers.UnderlyingClientIdentifier;
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
            public int InstrumentType { get; set; }
            
            public string UnderlyingCfi { get; set; }
            public string UnderlyingName { get; set; }
            public string UnderlyingSedol { get; set; }
            public string UnderlyingIsin { get; set; }
            public string UnderlyingFigi { get; set; }
            public string UnderlyingCusip { get; set; }
            public string UnderlyingLei { get; set; }
            public string UnderlyingExchangeSymbol { get; set; }
            public string UnderlyingBloombergTicker { get; set; }
            public string UnderlyingClientIdentifier { get; set; }

            public string FinancialInstrumentId { get; set; }

            public DateTime Epoch { get; set; }
            public DateTime EpochDate { get; set; }

            public decimal? BidPrice { get; set; }

            public decimal? AskPrice { get; set; }

            public decimal? MarketPrice { get; set; }

            public decimal? OpenPrice { get; set; }

            public decimal? ClosePrice { get; set; }

            public decimal? HighIntradayPrice { get; set; }

            public decimal? LowIntradayPrice { get; set; }

            public long? ListedSecurities { get; set; }

            public decimal? MarketCap { get; set; }

            public long? VolumeTraded { get; set; }

            public long? DailyVolume { get; set; }
        }

        /// <summary>
        /// Just extend the dto with the InstrumentType field calculated from a potentially enriched CFI value
        /// </summary>
        public class SecurityEnrichmentDtoDapper
        {
            public SecurityEnrichmentDtoDapper()
            {
                // leave blank ctor
            }

            public SecurityEnrichmentDtoDapper(SecurityEnrichmentDto dto, ICfiInstrumentTypeMapper cfiMapper)
            {
                if (dto == null)
                {
                    return;
                }

                if (cfiMapper == null)
                {
                    throw new ArgumentNullException(nameof(cfiMapper));
                }

                Id = dto.Id;
                ReddeerId = dto.ReddeerId;
                MarketIdentifierCode = dto.MarketIdentifierCode;
                MarketName = dto.MarketName;
                SecurityName = dto.SecurityName;
                Cfi = dto.Cfi;
                IssuerIdentifier = dto.IssuerIdentifier;
                SecurityClientIdentifier = dto.SecurityClientIdentifier;
                Sedol = dto.Sedol;
                Isin = dto.Isin;
                Figi = dto.Figi;
                ExchangeSymbol = dto.ExchangeSymbol;
                Cusip = dto.Cusip;
                Lei = dto.Lei;
                BloombergTicker = dto.BloombergTicker;
                InstrumentType = (int)cfiMapper.MapCfi(dto.Cfi);
            }

            public string Id { get; set; }
            public string ReddeerId { get; set; }
            public string MarketIdentifierCode { get; set; }
            public string MarketName { get; set; }
            public string SecurityName { get; set; }
            public string Cfi { get; set; }
            public string IssuerIdentifier { get; set; }
            public string SecurityClientIdentifier { get; set; }
            public string Sedol { get; set; }
            public string Isin { get; set; }
            public string Figi { get; set; }
            public string ExchangeSymbol { get; set; }
            public string Cusip { get; set; }
            public string Lei { get; set; }
            public string BloombergTicker { get; set; }
            public int InstrumentType { get; set; }
        }
    }
}
