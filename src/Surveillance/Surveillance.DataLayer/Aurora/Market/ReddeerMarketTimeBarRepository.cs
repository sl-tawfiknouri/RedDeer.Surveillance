using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace Surveillance.DataLayer.Aurora.Market
{
    /// <summary>
    /// Be aware that this table is also referenced in the market repository
    /// </summary>
    public class ReddeerMarketTimeBarRepository : IReddeerMarketTimeBarRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<ReddeerMarketTimeBarRepository> _logger;

        private const string GetFigiToSecurityIdLookup = @"
            SELECT DISTINCT Id, Figi FROM FinancialInstruments WHERE Figi <> '';";

        private const string CreateSql = @"
            INSERT IGNORE INTO InstrumentEquityTimeBars (SecurityId, Epoch, BidPrice, AskPrice, MarketPrice, VolumeTraded, Currency) VALUES(@SecurityId, @Epoch, @BidPrice, @AskPrice, @MarketPrice, @VolumeTraded, @Currency)";

        public ReddeerMarketTimeBarRepository(
            IConnectionStringFactory dbConnectionFactory, 
            ILogger<ReddeerMarketTimeBarRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Save(List<MinuteBarDto> barDtos)
        {
            _logger?.LogInformation($"ReddeerMarketTimeBarRepository Save method initiated");

            if (barDtos == null
                || !barDtos.Any())
            {
                _logger?.LogInformation($"ReddeerMarketTimeBarRepository Save method returned due to null or empty response items list");
                return;
            }

            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                _logger?.LogInformation($"ReddeerMarketTimeBarRepository Save method open db connection");

                var lookupFigiToSecurityIds = new Dictionary<string, List<string>>();

                _logger?.LogInformation($"ReddeerMarketTimeBarRepository Save method about to build figi to security id look up");
                using (var conn = dbConnection.QueryAsync<FigiLookup>(GetFigiToSecurityIdLookup))
                {
                    var lookup = (await conn).ToList();
                    lookupFigiToSecurityIds = lookup.GroupBy(i => i.Figi).ToDictionary(y => y.Key, y => y.Select(i => i.Id).ToList());

                    _logger?.LogInformation($"ReddeerMarketTimeBarRepository Save method built figi to security id look up");
                }

                var saveBarDtos = MinuteBars(barDtos, lookupFigiToSecurityIds);
                _logger?.LogInformation(
                    $"ReddeerMarketTimeBarRepository Save method about to save {barDtos?.Count} rows. If there are any duplicate inserts these will be  been discarded.");

                using (var conn = dbConnection.ExecuteAsync(CreateSql, saveBarDtos))
                {
                    await conn;
                    _logger?.LogInformation(
                        $"ReddeerMarketTimeBarRepository Save method completed saving {barDtos?.Count} rows. If there were any duplicate inserts these would of been discarded.");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"ReddeerMarketTimeBarRepository Save method exception {e.Message}", e);
                if (e.InnerException != null)
                {
                    _logger?.LogError($"ReddeerMarketTimeBarRepository Save method exception {e.InnerException?.Message}", e);
                }
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            _logger?.LogInformation($"ReddeerMarketTimeBarRepository Save method initiated");
        }

        private List<MinuteBarSaveDto> MinuteBars(List<MinuteBarDto> barDtos, Dictionary<string, List<string>> lookupFigis)
        {
            if (barDtos == null
                || !barDtos.Any()
                || lookupFigis == null
                || !lookupFigis.Any())
            {
                return new List<MinuteBarSaveDto>();
            }

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
                    _logger?.LogError($"ReddeerMarketTimeBarRepository Save method asked to save a bar with a figi of {bar.Figi} but couldn't find the security lookup!");
                    continue;
                }

                var saveBarDtos = securityIds.Select(o => new MinuteBarSaveDto(bar, o)).ToList();

                if (saveBarDtos != null
                    && saveBarDtos.Any())
                {
                    saveBars.AddRange(saveBarDtos);
                }
            }

            return saveBars;
        }

        private class MinuteBarSaveDto
        {
            public MinuteBarSaveDto(MinuteBarDto dto, string securityId)
            {
                if (dto == null)
                {
                    return;
                }

                Epoch = new DateTime(
                    dto.DateTime.Year,
                    dto.DateTime.Month,
                    dto.DateTime.Day,
                    dto.DateTime.Hour,
                    dto.DateTime.Minute,
                    dto.DateTime.Second);

                Figi = dto.Figi;
                Currency = null;

                VolumeTraded = dto.ExecutionVolume;
                MarketPrice = dto.ExecutionClosePrice; // nulls filtered out in storage manager

                BidPrice = dto.BestBidClosePrice;
                AskPrice = dto.BestAskClosePrice;

                SecurityId = securityId;
            }

            public string SecurityId { get; set; }
            public string Figi { get; set; }
            public DateTime Epoch { get; set; }
            public double? VolumeTraded { get; set; }
            public double? MarketPrice { get; set; }
            public double? BidPrice { get; set; }
            public double? AskPrice { get; set; }
            public string Currency { get; set; }
        }

        public class FigiLookup
        {
            public string Id { get; set; }
            public string Figi { get; set; }
        }
    }
}
