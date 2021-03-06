﻿namespace Surveillance.DataLayer.Aurora.Market
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Cfis.Interfaces;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Microsoft.Extensions.Logging;
    using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;
    
    public class ReddeerMarketRepository : IReddeerMarketRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ICfiInstrumentTypeMapper _cfiMapper;
        private readonly ILogger<ReddeerMarketRepository> _logger;

        private const string GetEquityIntraDaySql =
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
             MSES.Ric as Ric,
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
             MSES.UnderlyingRic as UnderlyingRic,
             MSES.SectorCode as SectorCode,
             MSES.IndustryCode as IndustryCode,
             MSES.RegionCode as RegionCode,
             MSES.CountryCode as CountryCode,
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
             IEDS.DailyVolume as DailyVolume,
             IEDS.Currency AS MarketCapCurrency
             FROM InstrumentEquityTimeBars AS MSEP
             LEFT OUTER JOIN InstrumentEquityDailySummary AS IEDS
             ON MSEP.SecurityId = IEDS.SecurityId AND date(MSEP.Epoch) = date(IEDS.Epoch) AND IEDS.Epoch >= MSEP.Epoch
             LEFT OUTER JOIN FinancialInstruments AS MSES
             ON MSEP.SecurityId = MSES.Id
             LEFT OUTER JOIN Market AS MSE
             ON MSES.MarketId = MSE.Id
             WHERE date(MSEP.Epoch) >= date(@start)
             AND date(MSEP.Epoch) <= date(@end);";

        private const string GetEquityInterDaySql =
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
             MSES.Ric as Ric,
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
             MSES.UnderlyingRic as UnderlyingRic,
             MSES.SectorCode as SectorCode,
             MSES.IndustryCode as IndustryCode,
             MSES.RegionCode as RegionCode,
             MSES.CountryCode as CountryCode,
             IEDS.Epoch as Epoch,
             IEDS.OpenPrice as OpenPrice,
             IEDS.ClosePrice as ClosePrice,
             IEDS.HighIntradayPrice as HighIntradayPrice,
             IEDS.LowIntradayPrice as LowIntradayPrice,
             IEDS.ListedSecurities as ListedSecurities,
             IEDS.MarketCap as MarketCap,
             IEDS.DailyVolume as DailyVolume,
             IEDS.Currency AS MarketCapCurrency
             FROM InstrumentEquityDailySummary AS IEDS
             LEFT OUTER JOIN FinancialInstruments AS MSES
             ON IEDS.SecurityId = MSES.Id
             LEFT OUTER JOIN Market AS MSE
             ON MSES.MarketId = MSE.Id
             WHERE date(IEDS.Epoch) >= date(@start)
             AND date(IEDS.Epoch) <= date(@end);";

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
              sec.BloombergTicker As BloombergTicker,
              sec.Ric as Ric,
              sec.SectorCode As SectorCode,
              sec.IndustryCode As IndustryCode,
              sec.RegionCode As RegionCode,
              sec.CountryCode As CountryCode
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
              Ric = @Ric,
              InstrumentType = @InstrumentType,
              SectorCode = @SectorCode,
              IndustryCode = @IndustryCode,
              RegionCode = @RegionCode,
              CountryCode = @CountryCode,
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
                Ric,
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
                UnderlyingClientIdentifier,
                UnderlyingRic,
                SectorCode,
                IndustryCode,
                RegionCode,
                CountryCode)
            SELECT @MarketIdPrimaryKey, @ClientIdentifier, @Sedol, @Isin, @Figi, @Cusip, @Lei, @ExchangeSymbol, @BloombergTicker, @Ric, @SecurityName, @Cfi, @IssuerIdentifier, @SecurityCurrency, @ReddeerId, @InstrumentType, @UnderlyingCfi, @UnderlyingName, @UnderlyingSedol, @UnderlyingIsin,
                @UnderlyingFigi, @UnderlyingCusip, @UnderlyingLei, @UnderlyingExchangeSymbol, @UnderlyingBloombergTicker, @UnderlyingClientIdentifier, @UnderlyingRic, @SectorCode, @IndustryCode, @RegionCode, @CountryCode
            FROM DUAL
            WHERE NOT EXISTS(
	            SELECT 1
	            FROM FinancialInstruments
	            WHERE 
                    (Sedol = @Sedol AND Sedol <> '' AND Sedol Is Not Null)
                    OR
                    (Isin = @Isin and MarketId = @MarketIdPrimaryKey AND Isin <> '' AND Isin Is Not Null)
                    OR 
                    (Ric = @Ric AND  Ric <> '' AND Ric Is Not Null)
                )
            LIMIT 1;

            SELECT Id 
            FROM FinancialInstruments 
            WHERE 
                (Sedol = @sedol AND Sedol <> '' AND Sedol IS NOT NULL)
                OR 
                (Isin = @Isin and MarketId = @MarketIdPrimaryKey AND Isin <> '' AND Isin IS NOT NULL)
                OR
                (Ric = @Ric AND  Ric <> '' AND Ric Is Not Null);";

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
                Ric,
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
                UnderlyingClientIdentifier,
                UnderlyingRic,
                SectorCode,
                IndustryCode,
                RegionCode,
                CountryCode)
            SELECT @MarketIdPrimaryKey, @ClientIdentifier, @Sedol, @Isin, @Figi, @Cusip, @Lei, @ExchangeSymbol, @BloombergTicker, @Ric, @SecurityName, @Cfi, @IssuerIdentifier, @SecurityCurrency, @ReddeerId, @InstrumentType, @UnderlyingCfi, @UnderlyingName, @UnderlyingSedol, @UnderlyingIsin,
                @UnderlyingFigi, @UnderlyingCusip, @UnderlyingLei, @UnderlyingExchangeSymbol, @UnderlyingBloombergTicker, @UnderlyingClientIdentifier, @UnderlyingRic, @SectorCode, @IndustryCode, @RegionCode, @CountryCode
            FROM DUAL
            WHERE NOT EXISTS
            (
	            SELECT 1
	            FROM FinancialInstruments
	            WHERE Sedol = @Sedol
                    Or (Isin = @Isin and MarketId = @MarketIdPrimaryKey)
                    OR Ric = @Ric
            )
            LIMIT 1;

            (
                SELECT @FinancialInstrumentId2 := Id FROM FinancialInstruments WHERE Sedol = @sedol AND Sedol <> '' AND Sedol Is Not Null
                UNION
                SELECT @FinancialInstrumentId2 := Id FROM FinancialInstruments WHERE (Isin = @Isin and MarketId = @MarketIdPrimaryKey AND Isin <> '' AND Isin Is Not Null)
                UNION
                SELECT @FinancialInstrumentId2 := Id FROM FinancialInstruments WHERE (Ric = @Ric AND Ric <> '' AND Ric Is Not Null)
            )
            LIMIT 1;

             INSERT IGNORE INTO InstrumentEquityTimeBars (SecurityId, Epoch, BidPrice, AskPrice, MarketPrice, VolumeTraded) VALUES (@FinancialInstrumentId2, @Epoch, @BidPrice, @AskPrice, @MarketPrice, @VolumeTraded);

             INSERT IGNORE INTO InstrumentEquityDailySummary (SecurityId, Epoch, EpochDate, OpenPrice, ClosePrice, HighIntradayPrice, LowIntradayPrice, ListedSecurities, MarketCap, DailyVolume) VALUES (@FinancialInstrumentId2, @Epoch, @EpochDate, @OpenPrice, @ClosePrice, @HighIntradayPrice, @LowIntradayPrice, @ListedSecurities, @MarketCap, @DailyVolume);";

        public ReddeerMarketRepository(
            IConnectionStringFactory dbConnectionFactory,
            ICfiInstrumentTypeMapper cfiMapper,
            ILogger<ReddeerMarketRepository> logger)
        {
            this._dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._cfiMapper = cfiMapper ?? throw new ArgumentNullException(nameof(cfiMapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogError(e, $"ReddeerMarketRepository GetUnEnrichedSecurities method for {e.Message}");
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
                _logger.LogError(e, $"ReddeerMarketRepository UpdateUnEnrichedSecurities method for {e.Message}");
            }
            finally
            {
                _logger.LogInformation($"ReddeerMarketRepository closing db connection for update unenriched securities");

                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public void Create(EquityIntraDayTimeBarCollection entity)
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
                _logger.LogError(e, $"ReddeerMarketRepository Create Method For {entity.Exchange?.Name} ERROR MESSAGE ({e.Message}) INNER ERROR MESSAGE ({e?.InnerException.Message})");
            }
            finally
            {
                _logger.LogInformation($"ReddeerMarketRepository closing db connection for create security");

                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<EquityIntraDayTimeBarCollection>> GetEquityIntraday(
            DateTime start,
            DateTime end,
            ISystemProcessOperationContext opCtx)
        {
            start = start.Date;
            end = end.Date;

            if (start > end)
            {
                _logger.LogWarning($"ReddeerMarketRepository Get request had a start date larger than end date {start} {end}");
                return new EquityIntraDayTimeBarCollection[0];
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
                using (var conn = dbConnection.QueryAsync<MarketStockExchangeSecuritiesDto>(GetEquityIntraDaySql, query))
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
                                var market = new Domain.Core.Markets.Market(i.Key1, i.Key4, i.Key2, MarketTypes.STOCKEXCHANGE);
                                var frame =
                                    new EquityIntraDayTimeBarCollection(
                                        market,
                                        i.Key3,
                                        i.Result.Select(o => ProjectToSecurity(o, market)).Where(_ => _.SpreadTimeBar.Price.Value > 0).ToList());

                                    return frame;
                                });

                    return groupedByExchange.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"ReddeerMarketRepository Get Method For {start} {end} {e.Message}");
                opCtx.EventError(e);
            }
            finally
            {
                _logger.LogInformation($"ReddeerMarketRepository get closed connection to db");

                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new EquityIntraDayTimeBarCollection[0];
        }

        public async Task<IReadOnlyCollection<EquityInterDayTimeBarCollection>> GetEquityInterDay(
            DateTime start,
            DateTime end,
            ISystemProcessOperationContext opCtx)
        {
            start = start.Date;
            end = end.Date;

            if (start > end)
            {
                _logger.LogWarning($"ReddeerMarketRepository GetEquityInterDay request had a start date larger than end date {start} {end}");
                return new EquityInterDayTimeBarCollection[0];
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
                _logger.LogInformation($"ReddeerMarketRepository GetEquityInterDay opened connection to db");

                _logger.LogInformation($"ReddeerMarketRepository GetEquityInterDay about to query for {start} and {end}");
                using (var conn = dbConnection.QueryAsync<MarketStockExchangeSecuritiesDto>(GetEquityInterDaySql, query))
                {
                    var response = (await conn)?.ToList() ?? new List<MarketStockExchangeSecuritiesDto>();

                    _logger.LogInformation($"ReddeerMarketRepository GetEquityInterDay query for {start} and {end} received {response?.Count() ?? 0} results");

                    var groupedByExchange =
                        response
                            .GroupBy(rep =>
                                new { rep.MarketId, Mic = rep.MarketIdentifierCode, rep.MarketName, rep.Epoch },
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
                                var market = new Domain.Core.Markets.Market(i.Key1, i.Key4, i.Key2, MarketTypes.STOCKEXCHANGE);
                                var frame =
                                    new EquityInterDayTimeBarCollection(
                                        market,
                                        i.Key3,
                                        i.Result.Select(o => ProjectToInterDaySecurity(o, market)).ToList());

                                return frame;
                            });

                    return groupedByExchange.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"ReddeerMarketRepository GetEquityInterDay Method For {start} {end} {e.Message}");
                opCtx.EventError(e);
            }
            finally
            {
                _logger.LogInformation($"ReddeerMarketRepository GetEquityInterDay closed connection to db");

                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new EquityInterDayTimeBarCollection[0];
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


                var marketUpdate = new MarketUpdateDto(pair.Exchange);

                _logger.LogInformation($"Reddeer Market Repository CreateAndOrGetSecurityId about to match market or insert sql for {pair.Exchange?.MarketIdentifierCode}");
                
                var marketId = await dbConnection.ExecuteScalarAsync<string>(MarketMatchOrInsertSql, marketUpdate);
                
                _logger.LogInformation($"Reddeer Market Repository CreateAndOrGetSecurityId match market or insert sql for {pair.Exchange?.MarketIdentifierCode}");

                var securityUpdate = new InsertSecurityDto(pair.Security, marketId, _cfiMapper);

                _logger.LogInformation($"Reddeer Market Repository CreateAndOrGetSecurityId about to match security or insert sql for {pair.Security?.Name}");
                
                var securityId = await dbConnection.ExecuteScalarAsync<string>(SecurityMatchOrInsertSql, securityUpdate);
                
                _logger.LogInformation($"Reddeer Market Repository CreateAndOrGetSecurityId completed match security or insert sql for {pair.Security?.Name}");

                return new MarketSecurityIds {MarketId = marketId, SecurityId = securityId};
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"ReddeerMarketRepository CreateAndOrGetSecurityId");
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

            public MarketUpdateDto(Domain.Core.Markets.Market market)
            {
                MarketId = market.MarketIdentifierCode;
                MarketName = market.Name;
            }

            // ReSharper disable MemberCanBePrivate.Local
            public string MarketId { get; set; }
            public string MarketName { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
        }

        private EquityInstrumentIntraDayTimeBar ProjectToSecurity(MarketStockExchangeSecuritiesDto dto, Domain.Core.Markets.Market market)
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
                        dto.BloombergTicker,
                        dto.Ric),
                    dto.SecurityName,
                    dto.Cfi,
                    dto.SecurityCurrency,
                    dto.IssuerIdentifier,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    dto.SectorCode,
                    dto.IndustryCode,
                    dto.RegionCode,
                    dto.CountryCode);

            var spread =
                new SpreadTimeBar(
                    new Money(dto.BidPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Money(dto.AskPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Money(dto.MarketPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Volume(dto.VolumeTraded.GetValueOrDefault(0)));

            var intradayPrices =
                new IntradayPrices(
                    new Money(dto.OpenPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Money(dto.ClosePrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Money(dto.HighIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Money(dto.LowIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency));

            var dailySummary =
                new DailySummaryTimeBar(
                    dto.MarketCap,
                    dto.MarketCapCurrency,
                    intradayPrices,
                    dto.ListedSecurities,
                    new Volume(dto.DailyVolume.GetValueOrDefault(0)),
                    dto.Epoch);

            var tick =
                new EquityInstrumentIntraDayTimeBar(
                    security,
                    spread,
                    dailySummary,
                    dto.Epoch,
                    market);

            return tick;
        }

        private EquityInstrumentInterDayTimeBar ProjectToInterDaySecurity(
            MarketStockExchangeSecuritiesDto dto,
            Domain.Core.Markets.Market market)
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
                        dto.BloombergTicker,
                        dto.Ric),
                    dto.SecurityName,
                    dto.Cfi,
                    dto.SecurityCurrency,
                    dto.IssuerIdentifier,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    dto.SectorCode,
                    dto.IndustryCode,
                    dto.RegionCode,
                    dto.CountryCode);

            var intradayPrices =
                new IntradayPrices(
                    new Money(dto.OpenPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Money(dto.ClosePrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Money(dto.HighIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Money(dto.LowIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency));

            var dailySummary =
                new DailySummaryTimeBar(
                    dto.MarketCap,
                    dto.MarketCapCurrency,
                    intradayPrices,
                    dto.ListedSecurities,
                    new Volume(dto.DailyVolume.GetValueOrDefault(0)),
                    dto.Epoch);

            var tick =
                new EquityInstrumentInterDayTimeBar(
                    security,
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

            public MarketStockExchangeDto(EquityIntraDayTimeBarCollection entity)
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

            public MarketStockExchangeSecuritiesDto(
                EquityInstrumentIntraDayTimeBar entity,
                int marketId,
                ICfiInstrumentTypeMapper cfiMapper)
            {
                if (entity == null)
                {
                    return;
                }

                this.ClientIdentifier = entity.Security?.Identifiers.ClientIdentifier;
                this.ReddeerId = entity.Security?.Identifiers.ReddeerId;
                this.ReddeerEnrichmentId = entity.Security?.Identifiers.ReddeerEnrichmentId;
                this.Sedol = entity.Security?.Identifiers.Sedol;
                this.Isin = entity.Security?.Identifiers.Isin;
                this.Figi = entity.Security?.Identifiers.Figi;
                this.Cusip = entity.Security?.Identifiers.Cusip;
                this.Lei = entity.Security?.Identifiers.Lei;
                this.ExchangeSymbol = entity.Security?.Identifiers.ExchangeSymbol;
                this.BloombergTicker = entity.Security?.Identifiers.BloombergTicker;
                this.Ric = entity.Security?.Identifiers.Ric;
                this.SecurityName = entity.Security?.Name;
                this.Cfi = entity.Security?.Cfi;
                this.IssuerIdentifier = entity.Security?.IssuerIdentifier;
                this.SecurityCurrency =
                    entity.SpreadTimeBar.Price.Currency.Code
                    ?? entity.SpreadTimeBar.Ask.Currency.Code
                    ?? entity.SpreadTimeBar.Bid.Currency.Code;

                this.Epoch = entity.TimeStamp;
                this.EpochDate = entity.TimeStamp.Date;
                this.BidPrice = entity.SpreadTimeBar.Bid.Value;
                this.AskPrice = entity.SpreadTimeBar.Ask.Value;
                this.MarketPrice = entity.SpreadTimeBar.Price.Value;
                this.OpenPrice = entity.DailySummaryTimeBar.IntradayPrices.Open?.Value;
                this.ClosePrice = entity.DailySummaryTimeBar.IntradayPrices.Close?.Value;
                this.HighIntradayPrice = entity.DailySummaryTimeBar.IntradayPrices.High?.Value;
                this.LowIntradayPrice = entity.DailySummaryTimeBar.IntradayPrices.Low?.Value;
                this.ListedSecurities = entity.DailySummaryTimeBar.ListedSecurities;
                this.MarketCap = entity.DailySummaryTimeBar?.MarketCap?.Value;
                this.MarketCapCurrency = entity.DailySummaryTimeBar?.MarketCap?.Currency.Code;
                this.VolumeTraded = entity.SpreadTimeBar.Volume.Traded;
                this.DailyVolume = entity.DailySummaryTimeBar.DailyVolume.Traded;
                this.MarketId = marketId.ToString();
                this.InstrumentType = (int)cfiMapper.MapCfi(entity.Security?.Cfi);

                this.UnderlyingCfi = entity?.Security?.UnderlyingCfi;
                this.UnderlyingName = entity?.Security?.UnderlyingName;
                this.UnderlyingSedol = entity?.Security?.Identifiers.UnderlyingSedol;
                this.UnderlyingIsin = entity?.Security?.Identifiers.UnderlyingIsin;
                this.UnderlyingFigi = entity?.Security?.Identifiers.UnderlyingFigi;
                this.UnderlyingCusip = entity?.Security?.Identifiers.UnderlyingCusip;
                this.UnderlyingLei = entity?.Security?.Identifiers.UnderlyingLei;
                this.UnderlyingExchangeSymbol = entity?.Security?.Identifiers.UnderlyingExchangeSymbol;
                this.UnderlyingBloombergTicker = entity?.Security?.Identifiers.UnderlyingBloombergTicker;
                this.UnderlyingClientIdentifier = entity?.Security?.Identifiers.UnderlyingClientIdentifier;
                this.UnderlyingRic = entity?.Security?.Identifiers.UnderlyingRic;

                this.SectorCode = entity?.Security?.SectorCode;
                this.IndustryCode = entity?.Security?.IndustryCode;
                this.RegionCode = entity?.Security?.RegionCode;
                this.CountryCode = entity?.Security?.CountryCode;
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

            public string Ric { get; set; }

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
            public string UnderlyingRic { get; set; }

            public string SectorCode { get; set; }
            public string IndustryCode { get; set; }
            public string RegionCode { get; set; }
            public string CountryCode { get; set; }


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

            public string MarketCapCurrency { get; set; }




            // dont set these two for writes they're just for reads
            public string MarketId { get; set; }
            public string MarketIdentifierCode { get; set; }
            public string MarketName { get; set; }
        }

        private InsertSecurityDto Project(EquityInstrumentIntraDayTimeBar tick)
        {
            return new InsertSecurityDto(tick, null, _cfiMapper);
        }

        private class InsertSecurityDto
        {
            public InsertSecurityDto(EquityInstrumentIntraDayTimeBar entity, string marketIdForeignKey, ICfiInstrumentTypeMapper cfiMapper)
            {
                if (entity == null)
                {
                    return;
                }

                this.Id = entity?.Security?.Identifiers.Id ?? string.Empty;
                this.MarketIdPrimaryKey = marketIdForeignKey;
                this.MarketId = entity.Market?.MarketIdentifierCode;
                this.MarketName = entity.Market?.Name;
                this.ReddeerId = entity.Security?.Identifiers.ReddeerId;
                this.ClientIdentifier = entity.Security?.Identifiers.ClientIdentifier;
                this.Sedol = entity.Security?.Identifiers.Sedol;
                this.Isin = entity.Security?.Identifiers.Isin;
                this.Figi = entity.Security?.Identifiers.Figi;
                this.Cusip = entity.Security?.Identifiers.Cusip;
                this.Lei = entity.Security?.Identifiers.Lei;
                this.ExchangeSymbol = entity.Security?.Identifiers.ExchangeSymbol;
                this.BloombergTicker = entity.Security?.Identifiers.BloombergTicker;
                this.Ric = entity.Security?.Identifiers.Ric;
                this.SecurityName = entity.Security?.Name;
                this.Cfi = entity.Security?.Cfi;
                this.IssuerIdentifier = entity.Security?.IssuerIdentifier;
                this.SecurityCurrency =
                    entity.SpreadTimeBar.Price.Currency.Code
                    ?? entity.SpreadTimeBar.Ask.Currency.Code
                    ?? entity.SpreadTimeBar.Bid.Currency.Code
                    ?? entity.SpreadTimeBar.Price.Currency.Code;
                this.Epoch = entity.TimeStamp;
                this.BidPrice = entity.SpreadTimeBar.Bid.Value;
                this.AskPrice = entity.SpreadTimeBar.Ask.Value;
                this.MarketPrice = entity.SpreadTimeBar.Price.Value;
                this.OpenPrice = entity.DailySummaryTimeBar.IntradayPrices.Open?.Value;
                this.ClosePrice = entity.DailySummaryTimeBar.IntradayPrices.Close?.Value;
                this.HighIntradayPrice = entity.DailySummaryTimeBar.IntradayPrices.High?.Value;
                this.LowIntradayPrice = entity.DailySummaryTimeBar.IntradayPrices.Low?.Value;
                this.ListedSecurities = entity.DailySummaryTimeBar.ListedSecurities;
                this.MarketCap = entity.DailySummaryTimeBar?.MarketCap?.Value;
                this.MarketCapCurrency = entity.DailySummaryTimeBar?.MarketCap?.Currency.Code;
                this.VolumeTraded = entity.SpreadTimeBar.Volume.Traded;
                this.DailyVolume = entity.DailySummaryTimeBar.DailyVolume.Traded;
                this.InstrumentType = (int)cfiMapper.MapCfi(entity.Security?.Cfi);
                this.EpochDate = entity.TimeStamp.Date;

                this.UnderlyingCfi = entity?.Security?.UnderlyingCfi;
                this.UnderlyingName = entity?.Security?.UnderlyingName;
                this.UnderlyingSedol = entity?.Security?.Identifiers.UnderlyingSedol;
                this.UnderlyingIsin = entity?.Security?.Identifiers.UnderlyingIsin;
                this.UnderlyingFigi = entity?.Security?.Identifiers.UnderlyingFigi;
                this.UnderlyingCusip = entity?.Security?.Identifiers.UnderlyingCusip;
                this.UnderlyingLei = entity?.Security?.Identifiers.UnderlyingLei;
                this.UnderlyingExchangeSymbol = entity?.Security?.Identifiers.UnderlyingExchangeSymbol;
                this.UnderlyingBloombergTicker = entity?.Security?.Identifiers.UnderlyingBloombergTicker;
                this.UnderlyingClientIdentifier = entity?.Security?.Identifiers.UnderlyingClientIdentifier;
                this.UnderlyingRic = entity?.Security?.Identifiers.UnderlyingRic;

                this.SectorCode = entity?.Security?.SectorCode;
                this.IndustryCode = entity?.Security?.IndustryCode;
                this.RegionCode = entity?.Security?.RegionCode;
                this.CountryCode = entity?.Security?.CountryCode;
            }

            public InsertSecurityDto(FinancialInstrument security, string marketIdForeignKey, ICfiInstrumentTypeMapper cfiMapper)
            {
                this.MarketIdPrimaryKey = marketIdForeignKey;

                this.Id = security.Identifiers.Id;
                this.ReddeerId = security.Identifiers.ReddeerId;
                this.ClientIdentifier = security.Identifiers.ClientIdentifier;
                this.Sedol = security.Identifiers.Sedol;
                this.Isin = security.Identifiers.Isin;
                this.Figi = security.Identifiers.Figi;
                this.Cusip = security.Identifiers.Cusip;
                this.Lei = security.Identifiers.Lei;
                this.ExchangeSymbol = security.Identifiers.ExchangeSymbol;
                this.BloombergTicker = security.Identifiers.BloombergTicker;
                this.Ric = security.Identifiers.Ric;
                this.SecurityName = security.Name;
                this.Cfi = security.Cfi;
                this.IssuerIdentifier = security.IssuerIdentifier;
                this.InstrumentType = (int)cfiMapper.MapCfi(security.Cfi);
                this.SecurityCurrency = security.SecurityCurrency;
                
                this.UnderlyingCfi = security.UnderlyingCfi;
                this.UnderlyingName = security.UnderlyingName;
                this.UnderlyingSedol = security.Identifiers.UnderlyingSedol;
                this.UnderlyingIsin = security.Identifiers.UnderlyingIsin;
                this.UnderlyingFigi = security.Identifiers.UnderlyingFigi;
                this.UnderlyingCusip = security.Identifiers.UnderlyingCusip;
                this.UnderlyingLei = security.Identifiers.UnderlyingLei;
                this.UnderlyingExchangeSymbol = security.Identifiers.UnderlyingExchangeSymbol;
                this.UnderlyingBloombergTicker = security.Identifiers.UnderlyingBloombergTicker;
                this.UnderlyingClientIdentifier = security.Identifiers.UnderlyingClientIdentifier;
                this.UnderlyingRic = security.Identifiers.UnderlyingRic;

                this.SectorCode = security.SectorCode;
                this.IndustryCode = security.IndustryCode;
                this.RegionCode = security.RegionCode;
                this.CountryCode = security.CountryCode;
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

            public string Ric { get; set; }

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
            public string UnderlyingRic { get; set; }

            public string SectorCode { get; set; }
            public string IndustryCode { get; set; }
            public string RegionCode { get; set; }
            public string CountryCode { get; set; }

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

            public string MarketCapCurrency { get; set; }

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

                this.Id = dto.Id;
                this.ReddeerId = dto.ReddeerId;
                this.MarketIdentifierCode = dto.MarketIdentifierCode;
                this.MarketName = dto.MarketName;
                this.SecurityName = dto.SecurityName;
                this.Cfi = dto.Cfi;
                this.IssuerIdentifier = dto.IssuerIdentifier;
                this.SecurityClientIdentifier = dto.SecurityClientIdentifier;
                this.Sedol = dto.Sedol;
                this.Isin = dto.Isin;
                this.Figi = dto.Figi;
                this.ExchangeSymbol = dto.ExchangeSymbol;
                this.Cusip = dto.Cusip;
                this.Lei = dto.Lei;
                this.BloombergTicker = dto.BloombergTicker;
                this.Ric = dto.Ric;
                this.InstrumentType = (int)cfiMapper.MapCfi(dto.Cfi);
                this.SectorCode = dto.SectorCode;
                this.IndustryCode = dto.IndustryCode;
                this.RegionCode = dto.RegionCode;
                this.CountryCode = dto.CountryCode;
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
            public string Ric { get; set; }
            public int InstrumentType { get; set; }
            public string SectorCode { get; set; }
            public string IndustryCode { get; set; }
            public string RegionCode { get; set; }
            public string CountryCode { get; set; }
        }
    }
}
