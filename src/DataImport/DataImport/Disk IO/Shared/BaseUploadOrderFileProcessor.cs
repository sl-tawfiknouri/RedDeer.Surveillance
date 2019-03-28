using CsvHelper;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Orders;

namespace DataImport.Disk_IO.Shared
{
    public abstract class BaseUploadOrderFileProcessor : BaseUploadFileProcessor<OrderFileContract, Order>
    {
        protected BaseUploadOrderFileProcessor(
            ILogger logger)
            : base(logger, "Upload Trade File Processor")
        {
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
