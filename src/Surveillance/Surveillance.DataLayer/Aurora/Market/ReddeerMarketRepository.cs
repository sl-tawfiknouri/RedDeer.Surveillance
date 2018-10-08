using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Market;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class ReddeerMarketRepository : IReddeerMarketRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ReddeerMarketRepository> _logger;

        private const string CreateMarketSql =
            @"INSERT INTO MarketStockExchange(MarketId, MarketName) SELECT @MarketId, @MarketName FROM (SELECT @MarketId, @MarketName) as TMP1 WHERE NOT EXISTS(SELECT * FROM MarketStockExchange WHERE MarketId = @MarketId);
            SELECT * FROM MarketStockExchange WHERE MarketId = @MarketID;";

        private const string CreateSecuritySql =
            @"
            INSERT INTO MarketStockExchangeSecurities(
            MarketStockExchangeId, ClientIdentifier, Sedol, Isin, Figi, Cusip, Lei, ExchangeSymbol, BloombergTicker, SecurityName, Cfi, IssuerIdentifier, SecurityCurrency)
             SELECT 
             MarketStockExchangeId, ClientIdentifier, Sedol, Isin, Figi, Cusip, Lei, ExchangeSymbol, BloombergTicker, SecurityName, Cfi, IssuerIdentifier, SecurityCurrency
             FROM
             (SELECT @MarketStockExchangeId AS MarketStockExchangeId, @ClientIdentifier AS ClientIdentifier, @Sedol AS Sedol, @Isin AS Isin, @Figi AS Figi, @Cusip AS Cusip, @Lei AS Lei, @ExchangeSymbol AS ExchangeSymbol, @BloombergTicker AS BloombergTicker, @SecurityName AS SecurityName, @Cfi AS Cfi, @IssuerIdentifier AS IssuerIdentifier, @SecurityCurrency AS SecurityCurrency) AS TMP2
             WHERE NOT EXISTS(SELECT * FROM MarketStockExchangeSecurities WHERE MarketStockExchangeId = @MarketStockExchangeId AND ClientIdentifier = @ClientIdentifier AND Sedol = @Sedol AND Isin = @Isin AND Figi = @Figi AND Cusip = @Cusip AND Lei = @Lei AND ExchangeSymbol = @ExchangeSymbol AND BloombergTicker = @BloombergTicker AND SecurityName = @SecurityName AND Cfi = @Cfi AND IssuerIdentifier = @IssuerIdentifier AND SecurityCurrency = @SecurityCurrency);

            SELECT @securityInsertId := Id FROM MarketStockExchangeSecurities WHERE MarketStockExchangeId = @MarketStockExchangeId AND ClientIdentifier = @ClientIdentifier AND Sedol = @Sedol AND Isin = @Isin AND Figi = @Figi AND Cusip = @Cusip AND Lei = @Lei AND ExchangeSymbol = @ExchangeSymbol AND BloombergTicker = @BloombergTicker AND SecurityName = @SecurityName AND Cfi = @Cfi AND IssuerIdentifier = @IssuerIdentifier AND SecurityCurrency = @SecurityCurrency;

             INSERT INTO MarketStockExchangePrices (SecurityId, Epoch, BidPrice, AskPrice, MarketPrice, OpenPrice, ClosePrice, HighIntradayPrice, LowIntradayPrice, ListedSecurities, MarketCap, VolumeTradedInTick, DailyVolume) VALUES (@securityInsertId, @Epoch, @BidPrice, @AskPrice, @MarketPrice, @OpenPrice, @ClosePrice, @HighIntradayPrice, @LowIntradayPrice, @ListedSecurities, @MarketCap, @VolumeTradedInTick, @DailyVolume);";

        private const string GetMarketSql =
            @"
            SELECT
             MSE.MarketId as MarketId,
             MSE.MarketName as MarketName,
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

        public async Task Create(ExchangeFrame entity)
        {
            if (entity == null)
            {
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                var stockMarketId = 0;
                var stockMarketExchangeDto = new MarketStockExchangeDto(entity);
                using (var conn = dbConnection.QuerySingleAsync<int>(CreateMarketSql, stockMarketExchangeDto))
                {
                    stockMarketId = await conn;
                }

                var securities =
                    entity
                        .Securities
                        .Select(sec => new MarketStockExchangeSecuritiesDto(sec, stockMarketId))
                        .FirstOrDefault();

                using (var conn = dbConnection.ExecuteAsync(CreateSecuritySql, securities))
                {
                    await conn;
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

        public async Task<IReadOnlyCollection<ExchangeFrame>> Get(DateTime start, DateTime end)
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
                                new { rep.MarketId, rep.MarketName, rep.Epoch},
                                (key, group) => new
                                {
                                    Key1 = key.MarketId,
                                    Key2 = key.MarketName,
                                    Key3 = key.Epoch,
                                    Result = group.ToList()
                                })
                            .Select(i =>
                            {
                                var market = new StockExchange(new Domain.Market.Market.MarketId(i.Key1), i.Key2);
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
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            return new ExchangeFrame[0];
        }

        private SecurityTick ProjectToSecurity(MarketStockExchangeSecuritiesDto dto, StockExchange market)
        {
            if (dto == null)
            {
                return null;
            }

            var security =
                new Security(
                    new SecurityIdentifiers(
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
                    new Price(dto.BidPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Price(dto.AskPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Price(dto.MarketPrice.GetValueOrDefault(0), dto.SecurityCurrency));

            var intradayPrices =
                new IntradayPrices(
                    new Price(dto.OpenPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Price(dto.ClosePrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Price(dto.HighIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency),
                    new Price(dto.LowIntradayPrice.GetValueOrDefault(0), dto.SecurityCurrency));

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

                MarketId = entity?.Exchange?.Id?.Id;
                MarketName = entity?.Exchange?.Name;
            }

            public int Id { get; set; }

            public string MarketId { get; set; }

            public string MarketName { get; set; }
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
                    entity.Spread.Price.Currency
                    ?? entity.Spread.Ask.Currency
                    ?? entity.Spread.Bid.Currency;




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

            public int Id { get; set; }

            public int MarketStockExchangeId { get; set; }

            public string ClientIdentifier { get; set; }

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

            public int? ListedSecurities { get; set; }

            public decimal? MarketCap { get; set; }

            public int? VolumeTradedInTick { get; set; }

            public int? DailyVolume { get; set; }




            // dont set these two for writes they're just for reads
            public string MarketId { get; set; }
            public string MarketName { get; set; }
        }
    }
}