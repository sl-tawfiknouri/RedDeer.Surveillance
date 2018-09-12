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

        public UploadTradeFileProcessorResult Process(string path)
        {
            if (!File.Exists(path))
            {
                _logger.LogError($"Upload Trade File Monitor did not find file {path}");
                return 
                    new UploadTradeFileProcessorResult(
                        new List<TradeOrderFrame>(),
                        new List<TradeOrderFrameCsv>());
            }

            var tradeOrders = new List<TradeOrderFrame>();
            var failedTradeOrderReads = new List<TradeOrderFrameCsv>();

            using (var reader = File.OpenText(path))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.MissingFieldFound = null;

                var csvRecords = new List<TradeOrderFrameCsv>();

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                   var record = MapToCsvDto(csv);

                    if (record == null)
                    {
                        continue;
                    }

                    csvRecords.Add(record);
                }

                foreach (var record in csvRecords)
                {
                    var mappedRecord = _csvToDtoMapper.Map(record);
                    if (mappedRecord != null)
                    {
                        tradeOrders.Add(mappedRecord);
                    }
                    else
                    {
                        failedTradeOrderReads.Add(record);
                    }
                }

                csv.Dispose();
                reader.Dispose();
            }

            if (_csvToDtoMapper.FailedParseTotal > 0)
            {
                _logger.LogError($"TradingFileRelayProcess had {_csvToDtoMapper.FailedParseTotal} errors parsing the input CSV file {path}");
            }

            var sortedTradeOrders = tradeOrders.OrderBy(to => to.StatusChangedOn).ToList();

            return new UploadTradeFileProcessorResult(sortedTradeOrders, failedTradeOrderReads);
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
                SecurityClientIdentifier = rawRecord[_mappingConfig.SecurityClientIdentifierFieldName],
                SecurityFigi = rawRecord[_mappingConfig.SecurityFigiFieldName],
                SecurityIsin = rawRecord[_mappingConfig.SecurityIsinFieldName],
                SecuritySedol = rawRecord[_mappingConfig.SecuritySedolFieldName],
                SecurityName = rawRecord[_mappingConfig.SecurityNameFieldName],
                OrderType = rawRecord[_mappingConfig.OrderTypeFieldName],
                OrderDirection = rawRecord[_mappingConfig.OrderDirectionFieldName],
                OrderStatus =  rawRecord[_mappingConfig.OrderStatusFieldName],
                Volume =  rawRecord[_mappingConfig.VolumeFieldName],
                LimitPrice = rawRecord[_mappingConfig.LimitPriceFieldName],
            };
        }

        public void WriteFailedReadsToDisk(
            string path,
            string originalFileName,
            IReadOnlyCollection<TradeOrderFrameCsv> failedReads)
        {
            if (failedReads == null
                || !failedReads.Any())
            {
                return;
            }

            var failedFileName = $"{originalFileName}-failed-read-{Guid.NewGuid()}.csv";
            var target = Path.Combine(path, failedFileName);

            using (TextWriter twriter = new StreamWriter(target))
            {
                var csv = new CsvWriter(twriter);

                // write out headers
                csv.WriteField(_mappingConfig.StatusChangedOnFieldName);
                csv.WriteField(_mappingConfig.MarketIdFieldName);
                csv.WriteField(_mappingConfig.MarketAbbreviationFieldName);
                csv.WriteField(_mappingConfig.MarketNameFieldName);
                csv.WriteField(_mappingConfig.SecurityClientIdentifierFieldName);
                csv.WriteField(_mappingConfig.SecurityFigiFieldName);
                csv.WriteField(_mappingConfig.SecurityIsinFieldName);
                csv.WriteField(_mappingConfig.SecuritySedolFieldName);
                csv.WriteField(_mappingConfig.SecurityNameFieldName);
                csv.WriteField(_mappingConfig.OrderTypeFieldName);
                csv.WriteField(_mappingConfig.OrderDirectionFieldName);
                csv.WriteField(_mappingConfig.OrderStatusFieldName);
                csv.WriteField(_mappingConfig.VolumeFieldName);
                csv.WriteField(_mappingConfig.LimitPriceFieldName);

                csv.NextRecord();

                foreach (var rec in failedReads)
                {
                    if (rec == null)
                    {
                        continue;
                    }

                    csv.WriteRecord(rec);
                    csv.NextRecord();
                }
            }
        }
    }
}