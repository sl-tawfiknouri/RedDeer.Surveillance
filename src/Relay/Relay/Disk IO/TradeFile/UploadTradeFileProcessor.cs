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

            _csvToDtoMapper.FailedParseTotal = 0;
        }

        protected override TradeOrderFrameCsv MapToCsvDto(CsvReader rawRecord, int rowId)
        {
            if (rawRecord == null)
            {
                return null;
            }

            return new TradeOrderFrameCsv
            {
                StatusChangedOn = rawRecord[_mappingConfig.StatusChangedOnFieldName],
                MarketIdentifierCode = rawRecord[_mappingConfig.MarketIdentifierCodeFieldName],
                MarketName = rawRecord[_mappingConfig.MarketNameFieldName],

                SecurityClientIdentifier = rawRecord[_mappingConfig.SecurityClientIdentifierFieldName],
                SecurityFigi = rawRecord[_mappingConfig.SecurityFigiFieldName],
                SecurityIsin = rawRecord[_mappingConfig.SecurityIsinFieldName],
                SecuritySedol = rawRecord[_mappingConfig.SecuritySedolFieldName],
                SecurityCusip = rawRecord[_mappingConfig.SecurityCusipFieldName],
                SecurityExchangeSymbol = rawRecord[_mappingConfig.SecurityExchangeSymbolFieldName],

                SecurityName = rawRecord[_mappingConfig.SecurityNameFieldName],
                SecurityCfi = rawRecord[_mappingConfig.SecurityCfiFieldName],

                OrderType = rawRecord[_mappingConfig.OrderTypeFieldName],
                OrderPosition = rawRecord[_mappingConfig.OrderPositionFieldName],
                OrderStatus = rawRecord[_mappingConfig.OrderStatusFieldName],
                FulfilledVolume = rawRecord[_mappingConfig.FulfilledVolumeFieldName],
                LimitPrice = rawRecord[_mappingConfig.LimitPriceFieldName],

                TradeSubmittedOn = rawRecord[_mappingConfig.TradeSubmittedOnFieldName],
                TraderId = rawRecord[_mappingConfig.TraderIdFieldName],
                ClientAttributionId = rawRecord[_mappingConfig.TraderClientAttributionIdFieldName],
                PartyBrokerId = rawRecord[_mappingConfig.PartyBrokerIdFieldName],
                CounterPartyBrokerId = rawRecord[_mappingConfig.CounterPartyBrokerIdFieldName],

                OrderCurrency = rawRecord[_mappingConfig.CurrencyFieldName],
                SecurityLei = rawRecord[_mappingConfig.SecurityLei],
                SecurityBloombergTicker = rawRecord[_mappingConfig.SecurityBloombergTickerFieldName],
                ExecutedPrice = rawRecord[_mappingConfig.ExecutedPriceFieldName],
                OrderedVolume = rawRecord[_mappingConfig.OrderedVolumeFieldName],
                AccountId = rawRecord[_mappingConfig.AccountIdFieldName],
                DealerInstructions = rawRecord[_mappingConfig.DealerInstructionsFieldName],
                TradeRationale = rawRecord[_mappingConfig.TradeRationaleFieldName],
                TradeStrategy = rawRecord[_mappingConfig.TradeStrategyFieldName],
                SecurityIssuerIdentifier = rawRecord[_mappingConfig.SecurityIssuerIdentifier],

                RowId = rowId
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
                csv.WriteField(_mappingConfig.MarketIdentifierCodeFieldName);
                csv.WriteField(_mappingConfig.MarketNameFieldName);

                csv.WriteField(_mappingConfig.SecurityClientIdentifierFieldName);
                csv.WriteField(_mappingConfig.SecurityFigiFieldName);
                csv.WriteField(_mappingConfig.SecurityIsinFieldName);
                csv.WriteField(_mappingConfig.SecuritySedolFieldName);
                csv.WriteField(_mappingConfig.SecurityCusipFieldName);
                csv.WriteField(_mappingConfig.SecurityExchangeSymbolFieldName);


                csv.WriteField(_mappingConfig.SecurityNameFieldName);
                csv.WriteField(_mappingConfig.SecurityCfiFieldName);

                csv.WriteField(_mappingConfig.OrderTypeFieldName);
                csv.WriteField(_mappingConfig.OrderPositionFieldName);
                csv.WriteField(_mappingConfig.OrderStatusFieldName);
                csv.WriteField(_mappingConfig.FulfilledVolumeFieldName);
                csv.WriteField(_mappingConfig.LimitPriceFieldName);

                csv.WriteField(_mappingConfig.TraderIdFieldName);
                csv.WriteField(_mappingConfig.CurrencyFieldName);
                csv.WriteField(_mappingConfig.PartyBrokerIdFieldName);
                csv.WriteField(_mappingConfig.CounterPartyBrokerIdFieldName);
                csv.WriteField(_mappingConfig.TradeSubmittedOnFieldName);
                csv.WriteField(_mappingConfig.TraderClientAttributionIdFieldName);

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