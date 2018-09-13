using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain.Trades.Orders;
using Domain.Trades.Orders.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Disk_IO.TradeFile.Interfaces;

namespace Relay.Disk_IO.TradeFile
{
    public class UploadTradeFileProcessor : BaseUploadFileProcessor<TradeOrderFrameCsv, TradeOrderFrame>, IUploadTradeFileProcessor
    {
        private readonly ITradeOrderCsvToDtoMapper _csvToDtoMapper;
        private readonly ITradeOrderCsvConfig _mappingConfig;

        public UploadTradeFileProcessor(
            ITradeOrderCsvToDtoMapper csvToDtoMapper,
            ITradeOrderCsvConfig mappingConfig,
            ILogger<UploadTradeFileProcessor> logger)
            : base(logger, "Upload Trade File Processor")
        {
            _csvToDtoMapper = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
            _mappingConfig = mappingConfig ?? throw new ArgumentNullException(nameof(mappingConfig));
        }

        protected override void MapRecord(
            TradeOrderFrameCsv record,
            List<TradeOrderFrame> tradeOrders,
            List<TradeOrderFrameCsv> failedTradeOrderReads)
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

        protected override void CheckAndLogFailedParsesFromDtoMapper()
        {
            if (_csvToDtoMapper.FailedParseTotal > 0)
            {
                Logger.LogError($"{UploadFileProcessorName} had {_csvToDtoMapper.FailedParseTotal} errors parsing the input CSV file");
            }
        }

        protected override TradeOrderFrameCsv MapToCsvDto(CsvReader rawRecord)
        {
            if (rawRecord == null)
            {
                return null;
            }

            return new TradeOrderFrameCsv
            {
                StatusChangedOn = rawRecord[_mappingConfig.StatusChangedOnFieldName],
                MarketId = rawRecord[_mappingConfig.MarketIdFieldName],
                MarketName = rawRecord[_mappingConfig.MarketNameFieldName],
                SecurityClientIdentifier = rawRecord[_mappingConfig.SecurityClientIdentifierFieldName],
                SecurityFigi = rawRecord[_mappingConfig.SecurityFigiFieldName],
                SecurityIsin = rawRecord[_mappingConfig.SecurityIsinFieldName],
                SecuritySedol = rawRecord[_mappingConfig.SecuritySedolFieldName],
                SecurityName = rawRecord[_mappingConfig.SecurityNameFieldName],
                OrderType = rawRecord[_mappingConfig.OrderTypeFieldName],
                OrderDirection = rawRecord[_mappingConfig.OrderDirectionFieldName],
                OrderStatus = rawRecord[_mappingConfig.OrderStatusFieldName],
                Volume = rawRecord[_mappingConfig.VolumeFieldName],
                LimitPrice = rawRecord[_mappingConfig.LimitPriceFieldName],
            };
        }

        public void WriteFailedReadsToDisk(string failedReadsPath, string failedReadFileName, IReadOnlyCollection<TradeOrderFrameCsv> failedReads)
        {
            if (failedReads == null
                || !failedReads.Any())
            {
                return;
            }

            var failedFileName = $"{failedReadFileName}-failed-read-{Guid.NewGuid()}.csv";
            var target = Path.Combine(failedReadsPath, failedFileName);

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