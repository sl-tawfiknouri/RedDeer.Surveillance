﻿using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using DataImport.Disk_IO.TradeFile.Interfaces;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Orders;
using SharedKernel.Files.Orders.Interfaces;

namespace DataImport.Disk_IO.TradeFile
{
    public class UploadTradeFileProcessor : BaseUploadFileProcessor<OrderFileContract, Order>, IUploadTradeFileProcessor
    {
        private readonly IOrderFileToOrderSerialiser _orderFileSerialiser;
        private readonly IOrderFileValidator _orderFileValidator;

        public UploadTradeFileProcessor(
            IOrderFileToOrderSerialiser csvToDtoMapper,
            IOrderFileValidator tradeFileCsvValidator,
            ILogger<UploadTradeFileProcessor> logger)
            : base(logger, "Upload Trade File Processor")
        {
            _orderFileSerialiser = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
            _orderFileValidator = tradeFileCsvValidator ?? throw new ArgumentNullException(nameof(tradeFileCsvValidator));
        }

        protected override void MapRecord(
            OrderFileContract record,
            List<Order> marketUpdates,
            List<OrderFileContract> failedMarketUpdateReads)
        {
            Logger.LogInformation($"Upload Trade File Processor about to validate record {record?.RowId}");
            var validationResult = _orderFileValidator.Validate(record);
            
            if (!validationResult.IsValid)
            {
                Logger.LogInformation($"Upload Trade File Processor was unable to validate {record?.RowId}");
                _orderFileSerialiser.FailedParseTotal += 1;
                failedMarketUpdateReads.Add(record);

                if (validationResult.Errors.Any())
                {
                    var consolidatedErrorMessage = validationResult.Errors.Aggregate(string.Empty, (a, b) => a + " " + b.ErrorMessage);
                    Logger.LogWarning(consolidatedErrorMessage);
                }

                return;
            }

            var mappedRecord = _orderFileSerialiser.Map(record);
            if (mappedRecord != null)
            {
                Logger.LogInformation($"Upload Trade File Processor successfully validated and mapped record {record?.RowId}");
                marketUpdates.Add(mappedRecord);
            }
            Logger.LogInformation($"Upload Trade File Processor did not successfully map record {record?.RowId}");
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper(string path)
        {
            if (_orderFileSerialiser.FailedParseTotal > 0)
            {
                Logger.LogError($"{UploadFileProcessorName} had {_orderFileSerialiser.FailedParseTotal} rows with errors when parsing the input CSV file ({path})");
            }

            _orderFileSerialiser.FailedParseTotal = 0;
        }

        protected override OrderFileContract MapToCsvDto(CsvReader rawRecord, int rowId)
        {
            if (rawRecord == null)
            {
                Logger.LogInformation($"Upload Trade File Processor received a null record to map with row id {rowId}");
                return null;
            }

            Logger.LogInformation($"Upload Trade File Processor about to map raw record to csv dto");
            var tradeCsv = new OrderFileContract
            {
                MarketType = PreProcess(rawRecord["MarketType"]),
                MarketIdentifierCode = PreProcess(rawRecord["MarketIdentifierCode"]),
                MarketName = PreProcess(rawRecord["MarketName"]),

                InstrumentName = PreProcess(rawRecord["InstrumentName"]),
                InstrumentCfi = PreProcess(rawRecord["InstrumentCfi"]),
                InstrumentIssuerIdentifier = PreProcess(rawRecord["InstrumentIssuerIdentifier"]),
                InstrumentClientIdentifier = PreProcess(rawRecord["InstrumentClientIdentifier"]),
                InstrumentSedol = PreProcess(rawRecord["InstrumentSedol"]),
                InstrumentIsin = PreProcess(rawRecord["InstrumentIsin"]),
                InstrumentFigi = PreProcess(rawRecord["InstrumentFigi"]),
                InstrumentCusip = PreProcess(rawRecord["InstrumentCusip"]),
                InstrumentLei = PreProcess(rawRecord["InstrumentLei"]),
                InstrumentExchangeSymbol = PreProcess(rawRecord["InstrumentExchangeSymbol"]),
                InstrumentBloombergTicker = PreProcess(rawRecord["InstrumentBloombergTicker"]),
                InstrumentUnderlyingName = PreProcess(rawRecord["InstrumentUnderlyingName"]),
                InstrumentUnderlyingCfi = PreProcess(rawRecord["InstrumentUnderlyingCfi"]),
                InstrumentUnderlyingIssuerIdentifier = PreProcess(rawRecord["InstrumentUnderlyingIssuerIdentifier"]),
                InstrumentUnderlyingClientIdentifier = PreProcess(rawRecord["InstrumentUnderlyingClientIdentifier"]),
                InstrumentUnderlyingSedol = PreProcess(rawRecord["InstrumentUnderlyingSedol"]),
                InstrumentUnderlyingIsin = PreProcess(rawRecord["InstrumentUnderlyingIsin"]),
                InstrumentUnderlyingFigi = PreProcess(rawRecord["InstrumentUnderlyingFigi"]),
                InstrumentUnderlyingCusip = PreProcess(rawRecord["InstrumentUnderlyingCusip"]),
                InstrumentUnderlyingLei = PreProcess(rawRecord["InstrumentUnderlyingLei"]),
                InstrumentUnderlyingExchangeSymbol = PreProcess(rawRecord["InstrumentUnderlyingExchangeSymbol"]),
                InstrumentUnderlyingBloombergTicker = PreProcess(rawRecord["InstrumentUnderlyingBloombergTicker"]),
                

                OrderId = PreProcess(rawRecord["OrderId"]),
                OrderVersion = PreProcess(rawRecord["OrderVersion"]),
                OrderVersionLinkId = PreProcess(rawRecord["OrderVersionLinkId"]),
                OrderGroupId = PreProcess(rawRecord["OrderGroupId"]),
                
                OrderPlacedDate = PreProcess(rawRecord["OrderPlacedDate"]),
                OrderBookedDate = PreProcess(rawRecord["OrderBookedDate"]),
                OrderAmendedDate = PreProcess(rawRecord["OrderAmendedDate"]),
                OrderRejectedDate = PreProcess(rawRecord["OrderRejectedDate"]),
                OrderCancelledDate = PreProcess(rawRecord["OrderCancelledDate"]),
                OrderFilledDate = PreProcess(rawRecord["OrderFilledDate"]),
                
                OrderType = PreProcess(rawRecord["OrderType"]),
                OrderDirection = PreProcess(rawRecord["OrderDirection"]),
                OrderCurrency = PreProcess(rawRecord["OrderCurrency"]),
                OrderSettlementCurrency = PreProcess(rawRecord["OrderSettlementCurrency"]),
                OrderCleanDirty = PreProcess(rawRecord["OrderCleanDirty"]),
                OrderAccumulatedInterest = PreProcess(rawRecord["OrderAccumulatedInterest"]),

                OrderLimitPrice = PreProcess(rawRecord["OrderLimitPrice"]),
                OrderAverageFillPrice = PreProcess(rawRecord["OrderAverageFillPrice"]),
                OrderOrderedVolume = PreProcess(rawRecord["OrderOrderedVolume"]),
                OrderFilledVolume = PreProcess(rawRecord["OrderFilledVolume"]),

                OrderTraderId = PreProcess(rawRecord["OrderTraderId"]),
                OrderTraderName = PreProcess(rawRecord["OrderTraderName"]),
                OrderClearingAgent = PreProcess(rawRecord["OrderClearingAgent"]),
                OrderDealingInstructions = PreProcess(rawRecord["OrderDealingInstructions"]),
                
                OrderOptionStrikePrice = PreProcess(rawRecord["OrderOptionStrikePrice"]),
                OrderOptionExpirationDate = PreProcess(rawRecord["OrderOptionExpirationDate"]),
                OrderOptionEuropeanAmerican = PreProcess(rawRecord["OrderOptionEuropeanAmerican"]),
                

                DealerOrderId = PreProcess(rawRecord["DealerOrderId"]),
                DealerOrderVersion = PreProcess(rawRecord["DealerOrderVersion"]),
                DealerOrderVersionLinkId = PreProcess(rawRecord["DealerOrderVersionLinkId"]),
                DealerOrderGroupId = PreProcess(rawRecord["DealerOrderGroupId"]),

                DealerOrderPlacedDate = PreProcess(rawRecord["DealerOrderPlacedDate"]),
                DealerOrderBookedDate = PreProcess(rawRecord["DealerOrderBookedDate"]),
                DealerOrderAmendedDate = PreProcess(rawRecord["DealerOrderAmendedDate"]),
                DealerOrderRejectedDate = PreProcess(rawRecord["DealerOrderRejectedDate"]),
                DealerOrderCancelledDate = PreProcess(rawRecord["DealerOrderCancelledDate"]),
                DealerOrderFilledDate = PreProcess(rawRecord["DealerOrderFilledDate"]),

                DealerOrderDealerId = PreProcess(rawRecord["DealerOrderDealerId"]),
                DealerOrderDealerName = PreProcess(rawRecord["DealerOrderDealerName"]),
                DealerOrderNotes = PreProcess(rawRecord["DealerOrderNotes"]),

                DealerOrderCounterParty = PreProcess(rawRecord["DealerOrderCounterParty"]),
                DealerOrderType = PreProcess(rawRecord["DealerOrderType"]),
                DealerOrderDirection = PreProcess(rawRecord["DealerOrderDirection"]),
                DealerOrderCurrency = PreProcess(rawRecord["DealerOrderCurrency"]),
                DealerOrderSettlementCurrency = PreProcess(rawRecord["DealerOrderSettlementCurrency"]),
                DealerOrderCleanDirty = PreProcess(rawRecord["DealerOrderCleanDirty"]),
                DealerOrderAccumulatedInterest = PreProcess(rawRecord["DealerOrderAccumulatedInterest"]),



                DealerOrderLimitPrice = PreProcess(rawRecord["DealerOrderLimitPrice"]),
                DealerOrderAverageFillPrice = PreProcess(rawRecord["DealerOrderAverageFillPrice"]),
                DealerOrderOrderedVolume = PreProcess(rawRecord["DealerOrderOrderedVolume"]),
                DealerOrderFilledVolume = PreProcess(rawRecord["DealerOrderFilledVolume"]),

                DealerOrderOptionStrikePrice = PreProcess(rawRecord["DealerOrderOptionStrikePrice"]),
                DealerOrderOptionExpirationDate = PreProcess(rawRecord["DealerOrderOptionExpirationDate"]),
                DealerOrderOptionEuropeanAmerican = PreProcess(rawRecord["DealerOrderOptionEuropeanAmerican"]),

                RowId = rowId
            };

            Logger.LogInformation($"Upload Trade File Processor has mapped raw record to csv dto");
            return tradeCsv;
        }
    }
}