using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain.Trades.Orders;
using Domain.Trades.Orders.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Disk_IO.Interfaces;

namespace Relay.Disk_IO
{
    public class UploadTradeFileProcessor : IUploadTradeFileProcessor
    {
        private readonly ITradeOrderCsvToDtoMapper _csvToDtoMapper;
        private readonly ITradeOrderCsvConfig _mappingConfig;
        private readonly ILogger _logger;

        public UploadTradeFileProcessor(
            ITradeOrderCsvToDtoMapper csvToDtoMapper,
            ITradeOrderCsvConfig mappingConfig,
            ILogger<UploadTradeFileProcessor> logger)
        {
            _csvToDtoMapper = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
            _mappingConfig = mappingConfig ?? throw new ArgumentNullException(nameof(mappingConfig));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<TradeOrderFrame> Process(string path)
        {
            if (!File.Exists(path))
            {
                _logger.LogError($"Upload Trade File Monitor did not find file {path}");
                return new List<TradeOrderFrame>();
            }

            var tradeOrders = new List<TradeOrderFrame>();

            using (var reader = File.OpenText(path))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.MissingFieldFound = null;

                var csvRecords2 = new List<TradeOrderFrameCsv>();

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                   var record = MapToCsvDto(csv);

                    if (record == null)
                    {
                        continue;
                    }

                    csvRecords2.Add(record);
                }

                foreach (var record in csvRecords2)
                {
                    var mappedRecord = _csvToDtoMapper.Map(record);
                    if (mappedRecord != null)
                    {
                        tradeOrders.Add(mappedRecord);
                    }
                }

                csv.Dispose();
                reader.Dispose();
            }

            if (_csvToDtoMapper.FailedParseTotal > 0)
            {
                _logger.LogError($"TradingFileRelayProcess had {_csvToDtoMapper.FailedParseTotal} errors parsing the input CSV file {path}");
            }

            if (!tradeOrders.Any())
            {
                return new List<TradeOrderFrame>();
            }

            var sortedTradeOrders = tradeOrders.OrderBy(to => to.StatusChangedOn).ToList();

            return sortedTradeOrders;
        }

        private TradeOrderFrameCsv MapToCsvDto(CsvReader rawRecord)
        {
            if (rawRecord == null)
            {
                return null;
            }

            return new TradeOrderFrameCsv
            {
                StatusChangedOn = rawRecord[_mappingConfig.StatusChangedOnFieldName],
                MarketId = rawRecord[_mappingConfig.MarketIdFieldName],
                MarketAbbreviation = rawRecord[_mappingConfig.MarketAbbreviationFieldName],
                MarketName = rawRecord[_mappingConfig.MarketNameFieldName],
                SecurityId = rawRecord[_mappingConfig.SecurityIdFieldName],
                SecurityName = rawRecord[_mappingConfig.SecurityNameFieldName],
                OrderType = rawRecord[_mappingConfig.OrderTypeFieldName],
                OrderDirection = rawRecord[_mappingConfig.OrderDirectionFieldName],
                OrderStatus =  rawRecord[_mappingConfig.OrderStatusFieldName],
                Volume =  rawRecord[_mappingConfig.VolumeFieldName],
                LimitPrice = rawRecord[_mappingConfig.LimitPriceFieldName],
            };
        }
    }
}