using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using DataImport.Disk_IO.TradeFile.Interfaces;
using DomainV2.Files;
using DomainV2.Files.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;

namespace DataImport.Disk_IO.TradeFile
{
    public class UploadTradeFileProcessor : BaseUploadFileProcessor<TradeFileCsv, Order>, IUploadTradeFileProcessor
    {
        private readonly ITradeFileCsvToOrderMapper _csvToDtoMapper;
        private readonly ITradeFileCsvValidator _tradeFileCsvValidator;


        public UploadTradeFileProcessor(
            ITradeFileCsvToOrderMapper csvToDtoMapper,
            ITradeFileCsvValidator tradeFileCsvValidator,
            ILogger<UploadTradeFileProcessor> logger)
            : base(logger, "Upload Trade File Processor")
        {
            _csvToDtoMapper = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
            _tradeFileCsvValidator = tradeFileCsvValidator ?? throw new ArgumentNullException(nameof(tradeFileCsvValidator));
        }

        protected override void MapRecord(
            TradeFileCsv record,
            List<Order> marketUpdates,
            List<TradeFileCsv> failedMarketUpdateReads)
        {
            Logger.LogInformation($"Upload Trade File Processor about to validate record {record?.RowId}");
            var validationResult = _tradeFileCsvValidator.Validate(record);
            
            if (!validationResult.IsValid)
            {
                Logger.LogInformation($"Upload Trade File Processor was unable to validate {record?.RowId}");
                _csvToDtoMapper.FailedParseTotal += 1;
                failedMarketUpdateReads.Add(record);

                if (validationResult.Errors.Any())
                {
                    var consolidatedErrorMessage = validationResult.Errors.Aggregate(string.Empty, (a, b) => a + " " + b.ErrorMessage);
                    Logger.LogWarning(consolidatedErrorMessage);
                }

                return;
            }

            var mappedRecord = _csvToDtoMapper.Map(record);
            if (mappedRecord != null)
            {
                Logger.LogInformation($"Upload Trade File Processor successfully validated and mapped record {record?.RowId}");
                marketUpdates.Add(mappedRecord);
            }
            Logger.LogInformation($"Upload Trade File Processor did not successfully map record {record?.RowId}");
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
                Logger.LogInformation($"Upload Trade File Processor received a null record to map with row id {rowId}");
                return null;
            }

            Logger.LogInformation($"Upload Trade File Processor about to map raw record to csv dto");
            var tradeCsv = new TradeFileCsv
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
                OrderTraderId = rawRecord["OrderTraderId"],
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

            Logger.LogInformation($"Upload Trade File Processor has mapped raw record to csv dto");
            return tradeCsv;
        }
    }
}