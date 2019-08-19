namespace Surveillance.DataLayer.Aurora.Market
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Firefly.Service.Data.BMLL.Shared.Dtos;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;

    /// <summary>
    ///     Be aware that this table is also referenced in the market repository
    /// </summary>
    public class ReddeerMarketTimeBarRepository : IReddeerMarketTimeBarRepository
    {
        private const string CreateSql = @"
            INSERT IGNORE INTO InstrumentEquityTimeBars (SecurityId, Epoch, BidPrice, AskPrice, MarketPrice, VolumeTraded, Currency) VALUES(@SecurityId, @Epoch, @BidPrice, @AskPrice, @MarketPrice, @VolumeTraded, @Currency)";

        private const string GetFigiToSecurityIdLookup = @"
            SELECT DISTINCT Id, Figi FROM FinancialInstruments WHERE Figi <> '';";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<ReddeerMarketTimeBarRepository> _logger;

        public ReddeerMarketTimeBarRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<ReddeerMarketTimeBarRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Save(List<MinuteBarDto> barDtos)
        {
            this._logger?.LogInformation("ReddeerMarketTimeBarRepository Save method initiated");

            if (barDtos == null || !barDtos.Any())
            {
                this._logger?.LogInformation(
                    "ReddeerMarketTimeBarRepository Save method returned due to null or empty response items list");
                return;
            }

            var dbConnection = this._dbConnectionFactory.BuildConn();

            try
            {
                this._logger?.LogInformation("ReddeerMarketTimeBarRepository Save method open db connection");

                var lookupFigiToSecurityIds = new Dictionary<string, List<string>>();

                this._logger?.LogInformation(
                    "ReddeerMarketTimeBarRepository Save method about to build figi to security id look up");
                using (var conn = dbConnection.QueryAsync<FigiLookup>(GetFigiToSecurityIdLookup))
                {
                    var lookup = (await conn).ToList();
                    lookupFigiToSecurityIds = lookup.GroupBy(i => i.Figi).ToDictionary(
                        y => y.Key,
                        y => y.Select(i => i.Id).ToList());

                    this._logger?.LogInformation(
                        "ReddeerMarketTimeBarRepository Save method built figi to security id look up");
                }

                var saveBarDtos = this.MinuteBars(barDtos, lookupFigiToSecurityIds);
                this._logger?.LogInformation(
                    $"ReddeerMarketTimeBarRepository Save method about to save {barDtos?.Count} rows. If there are any duplicate inserts these will be  been discarded.");

                using (var conn = dbConnection.ExecuteAsync(CreateSql, saveBarDtos))
                {
                    await conn;
                    this._logger?.LogInformation(
                        $"ReddeerMarketTimeBarRepository Save method completed saving {barDtos?.Count} rows. If there were any duplicate inserts these would of been discarded.");
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError($"ReddeerMarketTimeBarRepository Save method exception {e.Message}", e);
                if (e.InnerException != null)
                    this._logger?.LogError(
                        $"ReddeerMarketTimeBarRepository Save method exception {e.InnerException?.Message}",
                        e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            this._logger?.LogInformation("ReddeerMarketTimeBarRepository Save method initiated");
        }

        private List<MinuteBarSaveDto> MinuteBars(
            List<MinuteBarDto> barDtos,
            Dictionary<string, List<string>> lookupFigis)
        {
            if (barDtos == null || !barDtos.Any() || lookupFigis == null || !lookupFigis.Any())
                return new List<MinuteBarSaveDto>();

            var saveBars = new List<MinuteBarSaveDto>();
            foreach (var bar in barDtos)
            {
                var securityIds = new List<string>();
                if (lookupFigis.ContainsKey(bar.Figi))
                {
                    securityIds = lookupFigis[bar.Figi];
                }
                else
                {
                    this._logger?.LogError(
                        $"ReddeerMarketTimeBarRepository Save method asked to save a bar with a figi of {bar.Figi} but couldn't find the security lookup!");
                    continue;
                }

                var saveBarDtos = securityIds.Select(o => new MinuteBarSaveDto(bar, o)).ToList();

                if (saveBarDtos != null && saveBarDtos.Any()) saveBars.AddRange(saveBarDtos);
            }

            return saveBars;
        }

        public class FigiLookup
        {
            public string Figi { get; set; }

            public string Id { get; set; }
        }

        private class MinuteBarSaveDto
        {
            public MinuteBarSaveDto(MinuteBarDto dto, string securityId)
            {
                if (dto == null) return;

                this.Epoch = new DateTime(
                    dto.DateTime.Year,
                    dto.DateTime.Month,
                    dto.DateTime.Day,
                    dto.DateTime.Hour,
                    dto.DateTime.Minute,
                    dto.DateTime.Second);

                this.Figi = dto.Figi;
                this.Currency = null;

                this.VolumeTraded = dto.ExecutionVolume;
                this.MarketPrice = dto.ExecutionClosePrice; // nulls filtered out in storage manager

                this.BidPrice = dto.BestBidClosePrice;
                this.AskPrice = dto.BestAskClosePrice;

                this.SecurityId = securityId;
            }

            public double? AskPrice { get; }

            public double? BidPrice { get; }

            public string Currency { get; }

            public DateTime Epoch { get; }

            public string Figi { get; }

            public double? MarketPrice { get; }

            public string SecurityId { get; }

            public double? VolumeTraded { get; }
        }
    }
}