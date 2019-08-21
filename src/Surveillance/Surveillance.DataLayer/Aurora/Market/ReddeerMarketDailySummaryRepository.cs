namespace Surveillance.DataLayer.Aurora.Market
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;

    /// <summary>
    ///     Be aware that this table is also referenced in the market repository
    /// </summary>
    public class ReddeerMarketDailySummaryRepository : IReddeerMarketDailySummaryRepository
    {
        private const string CreateSql = @"
            SET @secId = (SELECT Id FROM FinancialInstruments WHERE Figi = @Figi LIMIT 1);

            INSERT IGNORE INTO InstrumentEquityDailySummary (SecurityId, Epoch, EpochDate, OpenPrice, ClosePrice, HighIntradayPrice, LowIntradayPrice, ListedSecurities, MarketCap, DailyVolume, Currency) VALUES (@secId, @Epoch, @EpochDate, @OpenPrice, @ClosePrice, @HighIntradayPrice, @LowIntradayPrice, @ListedSecurites, @MarketCap, @DailyVolume, @Currency)";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<ReddeerMarketDailySummaryRepository> _logger;

        public ReddeerMarketDailySummaryRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<ReddeerMarketDailySummaryRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Save(IReadOnlyCollection<FactsetSecurityDailyResponseItem> responseItems)
        {
            this._logger?.LogInformation(
                $"ReddeerMarketDailySummaryRepository Save method called with {responseItems?.Count ?? 0} items");

            if (responseItems == null || !responseItems.Any())
            {
                this._logger?.LogInformation(
                    "ReddeerMarketDailySummaryRepository Save method returned due to null or empty response items list");
                return;
            }

            try
            {
                this._logger?.LogInformation("ReddeerMarketDailySummaryRepository Save method opened db connection");

                var projectedItems = responseItems.Select(x => new DailySummaryDto(x)).ToList();

                using (var dbConn = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(CreateSql, projectedItems))
                {
                    await conn;
                    this._logger?.LogInformation(
                        $"ReddeerMarketDailySummaryRepository Save method completed saving {projectedItems?.Count} rows. If there were any duplicate inserts these would of been discarded.");
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(
                    "ReddeerMarketDailySummaryRepository encountered an exception on saving response items",
                    e);
            }

            this._logger?.LogInformation(
                $"ReddeerMarketDailySummaryRepository Save method completed for {responseItems?.Count ?? 0} items");
        }

        private class DailySummaryDto
        {
            public DailySummaryDto(FactsetSecurityDailyResponseItem item)
            {
                if (item == null) return;

                this.Epoch = item.Epoch;
                this.EpochDate = item.Epoch.Date;
                this.Figi = item.Figi;
                this.Currency = item.Currency;
                this.OpenPrice = item.OpenPrice;
                this.ClosePrice = item.ClosePrice;
                this.HighIntradayPrice = item.HighIntradayPrice;
                this.LowIntradayPrice = item.LowIntradayPrice;
                this.MarketCap = item.MarketCapitalisation;
                this.DailyVolume = item.DailyVolume;
            }

            public decimal? ClosePrice { get; }

            public string Currency { get; }

            public long? DailyVolume { get; }

            public DateTime Epoch { get; }

            public DateTime EpochDate { get; }

            public string Figi { get; }

            public decimal? HighIntradayPrice { get; }

            public decimal? LowIntradayPrice { get; }

            public decimal? MarketCap { get; }

            public decimal? OpenPrice { get; }
        }
    }
}