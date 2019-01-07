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

        private const string CreateSql = @"
            SET @secId = (SELECT Id FROM FinancialInstruments WHERE Figi = @Figi LIMIT 1);

            INSERT IGNORE INTO InstrumentEquityTimeBars (SecurityId, Epoch, BidPrice, AskPrice, MarketPrice, VolumeTraded, Currency) VALUES(@secId, @Epoch, @BidPrice, @AskPrice, @MarketPrice, @VolumeTraded, @Currency)";

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
                
                var saveBarDtos = barDtos.Select(i => new MinuteBarSaveDto(i)).ToList();
                using (var conn = dbConnection.ExecuteAsync(CreateSql, saveBarDtos))
                {
                    await conn;
                    _logger?.LogInformation(
                        $"ReddeerMarketTimeBarRepository Save method completed saving {barDtos?.Count} rows. If there were any duplicate inserts these would of been discarded.");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"ReddeerMarketTimeBarRepository Save method initiated", e);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            _logger?.LogInformation($"ReddeerMarketTimeBarRepository Save method initiated");
        }

        private class MinuteBarSaveDto
        {
            public MinuteBarSaveDto(MinuteBarDto dto)
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
                MarketPrice = dto.ExecutionClosePrice;

                BidPrice = dto.BestBidClosePrice;
                AskPrice = dto.BestAskClosePrice;
            }

            public string Figi { get; set; }
            public DateTime Epoch { get; set; }
            public double? VolumeTraded { get; set; }
            public double? MarketPrice { get; set; }
            public double? BidPrice { get; set; }
            public double? AskPrice { get; set; }
            public string Currency { get; set; }
        }
    }
}
