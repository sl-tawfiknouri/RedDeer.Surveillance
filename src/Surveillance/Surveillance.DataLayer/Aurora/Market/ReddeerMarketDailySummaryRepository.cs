using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class ReddeerMarketDailySummaryRepository : IReddeerMarketDailySummaryRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ReddeerMarketDailySummaryRepository> _logger;

        private const string CreateSql = @"
            SET @secId = (SELECT Id FROM FinancialInstruments WHERE Figi = @Figi LIMIT 1);

            INSERT IGNORE INTO InstrumentEquityDailySummary (SecurityId, Epoch, EpochDate, OpenPrice, ClosePrice, HighIntradayPrice, LowIntradayPrice, ListedSecurities, MarketCap, DailyVolume, Currency) VALUES (@secId, @Epoch, @EpochDate, @OpenPrice, @ClosePrice, @HighIntradayPrice, @LowIntradayPrice, @ListedSecurites, @MarketCap, @DailyVolume, @Currency)";

        public ReddeerMarketDailySummaryRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<ReddeerMarketDailySummaryRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Save(IReadOnlyCollection<FactsetSecurityDailyResponseItem> responseItems)
        {
            _logger?.LogInformation($"ReddeerMarketDailySummaryRepository Save method called with {responseItems?.Count ?? 0} items");

            if (responseItems == null
                || !responseItems.Any())
            {
                _logger?.LogInformation($"ReddeerMarketDailySummaryRepository Save method returned due to null or empty response items list");
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                _logger?.LogInformation($"ReddeerMarketDailySummaryRepository Save method opened db connection");

                var projectedItems = responseItems.Select(x => new DailySummaryDto(x)).ToList();

                using (var conn = dbConnection.ExecuteAsync(CreateSql, projectedItems))
                {
                    await conn;
                    _logger?.LogInformation(
                        $"ReddeerMarketDailySummaryRepository Save method completed saving {projectedItems?.Count} rows. If there were any duplicate inserts these would of been discarded.");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(
                    $"ReddeerMarketDailySummaryRepository encountered an exception on saving response items", e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            _logger?.LogInformation($"ReddeerMarketDailySummaryRepository Save method completed for {responseItems?.Count ?? 0} items");
        }

        private class DailySummaryDto
        {
            public DailySummaryDto(FactsetSecurityDailyResponseItem item)
            {
                if (item == null)
                {
                    return;
                }

                Epoch = item.Epoch;
                EpochDate = item.Epoch.Date;
                Figi = item.Figi;
                Currency = item.Currency;
                OpenPrice = item.OpenPrice;
                ClosePrice = item.ClosePrice;
                HighIntradayPrice = item.HighIntradayPrice;
                LowIntradayPrice = item.LowIntradayPrice;
                MarketCap = CalculateMarketCap(item);
                DailyVolume = item.DailyVolume;
            }

            private decimal? CalculateMarketCap(FactsetSecurityDailyResponseItem item)
            {
                if (item == null
                    || item.MarketCapitalisationUsd == null
                    || item.MarketCapitalisationUsd == 0
                    || item.OpenPriceUsd == null
                    || item.OpenPriceUsd == 0
                    || item.OpenPrice == null
                    || item.OpenPrice == 0)
                {
                    return null;
                }

                var vol = item.MarketCapitalisationUsd.Value / item.OpenPriceUsd.Value;
                return vol * item.OpenPrice;
            }

            public DateTime Epoch { get; set; }
            public DateTime EpochDate { get; set; }
            public string Figi { get; set; }
            public string Currency { get; set; }
            public decimal? OpenPrice { get; set; }
            public decimal? ClosePrice { get; set; }
            public decimal? HighIntradayPrice { get; set; }
            public decimal? LowIntradayPrice { get; set; }
            public decimal? MarketCap { get; set; }
            public long? DailyVolume { get; set; }
        }
    }
}
