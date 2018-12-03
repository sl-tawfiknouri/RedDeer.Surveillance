using System;
using System.Collections.Generic;
using CsvHelper;
using DataImport.Disk_IO.TradeFile.Interfaces;
using Domain.Trades.Orders.Interfaces;
using DomainV2.Files;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;

namespace DataImport.Disk_IO.TradeFile
{
    public class UploadTradeFileProcessor : BaseUploadFileProcessor<TradeFileCsv, Order>, IUploadTradeFileProcessor
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
            TradeFileCsv record,
            List<Order> marketUpdates,
            List<TradeFileCsv> failedMarketUpdateReads)
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

        protected override TradeFileCsv MapToCsvDto(CsvReader rawRecord, int rowId)
        {
            if (rawRecord == null)
            {
                return null;
            }

            return new TradeFileCsv
            {
                MarketType = rawRecord["MarketType"],
                MarketIdentifierCode = rawRecord["MarketIdentifierCode"],
                MarketName = rawRecord["MarketName"],

                InstrumentName = rawRecord["InstrumentName"],
                InstrumentCfi = rawRecord["InstrumentCfi"],
                InstrumentIssuerIdentifier = rawRecord["InstrumentIssuerIdentifier"],
                InstrumentClientIdentifier = rawRecord["InstrumentClientIdentifier"],
                InstrumentSedol = rawRecord["InstrumentSedol"],
                InstrumentIsin = rawRecord["InstrumentIsin"],
                InstrumentFigi = rawRecord["InstrumentFigi"],
                InstrumentCusip = rawRecord["InstrumentCusip"],
                InstrumentLei = rawRecord["InstrumentLei"],
                InstrumentExchangeSymbol = rawRecord["InstrumentExchangeSymbol"],
                InstrumentBloombergTicker = rawRecord["InstrumentBloombergTicker"],
                InstrumentUnderlyingName = rawRecord["InstrumentUnderlyingName"],
                InstrumentUnderlyingCfi = rawRecord["InstrumentUnderlyingCfi"],
                InstrumentUnderlyingIssuerIdentifier = rawRecord["InstrumentUnderlyingIssuerIdentifier"],
                InstrumentUnderlyingClientIdentifier = rawRecord["InstrumentUnderlyingClientIdentifier"],
                InstrumentUnderlyingSedol = rawRecord["InstrumentUnderlyingSedol"],
                InstrumentUnderlyingIsin = rawRecord["InstrumentUnderlyingIsin"],
                InstrumentUnderlyingFigi = rawRecord["InstrumentUnderlyingFigi"],
                InstrumentUnderlyingCusip = rawRecord["InstrumentUnderlyingCusip"],
                InstrumentUnderlyingLei = rawRecord["InstrumentUnderlyingLei"],
                InstrumentUnderlyingExchangeSymbol = rawRecord["InstrumentUnderlyingExchangeSymbol"],
                InstrumentUnderlyingBloombergTicker = rawRecord["InstrumentUnderlyingBloombergTicker"],
                
                OrderId = rawRecord["OrderId"],
                OrderPlacedDate = rawRecord["OrderPlacedDate"],
                OrderBookedDate = rawRecord["OrderBookedDate"],
                OrderAmendedDate = rawRecord["OrderAmendedDate"],
                OrderRejectedDate = rawRecord["OrderRejectedDate"],
                OrderCancelledDate = rawRecord["OrderCancelledDate"],
                OrderFilledDate = rawRecord["OrderFilledDate"],
                OrderType = rawRecord["OrderType"],
                OrderPosition = rawRecord["OrderPosition"],
                OrderCurrency = rawRecord["OrderCurrency"],
                OrderLimitPrice = rawRecord["OrderLimitPrice"],
                OrderAveragePrice = rawRecord["OrderAveragePrice"],
                OrderOrderedVolume = rawRecord["OrderOrderedVolume"],
                OrderFilledVolume = rawRecord["OrderFilledVolume"],
                OrderPortfolioManager = rawRecord["OrderPortfolioManager"],
                OrderExecutingBroker = rawRecord["OrderExecutingBroker"],
                OrderClearingAgent = rawRecord["OrderClearingAgent"],
                OrderDealingInstructions = rawRecord["OrderDealingInstructions"],
                OrderStrategy = rawRecord["OrderStrategy"],
                OrderRationale = rawRecord["OrderRationale"],
                OrderFund = rawRecord["OrderFund"],
                OrderClientAccountAttributionId = rawRecord["OrderClientAccountAttributionId"],
                
                TradeId = rawRecord["TradeId"],
                TradePlacedDate = rawRecord["TradePlacedDate"],
                TradeBookedDate = rawRecord["TradeBookedDate"],
                TradeAmendedDate = rawRecord["TradeAmendedDate"],
                TradeRejectedDate = rawRecord["TradeRejectedDate"],
                TradeCancelledDate = rawRecord["TradeCancelledDate"],
                TradeFilledDate = rawRecord["TradeFilledDate"],
                TraderId = rawRecord["TraderId"],
                TradeCounterParty = rawRecord["TradeCounterParty"],
                TradeType = rawRecord["TradeType"],
                TradePosition = rawRecord["TradePosition"],
                TradeCurrency = rawRecord["TradeCurrency"],
                TradeLimitPrice = rawRecord["TradeLimitPrice"],
                TradeAveragePrice = rawRecord["TradeAveragePrice"],
                TradeOrderedVolume = rawRecord["TradeOrderedVolume"],
                TradeFilledVolume = rawRecord["TradeFilledVolume"],
                TradeOptionStrikePrice = rawRecord["TradeOptionStrikePrice"],
                TradeOptionExpirationDate = rawRecord["TradeOptionExpirationDate"],
                TradeOptionEuropeanAmerican = rawRecord["TradeOptionEuropeanAmerican"],

                TransactionId = rawRecord["TransactionId"],
                TransactionPlacedDate = rawRecord["TransactionPlacedDate"],
                TransactionBookedDate = rawRecord["TransactionBookedDate"],
                TransactionAmendedDate = rawRecord["TransactionAmendedDate"],
                TransactionRejectedDate = rawRecord["TransactionRejectedDate"],
                TransactionCancelledDate = rawRecord["TransactionCancelledDate"],
                TransactionFilledDate = rawRecord["TransactionFilledDate"],
                TransactionTraderId = rawRecord["TransactionTraderId"],
                TransactionCounterParty = rawRecord["TransactionCounterParty"],
                TransactionType = rawRecord["TransactionType"],
                TransactionPosition = rawRecord["TransactionPosition"],
                TransactionCurrency = rawRecord["TransactionCurrency"],
                TransactionLimitPrice = rawRecord["TransactionLimitPrice"],
                TransactionAveragePrice = rawRecord["TransactionAveragePrice"],
                TransactionOrderedVolume = rawRecord["TransactionOrderedVolume"],
                TransactionFilledVolume = rawRecord["TransactionFilledVolume"],

                RowId = rowId
            };
        }
    }
}