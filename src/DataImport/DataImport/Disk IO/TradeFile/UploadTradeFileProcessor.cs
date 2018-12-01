using System;
using System.Collections.Generic;
using CsvHelper;
using DataImport.Disk_IO.TradeFile.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Orders.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport.Disk_IO.TradeFile
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
            List<TradeOrderFrame> marketUpdates,
            List<TradeOrderFrameCsv> failedMarketUpdateReads)
        {
            var mappedRecord = _csvToDtoMapper.Map(record);
            if (mappedRecord != null)
            {
                marketUpdates.Add(mappedRecord);
            }
            else
            {
                failedMarketUpdateReads.Add(record);
            }
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper(string path)
        {
            if (_csvToDtoMapper.FailedParseTotal > 0)
            {
                Logger.LogError($"{UploadFileProcessorName} had {_csvToDtoMapper.FailedParseTotal} rows with errors when parsing the input CSV file ({path})");
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
    }
}